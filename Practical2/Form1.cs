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
        int cx, cy, x1, y1, x2, y2;

        Rectangle[] rect = new Rectangle[4];

        bool markerdetected = false;
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


        // Converts a two-dimensional array.
        private static TOutput[,] ConvertAll<TInput, TOutput>(TInput[,] arr, Converter<TInput, TOutput> conv)
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

        // Computes a 3D histogram from an image
        private void trainingStage()
        {
            // Convert the input image from RGB to HSV space
            Image<Bgr, Byte> origImg = new Image<Bgr, Byte>(openFile.FileName);
            Image<Bgr, Byte> smoothImg = origImg.SmoothGaussian(gs1, gs2, gs3, gs4);
            Image<Hsv, Byte> hsvImg = smoothImg.Convert<Hsv, Byte>();

            // Declare the 2 dimensional array for the 3D histogram
            double[,] hist3D = new double[10, 10];

            // Scan every pixel in the image
            for (int x = 0; x < hsvImg.Width; x++)
            {
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    // Normalize the Hue and Saturation
                    double h = Math.Round(hsvImg.Data[y, x, 0] / 255d, 1);
                    double s = Math.Round(hsvImg.Data[y, x, 1] / 255d, 1);

                    // Convert the normalized Hue and Saturation to input for the indices of the hist3D array
                    h = (h == 0) ? 0 : ((h - 0.1) * 10d);
                    s = (s == 0) ? 0 : ((s - 0.1) * 10d);

                    // Increment the value at the specific hue, saturation by 1
                    hist3D[(int)h, (int)s]++;
                }
            }

            // Normalize the 3D histogram by. Divide all values by the maximum value
            double max = hist3D.Cast<double>().Max();
            hist3D = ConvertAll(hist3D, item => item / max);

            // Save the 3D histogram to the file "test.txt"
            WriteOut(hist3D, @"test.txt");
            MessageBox.Show("Three dimensional histogram created");

            // Show the original image
            showImage(origImg.ToBitmap());
        }

        // Writes a 2 dimensional array to a file
        private static void WriteOut(double[,] vals, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine(vals.GetLength(0));
            writer.WriteLine(vals.GetLength(1));
            for (int i = 0; i < vals.GetLength(0); i++)
            {
                for (int j = 0; j < vals.GetLength(1); j++)
                {
                    writer.WriteLine(vals[i, j]);
                }
            }
            writer.Close();
        }

        // Reads a 2 dimensional array from a file
        private static double[,] ReadIn(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            int w;
            int h;
            int.TryParse(reader.ReadLine(), out w);
            int.TryParse(reader.ReadLine(), out h);
            double[,] readValues = new double[w, h];
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

        // Detects the cropped marker saved in "test.txt"
        private void marker_Detection(object sender, EventArgs e)
        {
            // Convert the captured webcam image from RGB to HSV space
            Image<Bgr, Byte> origImg = capture.RetrieveBgrFrame();
            Image<Bgr, Byte> smoothImg = origImg.SmoothGaussian(gs1, gs2, gs3, gs4);
            Image<Hsv, Byte> hsvImg = smoothImg.Convert<Hsv, Byte>();

            // 3D histogram
            double[,] hist3D = ReadIn(@"test.txt");
            double threshold = 0.3; // Greater than threshold
            double[] histX = new double[hsvImg.Width];
            double[] histY = new double[hsvImg.Height];
            int scanx1, scanx2, scany1, scany2;

            // Loop if the marker is detected
            if (markerdetected)
            {
                scanx1 = cx - ((cx - x1) * 3);
                scanx2 = cx + ((cx - x1) * 3);
                scany1 = cy - ((cy - y1) * 3);
                scany2 = cy + ((cy - y1) * 3);

                for (int x = scanx1; x >= scanx1  && x <= scanx2; x++)
                {
                    for (int y = scany1; y >= scanx1 && y <= scanx2; y++)
                    {
                        double h = Math.Round(hsvImg.Data[y, x, 0] / 255d, 1);
                        h = (h == 0) ? 0 : ((h - 0.1) * 10d);

                        double s = Math.Round(hsvImg.Data[y, x, 1] / 255d, 1);
                        s = (s == 0) ? 0 : ((s - 0.1) * 10d);

                        if (hist3D[(int)h, (int)s] > threshold)
                        {
                            histX[x] += hist3D[(int)h, (int)s];
                            histY[y] += hist3D[(int)h, (int)s];
                        }
                    }
                }
            }
            // Loop if the marker is not detected yet
            else
            {
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
                            markerdetected = true;
                            histX[x] += hist3D[(int)h, (int)s];
                            histY[y] += hist3D[(int)h, (int)s];
                        }
                        else
                            markerdetected = false;
                    }
                }
            }

            // Find out where the marker starts and ends
            x1 = Array.FindIndex(histX, item => item > 0);
            y1 = Array.FindIndex(histY, item => item > 0);
            x2 = Array.FindLastIndex(histX, item => item > 0);
            y2 = Array.FindLastIndex(histY, item => item > 0);

            // Find the center of the marker
            cx = ((x2 - x1) / 2 + x1);
            cy = ((y2 - y1) / 2 + y1);

            // Draw the marker and the image
            origImg.Draw(new Ellipse(new PointF(cx, cy), new SizeF((y2 - y1), (x2 - x1)), 360f), new Bgr(0, 0, 0), -1);
            showImage(origImg.ToBitmap());
        }

        // Calculates the value for a var in a histogram
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