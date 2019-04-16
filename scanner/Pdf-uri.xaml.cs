using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Web;
using System.Windows.Controls;
using PDFTech;
using System.IO;
using System.Drawing;
using Image = System.Drawing.Image;
using ceTe.DynamicPDF.Printing;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Input;

namespace scanner
{
    /// <summary>
    /// Interaction logic for Pdf_uri.xaml
    /// </summary>
    public partial class Pdf_uri : Window
    {
        public string option;
        public string path_for_option;
        public static string path_for_signature;
        public static string seria;
        public static string numar;
        public static string cnp;
        public static string nume;
        public static string prenume;
        public static string cetatenie;
        public static string nastere;
        public static string domiciliu;
        public static string eliberat;
        public static string valabilitate;
        public static string oras = "";
        public static string judet = "";
        public static string strada = "";
        public static string numarul = "";
        public static string data = DateTime.Today.ToLongDateString();
        public Pdf_uri()
        {

            PDFDocument.License = "UOEIOBIR-2051-191-P0050";
            InitializeComponent();
            Page_Load();

        }
        public Pdf_uri(int aux)
        {

        }

        protected void Page_Load()
        {
            DataSet ds = RunQuery("Select Tip from Tip_cereri Order by Tip ASC");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                TreeViewItem root = new TreeViewItem();
                root.Header = ds.Tables[0].Rows[i][0].ToString();
                CreateNode(root);
                arbore_cereri.Items.Add(root);
            }

        }

        void CreateNode(TreeViewItem node)
        {
            DataSet ds = RunQuery("Select Nume_cerere,Tip from Cereri,Tip_cereri Where Tip_cerere=Tip_cereri.Id AND Tip='" + node.Header.ToString() + "';");
            if (ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                TreeViewItem tnode = new TreeViewItem();
                tnode.Header = ds.Tables[0].Rows[i][0].ToString();
                if (tnode.IsSelected == true)
                {
                    //  PDF.PdfPath = @"C:\Users\misco\source\repos\scanner\CV.pdf";
                }
                node.Items.Add(tnode);
            }
        }

        DataSet RunQuery(String Query)
        {
            DataSet ds = new DataSet();
            String connStr = "Data Source=DESKTOP-773D5U6;Initial Catalog=licenta;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand objCommand = new SqlCommand(Query, conn);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(ds);
                da.Dispose();
            }
            return ds;
        }

        private void selectitem(object sender, RoutedEventArgs e)
        {

            arbore_cereri.Tag = e.OriginalSource;
            if (arbore_cereri.SelectedItem != null && arbore_cereri.SelectedItem != Parent)
            {
                TreeViewItem selectedTVI = arbore_cereri.Tag as TreeViewItem;
                DataSet ds = RunQuery("Select Cale_cerere from Cereri Where Nume_cerere='" + selectedTVI.Header.ToString() + "';");
                option = selectedTVI.Header.ToString();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PDF.PdfPath = "";
                    Regions.clear_metadata();
                    printing.Visibility = Visibility.Hidden;
                    printing.IsEnabled = false;
                    string cale = ds.Tables[0].Rows[i][0].ToString();
                    path_for_option = cale;
                    PDF.PdfPath = cale;

                }
            }
        }

        private void PDF_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public string pdf_completed = "";
        private void lastframe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Regions reg = new Regions(option.ToString());

                byte[] templatePage1 = File.ReadAllBytes(@"" + path_for_option + "");
                var result = modifyPDF(templatePage1);
                string rez = @"C:\Users\misco\source\repos\scanner\Cereri\Temp\" + option.ToString() + "_completat.pdf";
                pdf_completed = rez;
                if (File.Exists(rez)) { File.Delete(rez); }
                File.WriteAllBytes(rez, result);
                PDF.PdfPath = rez;
                printing.Visibility = Visibility.Visible;
                printing.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Operation Failed!We were unable to edit this PDF...");
            }
        }
        private byte[] modifyPDF(byte[] templatePage)
        {
            try
            {
                using (var ms = new MemoryStream(templatePage))
                {
                    using (var outputMs = new MemoryStream())
                    {
                        var options = new PDFCreationOptions();
                        var document = new PDFDocument(outputMs, options);
                        document.Pages.Delete(document.CurrentPage);
                        document.LoadPdf(ms, "");

                        foreach (PDFPage p in document.Pages)
                        {
                            p.Body.SetActiveFont("Arial", PDFFontStyles.Regular, 12);
                            p.Body.SetCursorPos(200);
                            Bitmap sign = new Bitmap(@"" + path_for_signature + "");
                            PDFImage img = new PDFImage(sign);
                            img.Transparency = 10;
                            if (Regions.semnatura.Count != 0)
                            {
                                if (Regions.semnatura.Count == 1)
                                {
                                    p.Body.AddImage(img, Convert.ToDouble(Regions.semnatura[0].ElementAt(0)), Convert.ToDouble(Regions.semnatura[0].ElementAt(1)));
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.semnatura.Count)
                                    {
                                        p.Body.AddImage(img, Convert.ToDouble(Regions.semnatura[aux].ElementAt(0)), Convert.ToDouble(Regions.semnatura[0].ElementAt(1)));
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.nume.Count != 0)
                            {
                                if (Regions.nume.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.nume[0].ElementAt(0)), float.Parse(Regions.nume[0].ElementAt(1)), float.Parse(Regions.nume[0].ElementAt(2)), float.Parse(Regions.nume[0].ElementAt(3))), nume, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.nume.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.nume[aux].ElementAt(0)), float.Parse(Regions.nume[aux].ElementAt(1)), float.Parse(Regions.nume[aux].ElementAt(2)), float.Parse(Regions.nume[aux].ElementAt(3))), nume, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.prenume.Count != 0)
                            {
                                if (Regions.prenume.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.prenume[0].ElementAt(0)), float.Parse(Regions.prenume[0].ElementAt(1)), float.Parse(Regions.prenume[0].ElementAt(2)), float.Parse(Regions.prenume[0].ElementAt(3))), prenume, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.prenume.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.prenume[aux].ElementAt(0)), float.Parse(Regions.prenume[aux].ElementAt(1)), float.Parse(Regions.prenume[aux].ElementAt(2)), float.Parse(Regions.prenume[aux].ElementAt(3))), prenume, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.oras.Count != 0)
                            {
                                if (Regions.oras.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.oras[0].ElementAt(0)), float.Parse(Regions.oras[0].ElementAt(1)), float.Parse(Regions.oras[0].ElementAt(2)), float.Parse(Regions.oras[0].ElementAt(3))), oras, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.oras.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.oras[aux].ElementAt(0)), float.Parse(Regions.oras[aux].ElementAt(1)), float.Parse(Regions.oras[aux].ElementAt(2)), float.Parse(Regions.oras[aux].ElementAt(3))), oras, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.judet.Count != 0)
                            {
                                if (Regions.judet.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.judet[0].ElementAt(0)), float.Parse(Regions.judet[0].ElementAt(1)), float.Parse(Regions.judet[0].ElementAt(2)), float.Parse(Regions.judet[0].ElementAt(3))), judet, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.judet.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.judet[aux].ElementAt(0)), float.Parse(Regions.judet[aux].ElementAt(1)), float.Parse(Regions.judet[aux].ElementAt(2)), float.Parse(Regions.judet[aux].ElementAt(3))), judet, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.strada.Count != 0)
                            {
                                if (Regions.strada.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.strada[0].ElementAt(0)), float.Parse(Regions.strada[0].ElementAt(1)), float.Parse(Regions.strada[0].ElementAt(2)), float.Parse(Regions.strada[0].ElementAt(3))), strada, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.strada.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.strada[aux].ElementAt(0)), float.Parse(Regions.strada[aux].ElementAt(1)), float.Parse(Regions.strada[aux].ElementAt(2)), float.Parse(Regions.strada[aux].ElementAt(3))), strada, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.numarul.Count != 0)
                            {
                                if (Regions.numarul.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.numarul[0].ElementAt(0)), float.Parse(Regions.numarul[0].ElementAt(1)), float.Parse(Regions.numarul[0].ElementAt(2)), float.Parse(Regions.numarul[0].ElementAt(3))), numarul, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.numarul.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.numarul[aux].ElementAt(0)), float.Parse(Regions.numarul[aux].ElementAt(1)), float.Parse(Regions.numarul[aux].ElementAt(2)), float.Parse(Regions.numarul[aux].ElementAt(3))), numarul, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.seria.Count != 0)
                            {
                                if (Regions.seria.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.seria[0].ElementAt(0)), float.Parse(Regions.seria[0].ElementAt(1)), float.Parse(Regions.seria[0].ElementAt(2)), float.Parse(Regions.seria[0].ElementAt(3))), seria, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.seria.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.seria[aux].ElementAt(0)), float.Parse(Regions.seria[aux].ElementAt(1)), float.Parse(Regions.seria[aux].ElementAt(2)), float.Parse(Regions.seria[aux].ElementAt(3))), seria, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.numar.Count != 0)
                            {
                                if (Regions.numar.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.numar[0].ElementAt(0)), float.Parse(Regions.numar[0].ElementAt(1)), float.Parse(Regions.numar[0].ElementAt(2)), float.Parse(Regions.numar[0].ElementAt(3))), numar, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.numar.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.numar[aux].ElementAt(0)), float.Parse(Regions.numar[aux].ElementAt(1)), float.Parse(Regions.numar[aux].ElementAt(2)), float.Parse(Regions.numar[aux].ElementAt(3))), numar, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.cnp.Count != 0)
                            {
                                if (Regions.cnp.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.cnp[0].ElementAt(0)), float.Parse(Regions.cnp[0].ElementAt(1)), float.Parse(Regions.cnp[0].ElementAt(2)), float.Parse(Regions.cnp[0].ElementAt(3))), cnp, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.cnp.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.cnp[aux].ElementAt(0)), float.Parse(Regions.cnp[aux].ElementAt(1)), float.Parse(Regions.cnp[aux].ElementAt(2)), float.Parse(Regions.cnp[aux].ElementAt(3))), cnp, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.eliberat.Count != 0)
                            {
                                if (Regions.eliberat.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.eliberat[0].ElementAt(0)), float.Parse(Regions.eliberat[0].ElementAt(1)), float.Parse(Regions.eliberat[0].ElementAt(2)), float.Parse(Regions.eliberat[0].ElementAt(3))), eliberat, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.eliberat.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.eliberat[aux].ElementAt(0)), float.Parse(Regions.eliberat[aux].ElementAt(1)), float.Parse(Regions.eliberat[aux].ElementAt(2)), float.Parse(Regions.eliberat[aux].ElementAt(3))), eliberat, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.valabilitate.Count != 0)
                            {
                                if (Regions.valabilitate.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.valabilitate[0].ElementAt(0)), float.Parse(Regions.valabilitate[0].ElementAt(1)), float.Parse(Regions.valabilitate[0].ElementAt(2)), float.Parse(Regions.valabilitate[0].ElementAt(3))), valabilitate, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.valabilitate.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.valabilitate[aux].ElementAt(0)), float.Parse(Regions.valabilitate[aux].ElementAt(1)), float.Parse(Regions.valabilitate[aux].ElementAt(2)), float.Parse(Regions.valabilitate[aux].ElementAt(3))), valabilitate, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.cetatenie.Count != 0)
                            {
                                if (Regions.cetatenie.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.cetatenie[0].ElementAt(0)), float.Parse(Regions.cetatenie[0].ElementAt(1)), float.Parse(Regions.cetatenie[0].ElementAt(2)), float.Parse(Regions.cetatenie[0].ElementAt(3))), cetatenie, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.cetatenie.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.cetatenie[aux].ElementAt(0)), float.Parse(Regions.cetatenie[aux].ElementAt(1)), float.Parse(Regions.cetatenie[aux].ElementAt(2)), float.Parse(Regions.cetatenie[aux].ElementAt(3))), cetatenie, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.domiciliu.Count != 0)
                            {
                                if (Regions.domiciliu.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.domiciliu[0].ElementAt(0)), float.Parse(Regions.domiciliu[0].ElementAt(1)), float.Parse(Regions.domiciliu[0].ElementAt(2)), float.Parse(Regions.domiciliu[0].ElementAt(3))), domiciliu, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.domiciliu.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.domiciliu[aux].ElementAt(0)), float.Parse(Regions.domiciliu[aux].ElementAt(1)), float.Parse(Regions.domiciliu[aux].ElementAt(2)), float.Parse(Regions.domiciliu[aux].ElementAt(3))), domiciliu, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.nastere.Count != 0)
                            {
                                if (Regions.nastere.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.nastere[0].ElementAt(0)), float.Parse(Regions.nastere[0].ElementAt(1)), float.Parse(Regions.nastere[0].ElementAt(2)), float.Parse(Regions.nastere[0].ElementAt(3))), nastere, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.nastere.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.nastere[aux].ElementAt(0)), float.Parse(Regions.nastere[aux].ElementAt(1)), float.Parse(Regions.nastere[aux].ElementAt(2)), float.Parse(Regions.nastere[aux].ElementAt(3))), nastere, true);
                                        aux++;
                                    }
                                }
                            }
                            if (Regions.data.Count != 0)
                            {
                                if (Regions.data.Count == 1)
                                {
                                    p.Body.AddTextArea(new RectangleF(float.Parse(Regions.data[0].ElementAt(0)), float.Parse(Regions.data[0].ElementAt(1)), float.Parse(Regions.data[0].ElementAt(2)), float.Parse(Regions.data[0].ElementAt(3))), data, true);
                                }
                                else
                                {
                                    int aux = 0;
                                    while (aux < Regions.data.Count)
                                    {
                                        p.Body.AddTextArea(new RectangleF(float.Parse(Regions.data[aux].ElementAt(0)), float.Parse(Regions.data[aux].ElementAt(1)), float.Parse(Regions.data[aux].ElementAt(2)), float.Parse(Regions.data[aux].ElementAt(3))), data, true);
                                        aux++;
                                    }
                                }
                            }

                        }
                        document.Save();
                        return outputMs.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("FillFields Failed!" + ex.Message);
                return null;
            }
        }

        private void printpdf(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintJob printJob = new PrintJob("HP Deskjet 1050 J410 series", pdf_completed);
                printJob.Print();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Printer not connected!!");
            }

        }
    }
}

