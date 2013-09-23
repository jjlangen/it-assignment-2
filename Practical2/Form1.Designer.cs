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
            this.btnReset = new System.Windows.Forms.Button();
            this.checkJanKlaassen = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Location = new System.Drawing.Point(16, 672);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(129, 28);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Play";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnTracking
            // 
            this.btnTracking.Location = new System.Drawing.Point(580, 672);
            this.btnTracking.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTracking.Name = "btnTracking";
            this.btnTracking.Size = new System.Drawing.Size(131, 28);
            this.btnTracking.TabIndex = 1;
            this.btnTracking.Text = "Marker Tracking";
            this.btnTracking.UseVisualStyleBackColor = true;
            this.btnTracking.Click += new System.EventHandler(this.btnTracking_Click);
            // 
            // btnFaceDetection
            // 
            this.btnFaceDetection.Location = new System.Drawing.Point(731, 672);
            this.btnFaceDetection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFaceDetection.Name = "btnFaceDetection";
            this.btnFaceDetection.Size = new System.Drawing.Size(120, 28);
            this.btnFaceDetection.TabIndex = 2;
            this.btnFaceDetection.Text = "Face Detection";
            this.btnFaceDetection.UseVisualStyleBackColor = true;
            this.btnFaceDetection.Click += new System.EventHandler(this.btnFaceDetection_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(872, 672);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(119, 28);
            this.btnScan.TabIndex = 4;
            this.btnScan.Text = "Start Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // imageBox
            // 
            this.imageBox.Location = new System.Drawing.Point(17, 16);
            this.imageBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(973, 606);
            this.imageBox.TabIndex = 6;
            this.imageBox.TabStop = false;
            // 
            // checkRecord
            // 
            this.checkRecord.AutoSize = true;
            this.checkRecord.Location = new System.Drawing.Point(16, 629);
            this.checkRecord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkRecord.Name = "checkRecord";
            this.checkRecord.Size = new System.Drawing.Size(116, 21);
            this.checkRecord.TabIndex = 7;
            this.checkRecord.Text = "Record Video";
            this.checkRecord.UseVisualStyleBackColor = true;
            this.checkRecord.CheckedChanged += new System.EventHandler(this.checkRecord_CheckedChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(192, 672);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(132, 28);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(396, 672);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(123, 28);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // checkBox1
            // 
            this.checkJanKlaassen.AutoSize = true;
            this.checkJanKlaassen.Location = new System.Drawing.Point(580, 629);
            this.checkJanKlaassen.Name = "checkBox1";
            this.checkJanKlaassen.Size = new System.Drawing.Size(113, 21);
            this.checkJanKlaassen.TabIndex = 10;
            this.checkJanKlaassen.Text = "Hand Puppet";
            this.checkJanKlaassen.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 715);
            this.Controls.Add(this.checkJanKlaassen);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.checkRecord);
            this.Controls.Add(this.imageBox);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnFaceDetection);
            this.Controls.Add(this.btnTracking);
            this.Controls.Add(this.btnStart);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox checkJanKlaassen;
    }
}

