using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.IO;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Practical2
{
    public partial class Form1 : Form
    {
        #region Variables
        private Capture capture;
        // HaarCascade class is replaced by CascadeClassifier because it's deprecated
        //private HaarCascade haarCascade;
        private CascadeClassifier cascadeClassifier;
        SaveFileDialog saveFile;
        OpenFileDialog openFile;
        VideoWriter writer;
        DispatcherTimer timer;
        int frameWidth, frameHeight;
        double frameRate = 10;
        Image<Bgr, byte> img;
        Size size1 = new System.Drawing.Size(100, 100);
        Size size2 = new System.Drawing.Size(500, 500);
        int gs1 = 5;
        int gs2 = 5;
        int gs3 = 2;
        int gs4 = 2;

        Rectangle[] rect = new Rectangle[4];

        bool play = false;
        bool record = false;
        #endregion

        VideoState state = VideoState.Viewing;
        public enum VideoState
        {
            Viewing,
            Recording
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Marker Tracking!";
            
        }

        private void Update(object sender, EventArgs e)
        {
            if (state == VideoState.Viewing)
            {
                try
                { showImage(capture.RetrieveBgrFrame().ToBitmap()); }
                catch { }
            }

            else if (state == VideoState.Recording)
            {
                img = capture.RetrieveBgrFrame();
                showImage(capture.RetrieveBgrFrame().ToBitmap());

                if (record && writer.Ptr != IntPtr.Zero)
                { writer.WriteFrame(img); }
            }
        }

        private delegate void ShowImageDelegate(Bitmap image);
        private void showImage(Bitmap image)
        {
            if (imageBox.InvokeRequired)
            {
                try
                {
                    ShowImageDelegate DI = new ShowImageDelegate(showImage);
                    this.BeginInvoke(DI, new object[] { image });
                }
                catch { }
            }
            else
            {
                imageBox.Image = image;
            }
        }

        #region Button Eventhandlers
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                if (state == VideoState.Viewing)
                {
                    play = !play;
                    if (play)
                    {
                        btnStart.Text = "Pause";
                        btnStart.ForeColor = Color.Black;
                        capture.ImageGrabbed += Update;
                        capture.Start();
                    }
                    else
                    {
                        btnStart.Text = "Play";
                        capture.Pause();
                    }
                }
                else if (state == VideoState.Recording)
                {
                    record = !record;
                    if (record)
                    {
                        btnStart.Text = "Stop Recording";
                        btnStart.ForeColor = Color.Red;
                        if (writer.Ptr == IntPtr.Zero)
                            checkRecord_CheckedChanged(null, null);
                    }
                    else
                    {
                        writer.Dispose();
                        btnStart.ForeColor = Color.Black;
                        btnStart.Text = "Record Video";
                    }
                }
            }
            else
            {
                capture = new Capture();
                btnStart_Click(sender, e);
            }
        }

        private void openFiles(bool videoFile)
        {
            openFile = new OpenFileDialog();
            if(videoFile)
                openFile.Filter = "Video Files|*.avi;*.mp4;*.mpg";
            else
                openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                if (capture != null)
                {
                    if (capture.GrabProcessState == System.Threading.ThreadState.Running)
                        capture.Stop();
                    capture.Dispose();
                }
                try
                {
                    if (videoFile)
                    {
                        this.Text = "Viewing Video: " + openFile.FileName;
                        state = VideoState.Viewing;
                        capture = new Capture(openFile.FileName);
                        capture.ImageGrabbed += Update;

                        btnStart.Text = "Play";
                        btnStart.ForeColor = Color.Green;
                        play = false;
                    }
                    else
                    {
                        this.Text = "Scanning image: " + openFile.FileName;

                        trainingStage();
                    }
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            buttonsClicked(false);            
        }

        private void btnFaceDetection_Click(object sender, EventArgs e)
        {
            buttonsClicked(true);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please load the cropped marker image");
            openFiles(false);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFiles(true);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if(timer != null)
                timer.IsEnabled = false;
            btnStart.Text = "Play";
            play = false;
            record = false;
            if (capture != null)
            {
                capture.Dispose();
                imageBox.Image = null;             
            }
        }

        private void buttonsClicked(bool needCascade)
        {
            capture = new Capture();
            if(needCascade)
                cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            timer = new DispatcherTimer();
            if(needCascade)
                timer.Tick += timer_Tick;
            else
                timer.Tick += marker_Detection;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }
        #endregion

        private void checkRecord_CheckedChanged(object sender, EventArgs e)
        {
            saveFile = new SaveFileDialog();
            if (checkRecord.Checked)
            {
                saveFile.Filter = "Video Files|*.avi;*.mp4;*.mpg";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (capture != null)
                    {
                        if (capture.GrabProcessState == System.Threading.ThreadState.Running)
                            capture.Stop();
                        capture.Dispose();
                    }
                    try
                    {
                        this.Text = "Saving Video: " + saveFile.FileName;

                        state = VideoState.Recording;

                        capture = new Capture();
                        capture.ImageGrabbed += Update;

                        frameWidth = (int)capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH);
                        frameHeight = (int)capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT);

                        writer = new VideoWriter(saveFile.FileName, -1, (int)frameRate, frameWidth, frameHeight, true);

                        btnStart.Text = "Record Video";

                        record = false;

                        capture.Start();
                    }
                    catch (NullReferenceException exception)
                    { MessageBox.Show(exception.Message); }
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            img = capture.QueryFrame(); Point location = new Point();
            if (img != null)
            {
                Image<Gray, Byte> grayImage = img.Convert<Gray, Byte>();

                //var faces = grayImage.DetectHaarCascade(haarCascade)[0];

                var faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 1, size1, size2);
                foreach (var face in faces)
                {
                    img.Draw(face, new Bgr(0, 0, 255), 2);
                    location.X = face.Location.X + face.Size.Width / 2;
                    location.Y = face.Location.Y + face.Size.Height / 2;
                }

                if (location != new Point(0, 0))
                {
                    if (location.X < img.Width / 2 && location.Y < img.Height / 2)
                        img.Draw(rect[0], new Bgr(0, 255, 0), 3);
                    else if (location.X < img.Width && location.Y < img.Height / 2)
                        img.Draw(rect[1], new Bgr(0, 255, 0), 3);
                    else if (location.X < img.Width / 2 && location.Y < img.Height)
                        img.Draw(rect[2], new Bgr(0, 255, 0), 3);
                    else if (location.X < img.Width && location.Y < img.Height)
                        img.Draw(rect[3], new Bgr(0, 255, 0), 3);
                }

                showImage(img.ToBitmap());

                Graphics g = Graphics.FromImage(imageBox.Image);
                Pen pen = new Pen(Color.Black, 3);
                rect[0] = new Rectangle(0, 0, img.Width / 2, img.Height / 2);
                rect[1] = new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2);
                rect[2] = new Rectangle(0, img.Height / 2, img.Width / 2, img.Height / 2);
                rect[3] = new Rectangle(img.Width / 2, img.Height / 2, img.Width / 2, img.Height / 2);
                g.DrawRectangles(pen, rect);
            }
        }


        // Converts a two-dimensional array
        public static TOutput[,] ConvertAll<TInput, TOutput>(TInput[,] arr, Converter<TInput, TOutput> conv)
        {
            int h = arr.GetLength(0);
            int w = arr.GetLength(1);
            TOutput[,] locarr = new TOutput[w, h];
            for (int k = 0; k < h; k++)
            {
                for (int l = 0; l < w; l++)
                    locarr[k, l] = conv(arr[k, l]);
            }
            return locarr;
        }

        private void trainingStage()
        {
            // HS space
            Image<Bgr, Byte> origImg = new Image<Bgr, Byte>(openFile.FileName);
            Image<Bgr, Byte> smoothImg = origImg.SmoothGaussian(gs1, gs2, gs3, gs4);
            Image<Hsv, Byte> hsvImg = smoothImg.Convert<Hsv, Byte>();

            /* 2D histogram
            double[] histHue = new double[hsImg.Width];
            double[] histSat = new double[hsImg.Height];

            for (int x = 1; x < hsImg.Width; x++)
            {
                for (int y = 1; y < hsImg.Height; y++)
                {
                    if (hsImg.Data[y, x, 0] == 255)
                    {
                        histHue[x]++;
                        histSat[y]++;
                    }
                }
            }

            // Normalization
            histHue = Array.ConvertAll(histHue, item => item / hsImg.Width);
            histSat = Array.ConvertAll(histSat, item => item / hsImg.Height);
            */
                        
            // 3D histogram
            double[,] hist3D = new double[10, 10];

            for (int x = 0; x < hsvImg.Width; x++)
            {
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    double h = Math.Round(hsvImg.Data[y, x, 0] / 255d, 1);
                    h = (h == 0)? 0 : ((h - 0.1) * 10d);
                                
                    double s = Math.Round(hsvImg.Data[y, x, 1] / 255d, 1);
                    s = (s == 0)? 0 : ((s - 0.1) * 10d);
                                
                    hist3D[(int)h, (int)s]++;
                }
            }

            // Normalization
            double max = hist3D.Cast<double>().Max();
            hist3D = ConvertAll(hist3D, item => item / max);
            WriteOut(hist3D, @"C:\test.txt");
            MessageBox.Show("Three dimensional histogram created");


            // save histogram>!

            showImage(hsvImg.ToBitmap());

        }

        static void WriteOut(double[,] testValues, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine(testValues.GetLength(0));
            writer.WriteLine(testValues.GetLength(1));
            for (int i = 0; i < testValues.GetLength(0); i++)
            {
                for (int j = 0; j < testValues.GetLength(1); j++)
                {
                    writer.WriteLine(testValues[i, j]);
                }
            }
            writer.Close();
        }

        static double[,] ReadIn(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            int width;
            int height;
            int.TryParse(reader.ReadLine(), out width);
            int.TryParse(reader.ReadLine(), out height);
            double[,] readValues = new double[width, height];
            for (int i = 0; i < readValues.GetLength(0); i++)
            {
                for (int j = 0; j < readValues.GetLength(1); j++)
                {
                    double.TryParse(reader.ReadLine(), out readValues[i, j]);
                }
            }
            reader.Close();
            return readValues;
        }

        private void marker_Detection(object sender, EventArgs e)
        {
            Image<Bgr, Byte> origImg = capture.RetrieveBgrFrame();
            Image<Bgr, Byte> smoothImg = origImg.SmoothGaussian(gs1, gs2, gs3, gs4);
            Image<Hsv, Byte> hsvImg = smoothImg.Convert<Hsv, Byte>();
            /*
            Image<Gray, Byte>[] channels = hsvImg.Split();
            Image<Gray, Byte> hue = channels[0].InRange(new Gray(0), new Gray(30));
            Image<Gray, Byte> saturation = channels[1].InRange(new Gray(200), new Gray(255));
            Image<Gray, Byte> hsImg = (hue.And(saturation));
            

            // 2D histogram
            double[] histX = new double[hsImg.Width];
            double[] histY = new double[hsImg.Height];
            
            for (int x = 1; x < hsImg.Width; x++)
            {
                for (int y = 1; y < hsImg.Height; y++)
                {
                    if (hsImg.Data[y, x, 0] == 255)
                    {
                        histX[x]++;
                        histY[y]++;
                    }
                }
            }
            
            // Normalization
            histX = Array.ConvertAll(histX, item => item / hsImg.Width);
            histY = Array.ConvertAll(histY, item => item / hsImg.Height);
            */

            // 3D histogram
            double[,] hist3D = ReadIn(@"C:\test.txt");
            double[,] Tspace = new double[hsvImg.Width, hsvImg.Height];
            double threshold = 0.5; // Greater than threshold

            for (int x = 0; x < hsvImg.Width; x++)
            {
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    double h = Math.Round(hsvImg.Data[y, x, 0] / 255d, 1);
                    h = (h == 0) ? 0 : ((h - 0.1) * 10d);

                    double s = Math.Round(hsvImg.Data[y, x, 1] / 255d, 1);
                    s = (s == 0) ? 0 : ((s - 0.1) * 10d);

                    if (hist3D[(int)h, (int)s] > threshold)
                    {
                        Tspace[x, y] = 255;
                        // TEST maak de marker zwart
                        hsvImg.Data[y, x, 0] = 0;
                        hsvImg.Data[y, x, 1] = 0;
                        hsvImg.Data[y, x, 2] = 0;
                    }
                    else
                        Tspace[x, y] = 0;
                }
            }

            double[] histX = new double[hsvImg.Width];
            double[] histY = new double[hsvImg.Height];

            for (int x = 0; x < hsvImg.Width; x++)
            {
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    if (hsvImg.Data[y, x, 0] == 255)
                    {
                        histX[x]++;
                        histY[y]++;
                    }
                }
            }








            //histX = Array.ConvertAll(histX, item => (item < .5 * histX.Max()) ? 0 : item);
            // histY = Array.ConvertAll(histY, item => (item < .5 * histX.Max()) ? 0 : item);



            //origImg.Draw(new CircleF(new PointF((float)xMu, (float)yMu), (float)ySigma*2), new Bgr(0, 255, 0), 3);


            btnStart.Text = Tspace[0, 0].ToString();
            showImage(hsvImg.ToBitmap());
        }

        private double gaussian(double value, double[] hist)
        {
            hist = Array.ConvertAll(hist, item => item / img.Width);
            int x1 = Array.FindIndex(hist, item => item > 0);
            int x2 = Array.FindLastIndex(hist, item => item > 0);
            int N = x2 - x1;
            double R = hist.Sum();
            double Mu = R / N;
            double[] histX2 = Array.ConvertAll(hist, item => Math.Pow(item - Mu, 2));
            double Sigma = Math.Sqrt(histX2.Sum() / N);
            return 1 / (Sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-Math.Pow(value - Mu, 2) / (2 * Math.Pow(Sigma, 2)));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (capture != null)
            {
                if (capture.GrabProcessState == System.Threading.ThreadState.Running) capture.Stop();
                capture.Dispose();
            }
        }
    }
}