using System;
using ThinkAway.Core;

namespace ThinkAway.Controls.Dwm
{
    public static class OsSupport
    {
        private const int VistaMajorVersion = 6;

        public static bool IsCompositionEnabled
        {
            get
            {
                try
                {
                    bool flag;
                    int dwmIsCompositionEnabled = Win32API.DwmIsCompositionEnabled(out flag);
                    return flag;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool IsVistaOrBetter
        {
            get
            {
                return ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= VistaMajorVersion));
            }
        }
    }
}

