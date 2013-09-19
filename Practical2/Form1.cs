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
using Emgu.CV.UI;
using Emgu.CV.GPU;
using Emgu.CV.Util;

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
        VideoWriter writer;
        Image<Bgr, Byte> frame;

        public CamCapture()
        {
            InitializeComponent();
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

        private void btnRecord_Click(object sender, EventArgs e)
        {
            btnRecord.Text = "Stop Record";
            startRecording();
        }

        private void startRecording()
        {

        }
    }
}
