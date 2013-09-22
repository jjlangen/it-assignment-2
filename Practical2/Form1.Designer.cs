namespace Practical2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnTracking = new System.Windows.Forms.Button();
            this.btnFaceDetection = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.checkRecord = new System.Windows.Forms.CheckBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Location = new System.Drawing.Point(12, 546);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(97, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Play";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnTracking
            // 
            this.btnTracking.Location = new System.Drawing.Point(435, 546);
            this.btnTracking.Name = "btnTracking";
            this.btnTracking.Size = new System.Drawing.Size(98, 23);
            this.btnTracking.TabIndex = 1;
            this.btnTracking.Text = "Marker Tracking";
            this.btnTracking.UseVisualStyleBackColor = true;
            this.btnTracking.Click += new System.EventHandler(this.btnTracking_Click);
            // 
            // btnFaceDetection
            // 
            this.btnFaceDetection.Location = new System.Drawing.Point(548, 546);
            this.btnFaceDetection.Name = "btnFaceDetection";
            this.btnFaceDetection.Size = new System.Drawing.Size(90, 23);
            this.btnFaceDetection.TabIndex = 2;
            this.btnFaceDetection.Text = "Face Detection";
            this.btnFaceDetection.UseVisualStyleBackColor = true;
            this.btnFaceDetection.Click += new System.EventHandler(this.btnFaceDetection_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(654, 546);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(89, 23);
            this.btnScan.TabIndex = 4;
            this.btnScan.Text = "Start Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            // 
            // imageBox
            // 
            this.imageBox.Location = new System.Drawing.Point(13, 13);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(730, 492);
            this.imageBox.TabIndex = 6;
            this.imageBox.TabStop = false;
            // 
            // checkRecord
            // 
            this.checkRecord.AutoSize = true;
            this.checkRecord.Location = new System.Drawing.Point(12, 511);
            this.checkRecord.Name = "checkRecord";
            this.checkRecord.Size = new System.Drawing.Size(91, 17);
            this.checkRecord.TabIndex = 7;
            this.checkRecord.Text = "Record Video";
            this.checkRecord.UseVisualStyleBackColor = true;
            this.checkRecord.CheckedChanged += new System.EventHandler(this.checkRecord_CheckedChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(144, 546);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(99, 23);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(435, 512);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(35, 13);
            this.lbl.TabIndex = 9;
            this.lbl.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 581);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.checkRecord);
            this.Controls.Add(this.imageBox);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnFaceDetection);
            this.Controls.Add(this.btnTracking);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnTracking;
        private System.Windows.Forms.Button btnFaceDetection;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.PictureBox imageBox;
        private System.Windows.Forms.CheckBox checkRecord;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label lbl;
    }
}

