using Emgu.CV;
using System;
using System.Windows.Forms;
using Emgu.CV.Structure;
using System.Drawing;
using System.Threading;

namespace CS_HW_2_Emma_Harrison
{
    public partial class Form1 : Form
    {
        VideoCapture _capture;
        Thread _captureThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //create the capture object and processing thread
            _capture = new VideoCapture(0);
            _captureThread = new Thread(DisplayWebcam);
            _captureThread.Start();
        }

        private void DisplayWebcam()
        {
            while (_capture.IsOpened)
            {
                //frame maintenance
                Mat frame = _capture.QueryFrame();

                //resize to PictureBox aspect ratio
                int newHeight = (frame.Size.Height * pictureBox1.Size.Width) / frame.Size.Width;
                Size newSize = new Size(pictureBox1.Size.Width, newHeight);
                CvInvoke.Resize(frame, frame, newSize);

                //grayscaling and binary thresholding
                Mat grayscale = new Mat();
                CvInvoke.CvtColor(frame, grayscale, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                Mat binary_thresh = new Mat();
                CvInvoke.Threshold(grayscale, binary_thresh, 170, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

                //display the image in the PictureBox
                Bitmap bmp = frame.ToBitmap();
                Bitmap binaryBmp = binary_thresh.ToBitmap();
                pictureBox2.Invoke(new Action(() =>
                {
                    pictureBox2.Image = bmp;
                }));

                pictureBox3.Invoke(new Action(() =>
                {
                    pictureBox3.Image = binaryBmp;
                }));
                // pictureBox1.Image = frame.ToBitmap();

                //grayscaling and binary thresholding
                CvInvoke.Threshold(frame, frame, 150, 255,
                    Emgu.CV.CvEnum.ThresholdType.Binary);

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //terminate the image processing thread to avoid orphaned processes
            //_captureThread.Abort();
            if (_captureThread != null && _captureThread.IsAlive)
            {
                _captureThread.Join();
            }

            _capture?.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        /*private void Form1_Load_1(object sender, EventArgs e)
        {

        }*/
    }
}
