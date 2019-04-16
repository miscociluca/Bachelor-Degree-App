using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Ink;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;

namespace scanner
{
    public partial class Window1 : Window
    {
       public double leftx;
        public double lefty;
        public double width;
        public double height;
        public Window1()
        {
            InitializeComponent();            
        }
        public Bitmap CropImage(Bitmap source, System.Drawing.Rectangle section)
        {
            try
            {
                section.Width = section.Width + 30;
                section.Height = section.Height + 20;
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                source.Dispose();
                return bmp;
            }
            catch (Exception)
            {
                
                return null;
            }
        }
        void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            int margin = (int)this.MyInkCanvas.Margin.Left;
            int Width = (int)this.MyInkCanvas.ActualWidth - margin;
            int Height = (int)this.MyInkCanvas.ActualHeight - margin;
          
            RenderTargetBitmap rtb = new RenderTargetBitmap(Width, Height, 96d, 96d, PixelFormats.Default);
            rtb.Render(MyInkCanvas);


           
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            string path = "C://Users//misco//source//repos//scanner//auxilar.jpg";
            using (var files = new FileStream(path, FileMode.Create))
            {
                encoder.Save(files);
               
            }
            
            Bitmap source = new Bitmap(path);
            System.Drawing.Rectangle section = new System.Drawing.Rectangle(new System.Drawing.Point((int)leftx,(int)lefty), new System.Drawing.Size((int)this.width,(int)this.height));
            if (section.IsEmpty)
            {
                System.Windows.MessageBox.Show("Doriti sa nu va inregistrati semnatura?");
                if (MessageBoxResult.OK != 0)
                {
                    Pdf_uri.path_for_signature =null;
                    this.Close();
                    Pdf_uri p = new Pdf_uri();
                    //p.Owner = this;
                    p.Show();
                }
            }
            else
            {
                Bitmap CroppedImage = CropImage(source, section);
                CroppedImage.Save("C://Users//misco//source//repos//scanner//auxilar.jpg");
                Pdf_uri.path_for_signature = path;
                this.Close();
                Pdf_uri p = new Pdf_uri();
                //p.Owner = this;
                p.Show();
            }

        }

        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.MyInkCanvas.Strokes.Clear();
            this.MyInkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
        }

        private void select_but_Click(object sender, RoutedEventArgs e)
        {
            this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Select;
            var bounds = MyInkCanvas.GetSelectionBounds();
            leftx = bounds.TopLeft.X;
            lefty = bounds.TopLeft.Y;
            double maxwidth = bounds.TopRight.X > bounds.BottomRight.X ? bounds.TopRight.X : bounds.BottomRight.X;
            width = maxwidth - bounds.TopLeft.X;
            double maxheight = bounds.BottomLeft.Y > bounds.BottomRight.Y ? bounds.BottomLeft.Y : bounds.BottomRight.Y;
            height = maxheight - bounds.TopLeft.Y;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            MainWindow main = new MainWindow();
            if (main.ShowActivated)
            {
                main.Focus();
            }
            else
                main.Show();
        }
    }

}