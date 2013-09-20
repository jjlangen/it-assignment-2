using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.GPU;
using Emgu.CV.Util;
using System.Threading;

namespace CamCapture
{
    public partial class CamCapture : Form
    {
        //declaring global variables
        private Capture capture = new Capture(); //takes images from camera as image frames
        OpenFileDialog openFile = new OpenFileDialog();
        SaveFileDialog saveFile = new SaveFileDialog();
        private bool captureInProgress = false; // checks if capture is executing        
        bool play = false; bool record = false;
        const int fps = 20;
        int videoNr;
        VideoWriter writer;
        Image<Bgr, Byte> frame;
        BackgroundWorker bgworker = new BackgroundWorker();
        

        public CamCapture()
        {
            InitializeComponent();
            bgworker.DoWork += bgworker_DoWork;
        }


        private void ProcessFrame(object sender, EventArgs arg)
        {
            frame = capture.QueryFrame();
            CamImageBox.Image = frame;
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            captureInProgress = !captureInProgress;
            if (!captureInProgress)
            {  //if camera is getting frames then stop the capture and set button Text
                // "Start" for resuming capture
                btnStart.Text = "Play";
                btnRecord.Enabled = false;
                Application.Idle -= ProcessFrame;
            }
            else
            {
                //if camera is NOT getting frames then start the capture and set button
                // Text to "Stop" for pausing capture
                btnStart.Text = "Pause";
                btnRecord.Enabled = true;
                Application.Idle += ProcessFrame;
            }            
        }



        private void btnOpenSave_Click(object sender, EventArgs e)
        {

        }

        // Start or stops the recording process
        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (record)
            {
                record = false;
                /*
                saveFile.Filter = "Avi File | *.avi";
                
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Move("vid.avi", saveFile.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@ex.ToString() + "\n" + saveFile.FileName, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                 */
                MessageBox.Show("Video was saved succesfully to vid" + videoNr + ".avi", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnRecord.Text = "Start Record";
            }
            else
            {
                record = true;
                btnRecord.Text = "Stop Record";
                bgworker.RunWorkerAsync();
            }
        }

        // Backgroundworker tasked to record outside of the main thread
        private void bgworker_DoWork(object s, DoWorkEventArgs e)
        {
            startRecording();
        }

        // Method which writes the frames from the videostream to a Videowriter object
        private void startRecording()
        {
            fileSaving();
            while (record)
            {
                writer.WriteFrame(frame);
                Thread.Sleep(1000 / fps);
            }
            // Release the Videowriter object so the file can be opened
            writer.Dispose();
        }

        // This method finds a an unused filename to save the recording to: vid1.avi, vid2.avi, vid3.avi etc.
        private void fileSaving()
        {
            videoNr = 1;
            for (int i = 1; i < 100 && File.Exists("vid" + i + ".avi"); i++)
            {
                videoNr++;
            }
            writer = new VideoWriter("vid" + videoNr + ".avi", fps, 640, 480, true);
        }
    }
}
