using System;
using System.Windows.Media.Imaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Microsoft.Win32;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string path;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.ShowDialog();
                path = dlg.FileName;
                poza.Source = new BitmapImage(new Uri(path));
                cale.Text = path;
            }
            catch (Exception)
            {
                cale.Text = null;
                
            }
           

        }
        public string SERIA = "";
        public string NUMARUL = "";
        public string CNP = "";
        public string NUME = "";
        public string PRENUME = "";
        public string CETATENIE = "";
        public string NASTERE = "";
        public string DOMICILIU = "";
        public string ELIBERAT = "";
        public string VALABILITATE = "";
        public string ORAS = "";
        public string JUDET = "";
        public string STRADA = "";
        public string NUMAR = "";
        private void Compute_Click(object sender, RoutedEventArgs e)
        {

            if (path==null)
            {
                return;
            }
            
            seria.Text = "";
            numar.Text = "";
            cnp.Text = "";
            nume.Text = "";
            prenume.Text = "";
            cetatenie.Text = "";
            nastere.Text = "";
            domiciliu.Text = "";
            eliberat.Text = "";
            valabilitate.Text = "";
            

            var credential = GoogleCredential.GetApplicationDefault();
            var client = ImageAnnotatorClient.Create();
            var image = Google.Cloud.Vision.V1.Image.FromFile(""+path+"");
            var res = client.DetectText(image);
            int i = 0;
            string[] date=new string[200];
            string pattern = "[0-9]{2}.[0-9]{2}.[0-9]{2}-[0-9]{2}.[0-9]{2}.[0-9]{4}";
            foreach (var rez in res)
            {
                date[i]=rez.Description.ToUpper();
                i++;
            }
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 8;//nr de core-uri
            Parallel.For(3,i,j=>
            {
                if (date[j].Contains("RIA")&&date[j-1].Contains("IDENTI")&& date[j + 1].Length==2)
                {
                    SERIA = date[j + 1];
                    return;
                }
                if (date[j] == "NR" &&date[j+1].All(char.IsDigit))
                {
                    NUMARUL = date[j + 1];
                    return;
                }
                if (date[j] == "CNP"&&date[j+1].All(char.IsNumber))
                {
                    CNP = date[j + 1];
                    return;
                }
                if (date[j]=="NAME"&&date[j-1].Contains("/LAS")&&!date[j + 1].Any(char.IsDigit))
                {
                    if (!date[j+1].Contains("/")&& !date[j + 1].Contains("IDENT")&&!date[j + 1].Contains("CARD"))
                    {
                        NUME = date[j + 1];
                    }
                    return;
                }
               
                if ((date[j - 1].Contains("/PREN")|| date[j - 1].Contains("FIR")))
                {
                    if (!date[j+1].Contains("NATIO")&&(date[j].Contains("ME")|| date[j].Contains("NA") || date[j].Contains("NAM")))
                    {
                        PRENUME = date[j + 1];
                        return;
                    }
                    else
                        PRENUME = date[j];
                         return;

                }
                if (date[j].Contains("TENIE")&&date[j].Contains("/NATIONA"))
                {
                    CETATENIE= date[j + 1];
                    return;
                }
                if (date[j].Contains("BIRT"))
                 {
                     int aux = j+1;
                     while (!date[aux].Contains("DOMICI"))
                     {
                         if (date[aux].Contains("SEX/") || date[aux].Contains("CARD")|| date[aux].All(char.IsDigit))
                         {
                             aux++;
                         }
                         else
                         {
                             NASTERE += date[aux]+" ";
                             aux++;
                         }
                     }
                    return;
                }
               if (date[j].Contains("ADR")&&date[j].Contains("DOMIC"))
                  {
                    int aux;
                    if (date[j+1].Contains("ADDRE"))
                    {
                        aux = j + 2;
                    }
                    else
                        aux = j + 1;
                    bool start = false;
                     while (!date[aux].Contains("EMIS"))
                     {
                        if (date[aux].Contains("SEX"))
                        {
                            aux++;
                            continue;
                        }
                        if ((date[aux].Contains("CARTE")|| date[aux].Contains("SPCE") || date[aux].Contains("VALI")) &&DOMICILIU.Length>0)
                        {
                            while (!date[aux].Contains("EMIS"))
                            {
                                aux++;
                            }
                            continue;
                        }
                        if (!start&&!date[aux].Contains("JUD")&& !start && !date[aux].Contains("STR") && !date[aux].Contains("MUN") && !date[aux].Contains("SAT") && !date[aux].Contains("COM"))
                        {
                            aux++;
                        }
                        else
                         {
                            start = true;
                             DOMICILIU += date[aux] + " ";
                             aux++;
                         }
                     }
                    return;
                }
                if (date[j]==("BY")||date[j-1].Contains("SSUE"))
                {
                    int aux = j + 1;
                    while (date[aux]!=null)
                    {
                        if (date[aux].Any(char.IsDigit)|| date[aux].Contains("VALID") || date[aux].Contains("SEX"))
                        {
                            aux++;
                        }
                        else
                        {
                            ELIBERAT += date[aux] + " ";
                            aux++;
                        }
                    }
                    return;
                }
                
                if (Regex.Matches(date[j].ToString(), pattern).Count>0)
                {
                    VALABILITATE = date[j];
                    return;
                }
               });
            //StreamWriter s = new StreamWriter(@"C:\Users\misco\source\repos\scanner\scanner\testin.txt");
            //s.WriteLine(SERIA);
            //s.WriteLine(NUMARUL);
            //s.WriteLine(CNP);
            //s.WriteLine(NUME);
            //s.WriteLine(PRENUME);
            //s.WriteLine(CETATENIE);
            //s.WriteLine(NASTERE);
            //s.WriteLine(DOMICILIU);
            //s.WriteLine(ELIBERAT);
            //s.WriteLine(VALABILITATE);
            //s.Close();
            Parallel.Invoke(() => insert_get_metadata(), () => set_extra_metadata());
            
            cursor.Visibility = Visibility.Hidden;
        }
        private void insert_get_metadata()
        {
            
            string[] metadate = {SERIA, NUMARUL,CNP,NUME,PRENUME,CETATENIE,NASTERE,DOMICILIU,ELIBERAT,VALABILITATE};
            OCR o = new OCR();
            List<string> help =o.initializare(metadate);
            seria.Text = help[0];
            numar.Text = help[1];
            cnp.Text = help[2];
            nume.Text = help[3];
            prenume.Text = help[4];
            cetatenie.Text = help[5];
            nastere.Text = help[6];
            domiciliu.Text = help[7];
            eliberat.Text = help[8];
            valabilitate.Text = help[9];
            set_extra_metadata();
        }
        private void set_extra_metadata()
        {
            string domiciliu_aux = DOMICILIU;
            int indexesOras=0;
            foreach (Match match in Regex.Matches(domiciliu_aux, "MUN."))
            {
                indexesOras=match.Index+match.Length;
            }
            domiciliu_aux=domiciliu_aux.Remove(0,indexesOras);
            int punct = domiciliu_aux.IndexOf(".");
            ORAS = domiciliu_aux.Substring(0, punct);
            string domiciliu_aux2 = DOMICILIU;
            int indexesJudet = 0;
            foreach (Match match in Regex.Matches(domiciliu_aux2, "JUD."))
            {
                indexesJudet = match.Index + match.Length;
            }
            domiciliu_aux2 = domiciliu_aux2.Remove(0, indexesJudet);
            int punct2 = domiciliu_aux2.IndexOf(".");
            JUDET = domiciliu_aux2.Substring(0, punct2);

            int indexesNumar = 0;
            int indexesNumarLen = 0;
            foreach (Match match in Regex.Matches(domiciliu_aux, "NR."))
            {
                indexesNumar = match.Index;
                indexesNumarLen = match.Length;
            }
            int indexesStrada = 0;
            foreach (Match match in Regex.Matches(domiciliu_aux, "STR."))
            {
                indexesStrada = match.Index + match.Length;
            }
            domiciliu_aux = domiciliu_aux.Remove(0, indexesStrada);
            STRADA = domiciliu_aux.Substring(0, indexesNumar - indexesStrada);
            int indexesNum = 0;
            foreach (Match match in Regex.Matches(domiciliu_aux, "NR."))
            {
                indexesNum = match.Index + match.Length;
            }
            domiciliu_aux = domiciliu_aux.Remove(0, indexesNum);
            int punct3 = domiciliu_aux2.IndexOf(".");
            NUMAR = domiciliu_aux.Substring(0, punct3);


        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                fotoWindow p = new fotoWindow();
                p.Owner = this;
                p.Show();
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void nextframe_Click(object sender, RoutedEventArgs e)
        {
            Pdf_uri.seria = seria.Text.ToString();
            Pdf_uri.numar = numar.Text.ToString();
            Pdf_uri.cnp = cnp.Text.ToString();
            Pdf_uri.nume = nume.Text.ToString();
            Pdf_uri.prenume = prenume.Text.ToString();
            Pdf_uri.cetatenie = cetatenie.Text.ToString();
            Pdf_uri.nastere = nastere.Text.ToString();
            Pdf_uri.domiciliu = domiciliu.Text.ToString();
            Pdf_uri.eliberat = eliberat.Text.ToString();
            Pdf_uri.valabilitate = valabilitate.Text.ToString();
            Pdf_uri.oras = ORAS;
            Pdf_uri.judet = JUDET;
            Pdf_uri.strada = STRADA;
            Pdf_uri.numarul = NUMAR;
            Window1 w = new Window1();
            w.WindowStartupLocation = this.WindowStartupLocation;
            w.Show();
        }

        private void CircularProgressBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
