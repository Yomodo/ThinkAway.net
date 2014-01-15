using System;

namespace ThinkAway.Core
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
                   Win32API.DwmIsCompositionEnabled(out flag);
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

