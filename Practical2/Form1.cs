using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Practical2
{
    public partial class Form1 : Form
    {
        private Capture capture;
        SaveFileDialog saveFile;
        OpenFileDialog openFile;
        VideoWriter writer;
        int frameWidth, frameHeight;
        double frameRate = 10;

        bool play = false;
        bool record = false;

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
                Image<Bgr, byte> img = capture.RetrieveBgrFrame();
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

                        //frameRate = 5;

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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (capture != null)
            {
                if (capture.GrabProcessState == System.Threading.ThreadState.Running) capture.Stop();
                capture.Dispose();
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

    }
}