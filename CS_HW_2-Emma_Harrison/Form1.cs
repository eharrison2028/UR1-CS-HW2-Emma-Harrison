using Emgu.CV;
using System;
using System.Windows.Forms;
using Emgu.CV.Structure;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;
using Emgu.CV.CvEnum;

namespace CS_HW_2_Emma_Harrison
{
    public partial class Form1 : Form
    {
        VideoCapture _capture;
        Thread _captureThread;
        //private object meanLbl;

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
                int whitePixelsLeftQuarter = 0;
                int whitePixelsRightQuarter = 0;

                //resize to PictureBox aspect ratio
                int newHeight = (frame.Size.Height * pictureBox1.Size.Width) / frame.Size.Width;
                Size newSize = new Size(pictureBox1.Size.Width, newHeight);
                CvInvoke.Resize(frame, frame, newSize);
               
                //grayscaling and binary thresholding
                Mat grayscale = new Mat();
                CvInvoke.CvtColor(frame, grayscale, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                var mean = CvInvoke.Mean(grayscale);
                //Invoke(new Action(() =>
                //{
                 //   meanLbl.Text = $"Mean: {mean.V0}";
                //}));

                Mat binary_thresh = new Mat();
                CvInvoke.Threshold(grayscale, binary_thresh, mean.V0, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                //175,255

                Image<Gray, byte> img = binary_thresh.ToImage<Gray, byte>();
                Image<Gray, byte> img2 = binary_thresh.ToImage<Gray, byte>();

                img.ROI = new Rectangle(0, frame.Height - frame.Height / 4, frame.Width / 2, frame.Height / 4);

                whitePixelsLeftQuarter = img.CountNonzero()[0];
                Console.WriteLine($"Left ROI: {whitePixelsLeftQuarter}");
                CvInvoke.PutText(frame, $"L: {whitePixelsLeftQuarter}", new Point(10, 30), FontFace.HersheySimplex, 1.2, new MCvScalar(0, 0, 255), 2);
                // img.ROI = Rectangle.Empty;

                img2.ROI = new Rectangle(frame.Width / 2, frame.Height - frame.Height / 4, frame.Width / 2, frame.Height / 4);
                whitePixelsRightQuarter = img2.CountNonzero()[0];
                Console.WriteLine($"Right ROI: {whitePixelsRightQuarter}");
                CvInvoke.PutText(frame, $"R: {whitePixelsRightQuarter}", new Point(frame.Width / 2 + 10, 30), FontFace.HersheySimplex, 1.2, new MCvScalar(0, 255, 0), 2);
                // img2.ROI = Rectangle.Empty;

                CvInvoke.Rectangle(frame, img.ROI, new MCvScalar(0, 0, 255), 2);
                CvInvoke.Rectangle(frame, img2.ROI, new MCvScalar(0, 255, 0), 2);

                string decision;

                if (whitePixelsLeftQuarter > whitePixelsRightQuarter + 500)
                {
                    decision = "TURN LEFT";
                }
                else if (whitePixelsRightQuarter > whitePixelsLeftQuarter + 500)
                {
                    decision = "TURN RIGHT";
                }
                else
                {
                    decision = "FORWARD";
                }
                //update

                CvInvoke.PutText(
                    frame,
                    decision,
                    new Point(frame.Width / 2 - 80, 100),
                    FontFace.HersheySimplex,
                    1.5,
                    new MCvScalar(255, 0, 0), // blue
                    3
                );
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
                img.ROI = Rectangle.Empty;
                img2.ROI = Rectangle.Empty;
                //grayscaling and binary thresholding
                //CvInvoke.Threshold(frame, frame, mean.V0, 255,
                  //  Emgu.CV.CvEnum.ThresholdType.Binary);



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
