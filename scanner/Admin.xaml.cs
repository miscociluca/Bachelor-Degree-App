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
using System.Drawing.Imaging;
using System.Reflection;

namespace ImageCropper
{

    /// <summary>
    /// This window simply hosts a <see cref="UcImageCropper">UcImageCropper</see>
    /// control, and provides it with a new ImageUrl of the users picking
    /// </summary>
    public partial class Admin : System.Windows.Window
    {

        #region Ctor
        public Admin()
        {
            InitializeComponent();
           
        }
        #endregion


        /// <summary>
        /// get a file for the UcImageCropper to work with
        /// </summary>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            List<string> allowableFileTypes = new List<string>();
            allowableFileTypes.AddRange(new string[] { ".png", ".jpg",".jpeg", ".bmp",".gif" });
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!ofd.FileName.Equals(String.Empty))
                {
                    FileInfo f = new FileInfo(ofd.FileName);
                    if (f.Extension.ToLower().Contains("pdf"))
                    {
                        string PdfFile = f.FullName;
                        int help = PdfFile.IndexOf(".pdf");
                        string aux = PdfFile.Substring(0,help);
                        string PngFile = aux+".tiff";
                        ConvertPdfToImage(PdfFile, PngFile);
                        Bitmap Output = new Bitmap(PngFile);
                        ImageSource src = BitmapToImageSource(Output);
                        this.UcImageCropper.ImageUrl = PngFile;
                        this.UcImageCropper.cale = PdfFile;
                    }
                    if (allowableFileTypes.Contains(f.Extension.ToLower()))
                    {
                        this.UcImageCropper.ImageUrl = f.FullName;
                    }
                }
                else
                {
                    MessageBox.Show("You did pick a file to use");
                }
            }
        }

        public static void ConvertPdfToImage(string inputFile, string outputFileName)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Ghostscript.NET.Rasterizer.GhostscriptRasterizer rasterizer = null;
            Ghostscript.NET.GhostscriptVersionInfo vesion = new Ghostscript.NET.GhostscriptVersionInfo(new Version(0, 0, 0), path + @"\gsdll32.dll", string.Empty, Ghostscript.NET.GhostscriptLicense.GPL);
            using (rasterizer = new Ghostscript.NET.Rasterizer.GhostscriptRasterizer())
            {
                rasterizer.Open(inputFile, vesion, false);
                System.Drawing.Image img = rasterizer.GetPage(91,91, 1);
                img.Save(outputFileName, ImageFormat.Tiff);
                rasterizer.Close();
            }
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Tiff);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void UcImageCropper_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}