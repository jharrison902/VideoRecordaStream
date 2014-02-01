namespace RecordaStream
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
            this.cameraControl1 = new Camera.cameraControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraControl1
            // 
            this.cameraControl1.CaptureHeight = 500;
            this.cameraControl1.CaptureWidth = 500;
            this.cameraControl1.FrameNumber = ((ulong)(0ul));
            this.cameraControl1.Location = new System.Drawing.Point(0, 0);
            this.cameraControl1.Name = "WebCamCapture";
            this.cameraControl1.Size = new System.Drawing.Size(512, 512);
            this.cameraControl1.TabIndex = 0;
            this.cameraControl1.TimeToCapture_milliseconds = 1;
            this.cameraControl1.ImageCaptured += new System.EventHandler<Camera.CameraArgs>(this.cameraControl1_ImageCaptured);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(599, 416);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "RecrodaStream";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Camera.cameraControl cameraControl1;
        private System.Windows.Forms.PictureBox pictureBox1;

    }
}

