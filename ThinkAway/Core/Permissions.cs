using System;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace ThinkAway.Core
{
    public class Permissions
    {

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        public static void SetPrivileges(bool enabled)
        {
            SetPrivilege("SeShutdownPrivilege", enabled);
            SetPrivilege("SeBackupPrivilege", enabled);
            SetPrivilege("SeRestorePrivilege", enabled);
            SetPrivilege("SeDebugPrivilege", enabled);
        }

        public static bool SetPrivilege(string privilege, bool enabled)
        {
            try
            {
                TokPriv1Luid tp = new TokPriv1Luid();
                IntPtr hproc = System.Diagnostics.Process.GetCurrentProcess().Handle;
                IntPtr htok = IntPtr.Zero;

                if (!Win32API.OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
                    return false;

                if (!Win32API.LookupPrivilegeValue(null, privilege, ref tp.Luid))
                    return false;

                tp.Count = 1;
                tp.Luid = 0;
                tp.Attr = ((enabled) ? (SE_PRIVILEGE_ENABLED) : (0));

                Win32API.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                if (Marshal.GetLastWin32Error() != 0)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the user is an admin
        /// </summary>
        /// <returns>True if it is in admin group</returns>
        public static bool IsUserAdministrator
        {
            get
            {
                //bool value to hold our return value
                bool isAdmin;
                try
                {
                    //get the currently logged in user
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(user);
                    isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                catch (UnauthorizedAccessException)
                {
                    isAdmin = false;
                }
                catch (Exception)
                {
                    isAdmin = false;
                }
                return isAdmin;
            }
        }
    }
}
