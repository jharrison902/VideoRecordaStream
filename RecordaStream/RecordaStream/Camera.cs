using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Camera
{
    /// <summary>
    /// Summary description for UserControl1.
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(cameraControl), "CAMERA.ICO")] // toolbox bitmap
    [Designer("Sytem.Windows.Forms.Design.ParentControlDesigner,System.Design", typeof(System.ComponentModel.Design.IDesigner))] // make composite
    public class cameraControl : System.Windows.Forms.UserControl
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Timer timer1;

        // property variables
        private int m_TimeToCapture_milliseconds = 1;
        private int m_Width = 500;
        private int m_Height = 500;
        private int mCapHwnd;
        private ulong m_FrameNumber = 0;
        public int device_number = 0;

        private bool running = false;

        // global variables to make the video capture go faster
        private Camera.CameraArgs x = new Camera.CameraArgs();
        private IDataObject tempObj;
        private System.Drawing.Image tempImg;
        private bool bStopped = true;

        // event delegate
        public delegate void CameraHandler(object source, Camera.CameraArgs e);
        // fired when a new image is captured
        public event System.EventHandler<CameraArgs> ImageCaptured;

        #region API Declarations

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(int hWnd, int hWndPos, int posx, int posy, int width, int height, int wFlags);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();

        #endregion

        #region API Constants

        public const int WM_USER = 1024;

        public const int WM_CAP_CONNECT = 1034;
        public const int WM_CAP_DISCONNECT = 1035;
        public const int WM_CAP_GET_FRAME = 1084;
        public const int WM_CAP_COPY = 1054;

        public const int WM_CAP_START = WM_USER;

        public const int WM_CAP_SET_SCALE = WM_CAP_START + 53;
        public const int WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;
        public const int WM_CAP_PAL_PASTE = WM_CAP_START + 82;

        #endregion

        #region NOTES

        /*
		 * If you want to allow the user to change the display size and 
		 * color format of the video capture, call:
		 * SendMessage (mCapHwnd, WM_CAP_DLG_VIDEOFORMAT, 0, 0);
		 * You will need to requery the capture device to get the new settings
		*/

        #endregion


        public cameraControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Override the class's finalize method, so we can stop
        /// the video capture on exit
        /// </summary>
        ~cameraControl()
        {
            this.Stop();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WebCamCapture
            // 
            this.Name = "WebCamCapture";
            this.Size = new System.Drawing.Size(512, 512);

        }
        #endregion

        #region Control Properties

        /// <summary>
        /// The time intervale between frame captures
        /// </summary>
        public int TimeToCapture_milliseconds
        {
            get
            { return m_TimeToCapture_milliseconds; }

            set
            { m_TimeToCapture_milliseconds = value; }
        }

        /// <summary>
        /// The height of the video capture image
        /// </summary>
        public int CaptureHeight
        {
            get
            { return m_Height; }

            set
            { m_Height = value; }
        }

        /// <summary>
        /// The width of the video capture image
        /// </summary>
        public int CaptureWidth
        {
            get
            { return m_Width; }

            set
            { m_Width = value; }
        }

        /// <summary>
        /// The sequence number to start at for the frame number. OPTIONAL
        /// </summary>
        public ulong FrameNumber
        {
            get
            { return m_FrameNumber; }

            set
            { m_FrameNumber = value; }
        }

        public bool isRunning
        {
            get
            {
                return running;
            }
        }

        #endregion

        #region Start and Stop Capture Functions

        /// <summary>
        /// Starts the video capture
        /// </summary>
        /// <param name="FrameNumber">the frame number to start at. 
        /// Set to 0 to let the control allocate the frame number</param>
        public void Start(ulong FrameNum, bool showDialog)
        {
            try
            {
                // for safety, call stop, just in case we are already running
                this.Stop();

                // setup a capture window
                mCapHwnd = capCreateCaptureWindowA("WebCap", 0, 0, 0, m_Width, m_Height, this.Handle.ToInt32(), 0);

                //need to try changing the 0, 0);'s to different numbers to change the camera driver
                // connect to the capture device
                Application.DoEvents();
                int s1 = SendMessage(mCapHwnd, WM_CAP_CONNECT, device_number, 0);
                int s2 = SendMessage(mCapHwnd, WM_CAP_SET_PREVIEW, 0, 0);
                while (s1 != 1 && s2 != 1)
                {
                    device_number += 1;
                    s1 = SendMessage(mCapHwnd, WM_CAP_CONNECT, device_number, 0);
                    s2 = SendMessage(mCapHwnd, WM_CAP_SET_PREVIEW, 0, 0);
                    if (device_number >= 10)
                    {
                        break;
                    }
                }
                if (showDialog)
                {
                    SendMessage(mCapHwnd, WM_CAP_DLG_VIDEOFORMAT, 0, 0);
                }

                // set the frame number
                m_FrameNumber = FrameNum;

                // set the timer information
                this.timer1.Interval = m_TimeToCapture_milliseconds;
                bStopped = false;
                this.timer1.Start();
                running = true;
            }

            catch (Exception excep)
            {
                MessageBox.Show("Make sure your webcamera is connected and/or turned on.  Error Message: " + excep.Message);
                this.Stop();
            }
        }

        /// <summary>
        /// Stops the video capture
        /// </summary>
        public void Stop()
        {
            try
            {
                // stop the timer
                bStopped = true;
                this.timer1.Stop();

                // disconnect from the video source
                Application.DoEvents();
                SendMessage(mCapHwnd, WM_CAP_DISCONNECT, device_number, 0);
                running = false;
            }

            catch (Exception excep)
            {
                Console.WriteLine(excep.Message);
            }

        }

        #endregion

        public Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(size.Width);
            int destHeight = (int)(size.Height);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            b.SetResolution(16000f, 12800f);
            return (Image)b;
        }

        #region Video Capture Code

        /// <summary>
        /// Capture the next frame from the video feed
        /// </summary>
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            try
            {
                // pause the timer
                this.timer1.Stop();

                // get the next frame;
                SendMessage(mCapHwnd, WM_CAP_GET_FRAME, 0, 0);

                // copy the frame to the clipboard
                SendMessage(mCapHwnd, WM_CAP_COPY, 0, 0);

                // paste the frame into the event args image
                if (ImageCaptured != null)
                {
                    // get from the clipboard
                    tempObj = Clipboard.GetDataObject();
                    tempImg = (System.Drawing.Bitmap)tempObj.GetData(System.Windows.Forms.DataFormats.Bitmap);
//                    SendMessage(mCapHwnd, WM_CAP_PAL_PASTE, tempImg.Palette, 0);
  //                  tempImg.Palette = (System.Drawing.Imaging.ColorPalette)Clipboard.GetData(System.Windows.Forms.DataFormats.Palette);
                    GC.Collect();
                    /*
                    * For some reason, the API is not resizing the video
                    * feed to the width and height provided when the video
                    * feed was started, so we must resize the image here
                    */
                    //IT'S COPYING TO THE CLIPBOARD!!!!!!  AS IN CTRL+C CLIPBOARD
//                    Console.WriteLine("{0} {1} {2}", Clipboard.ContainsData(DataFormats.Bitmap), Clipboard.ContainsText(), Clipboard.ContainsFileDropList());
                    x.WebCamImage = tempImg;
                    // raise the event
                    this.ImageCaptured(this, x);
                }

                // restart the timer
                Application.DoEvents();
                if (!bStopped)
                    this.timer1.Start();
            }

            catch (Exception excep)
            {
                MessageBox.Show("There was an error while capturing the video image, stopping video.  Error Message: " + excep.Message);
                this.Stop(); // stop the process
            }
        }

        #endregion
    }
}
