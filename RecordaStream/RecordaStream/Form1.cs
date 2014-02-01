using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace RecordaStream
{
    public partial class Form1 : Form
    {
        BufferedWaveProvider bwp;
        WaveIn wi;
        WaveOut wo;

        public Form1()
        {
            InitializeComponent();

            cameraControl1.Size = new System.Drawing.Size(640, 480);
            cameraControl1.Start(0, true);

            wo = new WaveOut();
            wi = new WaveIn();

            wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);

            bwp = new BufferedWaveProvider(wi.WaveFormat);
            bwp.DiscardOnBufferOverflow = true;

            wo.Init(bwp);
            wi.StartRecording();
            wo.Play();
        }

        void wi_DataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void cameraControl1_ImageCaptured(object sender, Camera.CameraArgs e)
        {
            pictureBox1.Image = e.WebCamImage;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            wi.StopRecording();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by: Gerald McAlister and Jesse Harrison", "About");
        }

        private void stopCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cameraControl1.isRunning)
            {
                cameraControl1.Stop();
                stopCameraToolStripMenuItem.Text = "Start Camera";
            }
            else if (!cameraControl1.isRunning)
            {
                cameraControl1.Start(0, false);
                stopCameraToolStripMenuItem.Text = "Stop Camera";
            }
        }

        private void stopAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wo.PlaybackState == PlaybackState.Playing)
            {
                wo.Stop();
                stopAudioToolStripMenuItem.Text = "Start Audio";
            }
            else if (wo.PlaybackState == PlaybackState.Stopped)
            {
                wo.Play();
                stopAudioToolStripMenuItem.Text = "Stop Audio";
            }
        }
    }
}
