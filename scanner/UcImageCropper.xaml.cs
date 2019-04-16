using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

//Josh Smith excellent DragCanvas
using WPF.JoshSmith.Controls;
using System.Data.SqlClient;
using PDFTech;
using Style = System.Windows.Style;

namespace ImageCropper
{
    public partial class UcImageCropper : System.Windows.Controls.UserControl
    {
        public static bool initialized;
        #region CropperStyle Dependancy property
        public System.Windows.Style CropperStyle
        {
            get { return (Style)GetValue(CropperStyleProperty); }
            set { SetValue(CropperStyleProperty, value); }
        }
        public static readonly DependencyProperty CropperStyleProperty =
            DependencyProperty.Register(
            "CropperStyle",
            typeof(Style),
            typeof(UcImageCropper),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnCropperStyleChanged)));

        static void OnCropperStyleChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Style s = e.NewValue as Style;
            if (s != null)
            {
                UcImageCropper uc = (UcImageCropper)depObj;
                uc.selectCanvForImg.CropperStyle = s;
            }
        }
        #endregion

        #region Instance fields
        private string ImgUrl = "";
        private string CALE = "";
        private BitmapImage bmpSource = null;
        private SelectionCanvas selectCanvForImg = null;
        private DragCanvas dragCanvasForImg = null;
        private System.Windows.Controls.Image img = null;
        private Shape rubberBand;
        private double rubberBandLeft;
        private double rubberBandTop;
        private string tempFileName;
        private ContextMenu cmSelectionCanvas;
        private RoutedEventHandler cmSelectionCanvasRoutedEventHandler;
        private ContextMenu cmDragCanvas;
        private RoutedEventHandler cmDragCanvasRoutedEventHandler;
        private double zoomFactor = 1.0;
        #endregion

        #region Ctor
        public UcImageCropper()
        {
            InitializeComponent();
            initialized = false;
            //this.Unloaded += new RoutedEventHandler(UcImageCropper_Unloaded);
            selectCanvForImg = new SelectionCanvas();
            selectCanvForImg.CropImage += new RoutedEventHandler(selectCanvForImg_CropImage);
            dragCanvasForImg = new DragCanvas();
        }
        #endregion

        public string ImageUrl
        {
            get { return this.ImgUrl; }
            set
            {
                ImgUrl = value;
                createImageSource();
                createSelectionCanvas();
                //apply the default style if the user of this control didnt supply one
                if (CropperStyle == null)
                {
                    System.Windows.Style s = gridMain.TryFindResource("defaultCropperStyle") as Style;
                    if (s != null)
                    {
                        CropperStyle = s;
                    }
                }

            }
        }
        public string cale
        {
            get { return this.CALE; }
            set
            {
                this.CALE = value;
            }


        }
        public void CleanUp(string tempPath, string fixedTempName, long CurrentFixedTempIdx)
        {
            //clean up the single temporary file created
            try
            {
                string filename = "";
                for (int i = 0; i < CurrentFixedTempIdx; i++)
                {
                    filename = tempPath + fixedTempName + i.ToString() + ".jpg";
                    File.Delete(filename);
                }
            }
            catch (Exception)
            {
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            createSelectionCanvas();
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ImageUrl = tempFileName;
        }
        private void createSelectionCanvas()
        {
            createImageSource();
            selectCanvForImg.Width = bmpSource.Width;
            selectCanvForImg.Height = bmpSource.Height;
            selectCanvForImg.Children.Clear();
            selectCanvForImg.rubberBand = null;
            selectCanvForImg.Children.Add(img);
            //svForImg.Width = selectCanvForImg.Width;
            //svForImg.Height = selectCanvForImg.Height;
            svForImg.Content = selectCanvForImg;
            createSelectionCanvasMenu();
        }

        private void createSelectionCanvasMenu()
        {
            cmSelectionCanvas = new ContextMenu();
            MenuItem miZoom25 = new MenuItem();
            miZoom25.Header = "Zoom 25%";
            miZoom25.Tag = "0.25";
            MenuItem miZoom50 = new MenuItem();
            miZoom50.Header = "Zoom 50%";
            miZoom50.Tag = "0.5";
            MenuItem miZoom100 = new MenuItem();
            miZoom100.Header = "Zoom 100%";
            miZoom100.Tag = "1.0";
            cmSelectionCanvas.Items.Add(miZoom25);
            cmSelectionCanvas.Items.Add(miZoom50);
            cmSelectionCanvas.Items.Add(miZoom100);
            cmSelectionCanvasRoutedEventHandler = new RoutedEventHandler(MenuSelectionCanvasOnClick);
            cmSelectionCanvas.AddHandler(MenuItem.ClickEvent, cmSelectionCanvasRoutedEventHandler);
            selectCanvForImg.ContextMenu = cmSelectionCanvas;
        }
        private void MenuSelectionCanvasOnClick(object sender, RoutedEventArgs args)
        {
            MenuItem item = args.Source as MenuItem;
            zoomFactor = double.Parse(item.Tag.ToString());
            img.RenderTransform = new ScaleTransform(zoomFactor, zoomFactor, 0.5, 0.5);
            selectCanvForImg.Width = bmpSource.Width * zoomFactor;
            selectCanvForImg.Height = bmpSource.Height * zoomFactor;
            svForImg.Width = selectCanvForImg.Width;
            svForImg.Height = selectCanvForImg.Height;

        }
        private void createImageSource()
        {
            bmpSource = new BitmapImage(new Uri(ImgUrl));
            img = new System.Windows.Controls.Image();
            img.Source = bmpSource;
        }
        private void createDragCanvas()
        {
            dragCanvasForImg.Width = bmpSource.Width;
            dragCanvasForImg.Height = bmpSource.Height;
            //svForImg.Width = dragCanvasForImg.Width;
            //svForImg.Height = dragCanvasForImg.Height;
            createImageSource();
            createDragCanvasMenu();
            selectCanvForImg.Children.Remove(rubberBand);
            dragCanvasForImg.Children.Clear();
            dragCanvasForImg.Children.Add(img);
            dragCanvasForImg.Children.Add(rubberBand);
            svForImg.Content = dragCanvasForImg;
        }
        private void createDragCanvasMenu()
        {
            cmSelectionCanvas.RemoveHandler(MenuItem.ClickEvent, cmSelectionCanvasRoutedEventHandler);
            selectCanvForImg.ContextMenu = null;
            cmSelectionCanvas = null;
            cmDragCanvas = new ContextMenu();
            MenuItem miCancel = new MenuItem();
            miCancel.Header = "Cancel";
            MenuItem miSave = new MenuItem();
            miSave.Header = "Save";
            cmDragCanvas.Items.Add(miCancel);
            cmDragCanvas.Items.Add(miSave);
            cmDragCanvasRoutedEventHandler = new RoutedEventHandler(MenuDragCanvasOnClick);
            cmDragCanvas.AddHandler(MenuItem.ClickEvent, cmDragCanvasRoutedEventHandler);
            dragCanvasForImg.ContextMenu = cmDragCanvas;
        }
        private void MenuDragCanvasOnClick(object sender, RoutedEventArgs args)
        {
            MenuItem item = args.Source as MenuItem;
            switch (item.Header.ToString())
            {
                case "Save":
                    SaveCroppedImage();
                    break;
                case "Cancel":
                    createSelectionCanvas();
                    break;
                default:
                    break;
            }
        }
        private void selectCanvForImg_CropImage(object sender, RoutedEventArgs e)
        {
            rubberBand = (Shape)selectCanvForImg.Children[1];
            createDragCanvas();
        }
        private void lblExit_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            createSelectionCanvas();
        }
        private void SaveCroppedImage()
        {
            try
            {
                int pdfheight = 0, pdfwidth = 0;
                string top = "", left = "", width = "", height = "";
                rubberBandLeft = System.Windows.Controls.Canvas.GetLeft(rubberBand);
                rubberBandTop = System.Windows.Controls.Canvas.GetTop(rubberBand);
                top = rubberBandTop.ToString().Length > 5 ? rubberBandTop.ToString().Substring(0, 5) : rubberBandTop.ToString();
                left = rubberBandLeft.ToString().Length > 5 ? rubberBandLeft.ToString().Substring(0, 5) : rubberBandLeft.ToString();
                width = rubberBand.Width.ToString().Length > 5 ? rubberBand.Width.ToString().Substring(0, 5) : rubberBand.Width.ToString();
                height = rubberBand.Height.ToString().Length > 5 ? rubberBand.Height.ToString().Substring(0, 5) : rubberBand.Height.ToString();
                byte[] templatePage1 = File.ReadAllBytes(@"" + cale + "");
                using (var ms = new MemoryStream(templatePage1))
                {
                    using (var outputMs = new MemoryStream())
                    {
                        var options = new PDFCreationOptions();
                        var document = new PDFDocument(outputMs, options);
                        document.Pages.Delete(document.CurrentPage);
                        document.LoadPdf(ms, "");
                        PDFPage pag = document.Pages[0];
                        pdfheight = (int)pag.PageSetupInfo.PageHeight;
                        pdfwidth = (int)pag.PageSetupInfo.PageWidth;
                    }
                }
                int x = (int)((Convert.ToDouble(left) + 0.5) * pdfwidth / (int)svForImg.Width) - 5;
                int y = (int)(Convert.ToDouble(top) + 0.5) * pdfwidth / (int)svForImg.Width;
                X1.Text = Convert.ToString(x);
                Y1.Text = Convert.ToString(y);
                X2.Text = width;
                Y2.Text = height;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Button1_Click(object sender, RoutedEventArgs e)
        {

            string nume_cerere = NumeCerere.Text.ToString();
            string tip_cerere = TipCerere.Text.ToString();
            string calea_initiala = cale != null ? cale : ImgUrl;
            string cale_finala = "";
            if (nume_cerere == "")
            {
                MessageBox.Show("Completați câmpul referitor la numele formularului");
            }
            if (tip_cerere == "")
            {
                MessageBox.Show("Completați câmpul referitor la tipul formularului");
            }
            else
            {
                if (!initialized)
                {
                    try
                    {
                        string str = calea_initiala;
                        string dir = @"C:\Users\misco\source\repos\scanner\Cereri";
                        File.Copy(str, string.Format(dir + string.Format("\\" + nume_cerere + System.IO.Path.GetExtension(str).ToString())));
                        cale_finala = string.Format(dir + string.Format("\\" + nume_cerere + System.IO.Path.GetExtension(str).ToString()));
                        insertFileinDatabase(cale_finala, tip_cerere, nume_cerere);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                string element = "", x = "", y = "", width = "", height = "";
                element = optiune.Text.ToString().ToLower();
                x = X1.Text.ToString();
                y = Y1.Text.ToString();
                width = X2.Text.ToString();
                height = Y2.Text.ToString();
                string path = @"C:\Users\misco\source\repos\scanner\Cereri";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string path_final = path + @"\" + nume_cerere + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path_final, true))
                {
                    file.WriteLine(string.Format("{0} {1} {2} {3} {4}", element, x, y, width, height));
                    file.Close();
                }
            }
            X1.Text = "";
            Y1.Text = "";
            X2.Text = "";
            Y2.Text = "";
            initialized = true;
        }
        public static void insertFileinDatabase(string cale, string tip, string nume)
        {
            using (var connection = new SqlConnection("Data Source=DESKTOP-773D5U6;Initial Catalog=licenta;Integrated Security=True"))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("Select Id from Tip_cereri where Tip=@val", connection);
                command.Parameters.AddWithValue("@val", tip);
                int result = (Int32)command.ExecuteScalar();

                var sql = "INSERT INTO Cereri(Nume_cerere, Tip_cerere,Cale_cerere) VALUES(@Nume, @Tip,@Cale)";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@Tip", result);
                    cmd.Parameters.AddWithValue("@Cale", cale);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}