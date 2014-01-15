using System;

/*LsongStudio
 *song940@gmail.com 
 *http://lsong.org 
 */

namespace ThinkAway.Core.SystemMenu
{
    internal class NativeMethods
    {
        public const int WM_SYSCOMMAND = 0x0112;

        public static readonly IntPtr TRUE = new IntPtr(1);
    }
}