using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using System.Drawing.Imaging;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //declaring global variables
        public Capture capture;        //takes images from camera as image frames
        public bool captureInProgress;
        public HaarCascade haar;

        public CameraCapture()
        {
            InitializeComponent();
        }
        //------------------------------------------------------------------------------//
        //Process Frame() below is our user defined function in which we will create an EmguCv 
        //type image called ImageFrame. capture a frame from camera and allocate it to our 
        //ImageFrame. then show this image in ourEmguCV imageBox
        //------------------------------------------------------------------------------//
        public void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> ImageFrame = capture.QuerySmallFrame();

            if (ImageFrame != null) {

                Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
                var faces=
                    grayframe.DetectHaarCascade(haar,1.4,4,Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,new Size(25,25))[0];
                foreach (var face in faces) {

                    ImageFrame.Draw(face.rect, new Bgr(Color.Green), 3);
                
                }
            
            
            
            
            }
            CamImgBox.Image = ImageFrame;

            
        }

        //btnStart_Click() function is the one that handles our "Start!" button' click 
        //event. it creates a new capture object if its not created already. e.g at first time
        //starting. once the capture is created, it checks if the capture is still in progress,
        //if so the
        public void btnStart_Click_1(object sender, EventArgs e)
        {

        

            #region if capture is not created, create it now
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            #endregion

            if (capture != null)
            {
                if (captureInProgress)
                {  //if camera is getting frames then stop the capture and set button Text
                    // "Start" for resuming capture
                    btnStart.Text = "Start!"; //
                    Application.Idle -= ProcessFrame;
                }
                else
                {
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Stop" for pausing capture
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }

                captureInProgress = !captureInProgress;
            }
        }

        public void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        public void CameraCapture_Load(object sender, EventArgs e)
        {
                haar = new HaarCascade("haarcascade_frontalface_alt2.xml");
                
        }

        public void Capturebtn_Click(object sender, EventArgs e)
        {
            CaptureBox.Image = CamImgBox.Image;
        }

        public void Savebtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter ="Image Files(*.JPG;)|*.JPG;|All files (*.*)|*.*";
            if (dialog.ShowDialog()==DialogResult.OK)
            {
                int width = Convert.ToInt32(CaptureBox.Width);
                int height = Convert.ToInt32(CaptureBox.Height);
                Bitmap bmp = new Bitmap(width,height);
                CaptureBox.DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
            }
        }
    }
}
