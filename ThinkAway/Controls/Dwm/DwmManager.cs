using System;
using System.Windows.Forms;
using ThinkAway.Core;

namespace ThinkAway.Controls.Dwm
{
    public static class DwmManager
    {
        public static void DisableBlurBehind(IntPtr hWnd)
        {
            BlurBehind pBlurBehind = new BlurBehind();
            pBlurBehind.dwFlags = BlurBehindFlags.Enable;
            pBlurBehind.fEnable = false;
            Win32API.DwmEnableBlurBehindWindow(hWnd, ref pBlurBehind);
        }

        public static void DisableGlassFrame(IntPtr hWnd)
        {
            InternalGlassFrame(hWnd, new Margins(0));
        }

        public static void DisableGlassFrame(Form window)
        {
            InternalGlassFrame(window.Handle, new Margins(0));
        }

        public static void EnableBlurBehind(IntPtr hWnd)
        {
            BlurBehind pBlurBehind = new BlurBehind();
            pBlurBehind.dwFlags = BlurBehindFlags.Enable;
            pBlurBehind.fEnable = true;
            pBlurBehind.hRgnBlur = IntPtr.Zero;
            Win32API.DwmEnableBlurBehindWindow(hWnd, ref pBlurBehind);
        }

        public static void EnableBlurBehind(Form form)
        {
            EnableBlurBehind(form.Handle);
        }

        public static void EnableBlurBehind(IntPtr hWnd, IntPtr regionHandle)
        {
            BlurBehind pBlurBehind = new BlurBehind();
            pBlurBehind.dwFlags = BlurBehindFlags.BlurRegion | BlurBehindFlags.Enable;
            pBlurBehind.fEnable = true;
            pBlurBehind.hRgnBlur = regionHandle;
            Win32API.DwmEnableBlurBehindWindow(hWnd, ref pBlurBehind);
        }

        public static void EnableGlassFrame(IntPtr hWnd, Margins margins)
        {
            InternalGlassFrame(hWnd, margins);
        }

        public static void EnableGlassFrame(Form window, Margins margins)
        {
            InternalGlassFrame(window.Handle, margins);
        }

        public static void EnableGlassSheet(IntPtr hWnd)
        {
            InternalGlassFrame(hWnd, new Margins(-1));
        }

        public static void EnableGlassSheet(Form window)
        {
            InternalGlassFrame(window.Handle, new Margins(-1));
        }

        private static void InternalGlassFrame(IntPtr hWnd, Margins margins)
        {
            if (Win32API.DwmExtendFrameIntoClientArea(hWnd, ref margins) != 0)
            {
                throw new DwmCompositionException(string.Format("NativeCallFailure:{0}", "DwmExtendFrameIntoClientArea"));
            }
        }

        public static Thumbnail Register(IntPtr destination, IntPtr source)
        {
            if (!OsSupport.IsVistaOrBetter)
            {
                throw new DwmCompositionException("DWMOsNotSupported");
            }
            if (!OsSupport.IsCompositionEnabled)
            {
                throw new DwmCompositionException("DWMNotEnabled");
            }
            if (destination == source)
            {
                throw new DwmCompositionException("DWMWindowMatch");
            }
            Thumbnail phThumbnailId;
            if (Win32API.DwmRegisterThumbnail(destination, source, out phThumbnailId) != 0)
            {
                throw new DwmCompositionException(string.Format("NativeCallFailure:{0}", "DwmRegisterThumbnail"));
            }
            return phThumbnailId;
        }

        public static Thumbnail Register(Form destination, IntPtr source)
        {
            return Register(destination.Handle, source);
        }

        public static void Unregister(Thumbnail handle)
        {
            if ((handle != null) && !handle.IsInvalid)
            {
                handle.Close();
            }
        }
    }
}

