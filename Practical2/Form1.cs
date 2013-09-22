using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

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
        Size size1 = new System.Drawing.Size(30, 30);
        Size size2 = new System.Drawing.Size(700, 500);

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
            capture = new Capture();
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
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFile = new OpenFileDialog();
            openFile.Filter = "Video Files|*.avi;*.mp4;*.mpg";
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
                    this.Text = "Viewing Video: " + openFile.FileName;
                    state = VideoState.Viewing;

                    capture = new Capture(openFile.FileName);
                    capture.ImageGrabbed += Update;

                    btnStart.Text = "Play";
                    btnStart.ForeColor = Color.Green;
                    play = false;
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += marker_Detection;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void btnFaceDetection_Click(object sender, EventArgs e)
        {

            //haarCascade = new HaarCascade("C:/Users/paktw_000/Documents/Uni/4e Jaar/Interactie Technologie/practical2/Recent Werkend/Practical2/haarcascade_frontalface_default.xml");
            cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
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
                    location = face.Location;
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

        private void marker_Detection(object sender, EventArgs e)
        {
            // Retrieve en blur het plaatje
            Image<Bgr, Byte> smoothImg = capture.RetrieveBgrFrame().SmoothGaussian(5, 5, 1.5, 1.5);

            // Plaatje omzetten in HSV en vervolgens splitten naar H, S en V
            Image<Hsv, Byte> imgHSV = smoothImg.Convert<Hsv, Byte>();
            Image<Gray, Byte>[] channels = imgHSV.Split();

            // Hue in range 0-30 en saturation in range 200-255 filteren
            Image<Gray, Byte> hue = channels[0].InRange(new Gray(0), new Gray(30));
            Image<Gray, Byte> saturation = channels[1].InRange(new Gray(200), new Gray(255));

            // Samenvoegen tot nieuw plaatje
            Image<Gray, Byte> img = (hue.And(saturation));
            showImage(img.ToBitmap());



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