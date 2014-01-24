using System;
using System.Drawing;
using System.Globalization;
using ThinkAway.Core;

namespace ThinkAway.IO.Camera
{
    /// <summary>
    /// Leon's webcam mirror
    /// </summary>
    public class WebCamera
    {
        /// <summary>
        /// Webcam handle. 
        /// </summary>
        private int _hHwnd;
        
        public struct VideohdrTag
        {
            public byte[] lpData;
            public int dwBufferLength;
            public int dwBytesUsed;
            public int dwTimeCaptured;
            public int dwUser;
            public int dwFlags;
            public int[] dwReserved;
        }


        /// <summary>
        /// Initialize webcam and display the video in a panel.
        /// </summary>
        /// <returns></returns>
        private bool InitializeWebcam(IntPtr handle,Size size)
        {
            bool ok = false;

            int intWidth = size.Width;
            int intHeight = size.Height;
            const int intDevice = 0;
            string refDevice = intDevice.ToString(CultureInfo.InvariantCulture);

            //Create vedio and get the window handle. 
            _hHwnd = Win32API.capCreateCaptureWindowA(ref refDevice, 0x50000000, 0, 0, 640, 480, handle.ToInt32(), 0);

            if (Win32API.SendMessage(_hHwnd, 0x40a, intDevice, 0) > 0)
            {
                Win32API.SendMessage(this._hHwnd, 0x435, -1, 0);
                Win32API.SendMessage(this._hHwnd, 0x434, 0x42, 0);
                Win32API.SendMessage(this._hHwnd, 0x432, -1, 0);
                Win32API.SetWindowPos(this._hHwnd, 1, 0, 0, intWidth, intHeight, 6);

                ok = true;
            }
            else
            {
                Win32API.DestroyWindow(this._hHwnd);
            }

            return ok;
        }

        /// <summary>
        /// App run, then invoke the webcam till successfully.
        /// </summary>
        public void OpenWebcam(IntPtr handle, Size size)
        {
            bool ok = false;

            while (!ok)
            {
                ok = this.InitializeWebcam(handle, size);
                System.Threading.Thread.Sleep(100);
            }
        }
        /// <summary>
        /// when close window, destroy the webcam window.
        /// </summary>
        public void CloseWebcam()
        {
            if (this._hHwnd > 0)
            {
                Win32API.SendMessage(this._hHwnd, 0x40b, 0, 0);
                Win32API.DestroyWindow(this._hHwnd);
            }
        }

        /// <summary>
        /// when window size changed, resize webcam pic.
        /// </summary>
        public void ChangedSize(Size size)
        {
            Win32API.SetWindowPos(this._hHwnd, 1, 0, 0, size.Width, size.Height, 6);
        }
    }
}