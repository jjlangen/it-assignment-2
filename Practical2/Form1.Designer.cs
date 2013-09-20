namespace CamCapture
{
    partial class CamCapture
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
            this.CamImageBox = new Emgu.CV.UI.ImageBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnOpenSave = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CamImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CamImageBox
            // 
            this.CamImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CamImageBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.CamImageBox.Location = new System.Drawing.Point(12, 12);
            this.CamImageBox.Name = "CamImageBox";
            this.CamImageBox.Size = new System.Drawing.Size(490, 328);
            this.CamImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.CamImageBox.TabIndex = 2;
            this.CamImageBox.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(427, 346);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Play";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnOpenSave
            // 
            this.btnOpenSave.Location = new System.Drawing.Point(332, 346);
            this.btnOpenSave.Name = "btnOpenSave";
            this.btnOpenSave.Size = new System.Drawing.Size(75, 23);
            this.btnOpenSave.TabIndex = 4;
            this.btnOpenSave.Text = "Open";
            this.btnOpenSave.UseVisualStyleBackColor = true;
            this.btnOpenSave.Click += new System.EventHandler(this.btnOpenSave_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Enabled = false;
            this.btnRecord.Location = new System.Drawing.Point(231, 346);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(75, 23);
            this.btnRecord.TabIndex = 5;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // CamCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 377);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnOpenSave);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.CamImageBox);
            this.Name = "CamCapture";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.CamImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox CamImageBox;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOpenSave;
        private System.Windows.Forms.Button btnRecord;
    }
}

