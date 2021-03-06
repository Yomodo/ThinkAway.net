using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using ThinkAway.Controls.Dwm;
using ThinkAway.IO.Camera;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core
{
    /// <summary>
    /// Windows API 函数声明
    /// </summary>
    public class Win32API
    {
        #region .ctor()
        // No need to construct this object
        private Win32API()
        {
        }
        #endregion

        #region Constans values

        public const string TOOLBARCLASSNAME = "ToolbarWindow32";
        public const string REBARCLASSNAME = "ReBarWindow32";
        public const string PROGRESSBARCLASSNAME = "msctls_progress32";
        public const string SCROLLBAR = "SCROLLBAR";

        #endregion

        #region CallBacks

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr TaskDialogCallback(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam, IntPtr refData);

        #endregion

        #region Kernel32.dll functions

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetCurrentThreadId();


        [DllImport("kernel32.dll")]
        public static extern int lstrlenA(string InString);

        #endregion

        #region Gdi32.dll functions

        [DllImport("gdi32.dll")]
        static public extern bool StretchBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
        IntPtr hDCSrc, int XOriginScr, int YOriginSrc, int WidthScr, int HeightScr, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Heigth);
        [DllImport("gdi32.dll")]
        static public extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern bool BitBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
        IntPtr hDCSrc, int XOriginScr, int YOriginSrc, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern IntPtr DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        static public extern bool PatBlt(IntPtr hDC, int XLeft, int YLeft, int Width, int Height, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
        [DllImport("gdi32.dll")]
        static public extern int SetMapMode(IntPtr hDC, int fnMapMode);
        [DllImport("gdi32.dll")]
        static public extern int GetObjectType(IntPtr handle);
        [DllImport("gdi32")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO_FLAT bmi,
        int iUsage, ref int ppvBits, IntPtr hSection, int dwOffset);
        [DllImport("gdi32")]
        public static extern int GetDIBits(IntPtr hDC, IntPtr hbm, int StartScan, int ScanLines, int lpBits, BITMAPINFOHEADER bmi, int usage);
        [DllImport("gdi32")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int StartScan, int ScanLines, int lpBits, ref BITMAPINFO_FLAT bmi, int usage);
        [DllImport("gdi32")]
        public static extern IntPtr GetPaletteEntries(IntPtr hpal, int iStartIndex, int nEntries, byte[] lppe);
        [DllImport("gdi32")]
        public static extern IntPtr GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);
        [DllImport("gdi32")]
        public static extern uint SetDCBrushColor(IntPtr hdc, uint crColor);
        [DllImport("gdi32")]
        public static extern IntPtr CreateSolidBrush(uint crColor);
        [DllImport("gdi32")]
        public static extern int SetBkMode(IntPtr hDC, BackgroundMode mode);
        [DllImport("gdi32")]
        public static extern int SetViewportOrgEx(IntPtr hdc, int x, int y, int param);
        [DllImport("gdi32")]
        public static extern uint SetTextColor(IntPtr hDC, uint colorRef);
        [DllImport("gdi32")]
        public static extern int SetStretchBltMode(IntPtr hDC, int StrechMode);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, BitBltOp dwRop);
 
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(int crColor);

        #endregion

        #region Uxtheme.dll functions

        [DllImport("uxtheme.dll")]
        static public extern int SetWindowTheme(IntPtr hWnd, string AppID, string ClassID);



        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);

        /// <summary>
        /// Tests if a visual style for the current application is active
        /// </summary>
        /// <returns>TRUE if a visual style is enabled, and windows with 
        /// visual styles applied should call OpenThemeData to start using 
        /// theme drawing services, FALSE otherwise</returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool IsThemeActive();


        /// <summary>
        /// Reports whether the current application's user interface 
        /// displays using visual styles
        /// </summary>
        /// <returns>TRUE if the application has a visual style applied,
        /// FALSE otherwise</returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool IsAppThemed();


        /// <summary>
        /// Opens the theme data for a window and its associated class
        /// </summary>
        /// <param name="hwnd">Handle of the window for which theme data 
        /// is required</param>
        /// <param name="pszClassList">Pointer to a string that contains 
        /// a semicolon-separated list of classes</param>
        /// <returns>OpenThemeData tries to match each class, one at a 
        /// time, to a class data section in the active theme. If a match 
        /// is found, an associated HTHEME handle is returned. If no match 
        /// is found NULL is returned</returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPTStr)] string pszClassList);


        /// <summary>
        /// Closes the theme data handle
        /// </summary>
        /// <param name="hTheme">Handle to a window's specified theme data. 
        /// Use OpenThemeData to create an HTHEME</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int CloseThemeData(IntPtr hTheme);


        /// <summary>
        /// Draws the background image defined by the visual style for the 
        /// specified control part
        /// </summary>
        /// <param name="hTheme">Handle to a window's specified theme data. 
        /// Use OpenThemeData to create an HTHEME</param>
        /// <param name="hdc">Handle to a device context (HDC) used for 
        /// drawing the theme-defined background image</param>
        /// <param name="iPartId">Value of type int that specifies the part 
        /// to draw</param>
        /// <param name="iStateId">Value of type int that specifies the state 
        /// of the part to draw</param>
        /// <param name="pRect">Pointer to a RECT structure that contains the 
        /// rectangle, in logical coordinates, in which the background image 
        /// is drawn</param>
        /// <param name="pClipRect">Pointer to a RECT structure that contains 
        /// a clipping rectangle. This parameter may be set to NULL</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);


        #endregion

        #region User32.dll functions

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ShowWindow(IntPtr hWnd, short State);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool UpdateWindow(IntPtr hWnd);
        /// <summary>
        /// Gives focus to a given window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool CloseClipboard();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool EmptyClipboard();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardData(uint Format, IntPtr hData);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool GetMenuItemRect(IntPtr hWnd, IntPtr hMenu, uint Item, ref RECT rc);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref POINT lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref TBBUTTON lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref TBBUTTONINFO lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref REBARBANDINFO lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref TVITEM lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref LVITEM lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref HDITEM lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref HD_HITTESTINFO hti);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int hookid, HookProc pfnhook, IntPtr hinst, int threadid);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static int DrawText(IntPtr hdc, string lpString, int nCount, ref RECT lpRect, int uFormat);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr SetParent(IntPtr hChild, IntPtr hParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr GetDlgItem(IntPtr hDlg, int nControlID);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static int GetClientRect(IntPtr hWnd, ref RECT rc);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static int InvalidateRect(IntPtr hWnd, IntPtr rect, int bErase);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool WaitMessage();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMessage(ref MSG msg, IntPtr hWnd, uint wFilterMin, uint wFilterMax);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool DispatchMessage(ref MSG msg);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, uint cursor);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetCursor(IntPtr hCursor);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern ushort GetKeyState(int virtKey);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, out STRINGBUFFER ClassName, int nMaxCount);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hRegion, uint flags);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int FillRect(IntPtr hDC, ref RECT rect, IntPtr hBrush);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT wp);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowText(IntPtr hWnd, string text);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, out STRINGBUFFER text, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int SetScrollInfo(IntPtr hwnd, int bar, ref SCROLLINFO si, int fRedraw);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowScrollBar(IntPtr hWnd, int bar, int show);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int EnableScrollBar(IntPtr hWnd, uint flags, uint arrows);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollInfo(IntPtr hwnd, int bar, ref SCROLLINFO si);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy,ref RECT rcScroll, ref RECT rcClip, IntPtr UpdateRegion, ref RECT rcInvalidated, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int IsWindow(IntPtr hWnd);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] Specifies the virtual-key code to be translated. 
            int uScanCode, // [in] Specifies the hardware scan code of the key to be translated. The high-order bit of this value is set if the key is up (not pressed). 
            byte[] lpbKeyState, // [in] Pointer to a 256-byte array that contains the current keyboard state. Each element (byte) in the array contains the state of one key. If the high-order bit of a byte is set, the key is down (pressed). The low bit, if set, indicates that the key is toggled on. In this function, only the toggle bit of the CAPS LOCK key is relevant. The toggle state of the NUM LOCK and SCROLL LOCK keys is ignored.
            byte[] lpwTransKey, // [out] Pointer to the buffer that receives the translated character or characters. 
            int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "GetClassLongW")]
        public static extern int GetClassLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtrW")]
        public static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);



        /// <summary>
        /// Creates the helper window that receives messages from the taskar icon.
        /// </summary>
        [DllImport("USER32.DLL", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
                               [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y,
                               int nWidth, int nHeight, uint hWndParent, int hMenu, int hInstance,
                               int lpParam);


        /// <summary>
        /// Processes a default windows procedure.
        /// </summary>
        [DllImport("USER32.DLL")]
        public static extern long DefWindowProc(IntPtr hWnd, uint msg, uint wparam, uint lparam);


        /// <summary>
        /// Registers a listener for a window message.
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("User32.Dll", EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// Used to destroy the hidden helper window that receives messages from the
        /// taskbar icon.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the maximum number of milliseconds that can elapse between a
        /// first click and a second click for the OS to consider the
        /// mouse action a double-click.
        /// </summary>
        /// <returns>The maximum amount of time, in milliseconds, that can
        /// elapse between a first click and a second click for the OS to
        /// consider the mouse action a double-click.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, int wIDNewItem, string lpNewItem);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AppendMenu(IntPtr hMenu, int wFlags, int wIDNewItem, string lpNewItem);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);



        /// <summary>
        /// The TrackMouseEvent function posts messages when the mouse pointer 
        /// leaves a window or hovers over a window for a specified amount of time
        /// </summary>
        /// <param name="tme">A TRACKMOUSEEVENT structure that contains tracking 
        /// information</param>
        /// <returns>true if the function succeeds, false otherwise</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool TrackMouseEvent(TRACKMOUSEEVENT tme);




        /// <summary>
        /// The MessageBeep function plays a waveform sound. The waveform sound for each 
        /// sound type is identified by an entry in the registry
        /// </summary>
        /// <param name="type">Sound type, as identified by an entry in the registry</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function 
        /// fails, the return value is zero</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool MessageBeep(int type);


        /// <summary>
        /// The NotifyWinEvent function signals the system that a predefined event occurred. 
        /// If any client applications have registered a hook function for the event, the 
        /// system calls the client's hook function
        /// </summary>
        /// <param name="winEvent">Specifies the event that occurred</param>
        /// <param name="hwnd">Handle to the window that contains the object that generated 
        /// the event</param>
        /// <param name="objType">Identifies the kind of object that generated the event</param>
        /// <param name="objID">Identifies whether the event was generated by an object or 
        /// by a child element of the object. If this value is CHILDID_SELF, the event was 
        /// generated by the object itself. If not, this value is the child ID of the element 
        /// that generated the event</param>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern void NotifyWinEvent(int winEvent, IntPtr hwnd, int objType, int objID);


        /// <summary>
        /// The ScrollWindow function scrolls the contents of the specified window's client area
        /// </summary>
        /// <param name="hWnd">Handle to the window where the client area is to be scrolled</param>
        /// <param name="XAmount">Specifies the amount, in device units, of horizontal scrolling. 
        /// This parameter must be a negative value to scroll the content of the window to the left</param>
        /// <param name="YAmount">Specifies the amount, in device units, of vertical scrolling. 
        /// This parameter must be a negative value to scroll the content of the window up</param>
        /// <param name="lpRect">Pointer to the RECT structure specifying the portion of the 
        /// client area to be scrolled. If this parameter is NULL, the entire client area is 
        /// scrolled</param>
        /// <param name="lpClipRect">Pointer to the RECT structure containing the coordinates 
        /// of the clipping rectangle. Only device bits within the clipping rectangle are affected. 
        /// Bits scrolled from the outside of the rectangle to the inside are painted; bits scrolled 
        /// from the inside of the rectangle to the outside are not painted</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, 
        /// the return value is zero</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool ScrollWindow(IntPtr hWnd, int XAmount, int YAmount, ref RECT lpRect, ref RECT lpClipRect);


        /// <summary>
        /// The keybd_event function synthesizes a keystroke. The system can use such a synthesized 
        /// keystroke to generate a WM_KEYUP or WM_KEYDOWN message. The keyboard driver's interrupt 
        /// handler calls the keybd_event function
        /// </summary>
        /// <param name="bVk">Specifies a virtual-key code</param>
        /// <param name="bScan">This parameter is not used</param>
        /// <param name="dwFlags">Specifies various aspects of function operation</param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static void keybd_event(byte bVk, byte bScan, KeyEventFFlags dwFlags, int dwExtraInfo);


        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);



        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();


        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool DestroyWindow(int hndw);
        
        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, [MarshalAs(UnmanagedType.AsAny)]   object lParam);
        
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

        #region Common Controls functions


        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);
        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControls();
        [DllImport("comctl32.dll", EntryPoint = "DllGetVersion")]
        public extern static int GetCommonControlDLLVersion(ref DLLVERSIONINFO dvi);
        [DllImport("comctl32.dll")]
        public static extern IntPtr ImageList_Create(int width, int height, uint flags, int count, int grow);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_Destroy(IntPtr handle);
        [DllImport("comctl32.dll")]
        public static extern int ImageList_Add(IntPtr imageHandle, IntPtr hBitmap, IntPtr hMask);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_Remove(IntPtr imageHandle, int index);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_BeginDrag(IntPtr imageHandle, int imageIndex, int xHotSpot, int yHotSpot);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_DragEnter(IntPtr hWndLock, int x, int y);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_DragMove(int x, int y);
        [DllImport("comctl32.dll")]
        public static extern bool ImageList_DragLeave(IntPtr hWndLock);
        [DllImport("comctl32.dll")]
        public static extern void ImageList_EndDrag();

        /// <summary>
        /// Implemented by many of the Microsoft?Windows?Shell dynamic-link libraries 
        /// (DLLs) to allow applications to obtain DLL-specific version information
        /// </summary>
        /// <param name="pdvi">Pointer to a DLLVERSIONINFO structure that receives the 
        /// version information. The cbSize member must be filled in before calling 
        /// the function</param>
        /// <returns>Returns NOERROR if successful, or an OLE-defined error value otherwise</returns>
        [DllImport("Comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);




        [DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
        public static extern int TaskDialog(IntPtr hWndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, IntPtr pszIcon, out int pnButton);
        [DllImport("comctl32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern IntPtr TaskDialogIndirect(ref TaskDialogConfig pTaskConfig, out int pnButton, out int pnRadioButton, out bool pfVerificationFlagChecked);


        #endregion

        #region dwmapi.dll functions DWM API

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, Margins pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmGetColorizationColor(
            out int pcrColorization,
            [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableComposition(bool bEnable);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern IntPtr DwmRegisterThumbnail(IntPtr dest, IntPtr source);


        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUpdateThumbnailProperties(IntPtr hThumbnail, DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out Size size);



        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref BlurBehind pBlurBehind);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool pfEnabled);
        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(Thumbnail hThumbnail, out DwmSize pSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource, out Thumbnail phThumbnailId);
        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr hThumbnailId);
        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(Thumbnail hThumbnailId, ref DwmThumbnailProperties ptnProperties);


        #endregion

        #region Win32 Macro-Like helpers

        public static int GET_X_LPARAM(int lParam)
        {
            return (lParam & 0xffff);
        }

        public static int GET_Y_LPARAM(int lParam)
        {
            return (lParam >> 16);
        }

        public static Point GetPointFromLPARAM(int lParam)
        {
            return new Point(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
        }

        public static int LOW_ORDER(int param)
        {
            return (param & 0xffff);
        }

        public static int HIGH_ORDER(int param)
        {
            return (param >> 16);
        }



        #endregion

        #region advapi32.dll functions DWM API

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        #endregion

        #region shell32.dll functions
        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref   SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        #endregion

        #region USB Key 加密狗


        /// <summary>
        /// 查找加密锁 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="OutKeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int FindPort(int start, StringBuilder OutKeyPath);
        /// <summary>
        /// 获到锁的版本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int GetIDVersion(StringBuilder id, ref short version, string KeyPath);
        /// <summary>
        /// 查找指定的加密锁
        /// </summary>
        /// <param name="start"></param>
        /// <param name="HashFlag"></param>
        /// <param name="OutKeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int FindPort_2(int start, string HashFlag, StringBuilder OutKeyPath);
        /// <summary>
        /// 从加密锁中读取一批字节
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="mylen"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="OutData"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YRead(int Address, int mylen, string HKey, string LKey, byte[] OutData, string KeyPath);
        /// <summary>
        /// 写一批字节到加密锁中
        /// </summary>
        /// <param name="InData"></param>
        /// <param name="Address"></param>
        /// <param name="mylen"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YWrite(byte[] InData, int Address, int mylen, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 从加密锁中读字符串
        /// </summary>
        /// <param name="outstring"></param>
        /// <param name="Address"></param>
        /// <param name="mylen"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YReadString(StringBuilder outstring, int Address, int mylen, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 从加密锁中读一个长整形
        /// </summary>
        /// <param name="OutData"></param>
        /// <param name="Address"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YReadLong(ref int OutData, int Address, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 获取储存在锁中的用户名
        /// </summary>
        /// <param name="outstring"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int GetName(StringBuilder outstring, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 获取储存在锁中的用户密码
        /// </summary>
        /// <param name="outstring"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int GetPWD(StringBuilder outstring, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 写一个字符串到锁中
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="Address"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YWriteString(string InString, int Address, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 写一个长整形到锁中
        /// </summary>
        /// <param name="InData"></param>
        /// <param name="Address"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int YWriteLong(int InData, int Address, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 设置特征码到锁中
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetFeature(string InString, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 设置用户名到锁中
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetName(string InString, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 设置用户密码到锁中
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="HKey"></param>
        /// <param name="LKey"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetPWD(string InString, string HKey, string LKey, string KeyPath);
        /// <summary>
        /// 设置开发商标识
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetFlag(string InString, string KeyPath);
        /// <summary>
        /// 初始化加密锁
        /// </summary>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int ReSet(string KeyPath);
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="Old_HKey"></param>
        /// <param name="Old_LKey"></param>
        /// <param name="new_HKey"></param>
        /// <param name="new_LKey"></param>
        /// <param name="flag"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetPassword(string Old_HKey, string Old_LKey, string new_HKey, string new_LKey, int flag, string KeyPath);
        /// <summary>
        /// 设置Pin模式
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="InMode"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetPinMode(int pin, int InMode, string KeyPath);
        /// <summary>
        /// 设置U盘是否只读模式
        /// </summary>
        /// <param name="IsReadOnly"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetReadOnly(bool IsReadOnly, string KeyPath);
        /// <summary>
        /// 打开PIN
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int OpenPin(int pin, string KeyPath);
        /// <summary>
        /// 设置PIN
        /// </summary>
        /// <param name="OldPin"></param>
        /// <param name="NewPin"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int SetPin(int OldPin, int NewPin, string KeyPath);
        /// <summary>
        /// HASH开发商标志
        /// </summary>
        /// <param name="outstring"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int Sha1Flag(StringBuilder outstring, string KeyPath);
        /// <summary>
        /// HASH用户数据
        /// </summary>
        /// <param name="InHashData"></param>
        /// <param name="OutHashData"></param>
        /// <param name="KeyPath"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int Sha1Data(string InHashData, StringBuilder OutHashData, string KeyPath);
        /// <summary>
        /// 返回错误码相关信息
        /// </summary>
        /// <param name="ErrCode"></param>
        /// <param name="OutErrInfo"></param>
        [DllImport("UKOtpDll.dll")]
        public static extern void GetKeyErrorInfo(int ErrCode, StringBuilder OutErrInfo);

        /// <summary>
        /// 获取字符串长度
        /// </summary>
        /// <param name="InString"></param>
        /// <returns></returns>
        [DllImport("UKOtpDll.dll")]
        public static extern int GetLen(string InString);

        #endregion

        #region camera P/Invoke

        [DllImport("avicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int capCreateCaptureWindowA([MarshalAs(UnmanagedType.VBByRefStr)]   ref   string lpszWindowName, int dwStyle, int x, int y, int nWidth, short nHeight, int hWndParent, int nID);
        [DllImport("avicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool capGetDriverDescriptionA(short wDriver, [MarshalAs(UnmanagedType.VBByRefStr)]   ref   string lpszName, int cbName, [MarshalAs(UnmanagedType.VBByRefStr)]   ref   string lpszVer, int cbVer);
        [DllImport("vfw32.dll")]
        public static extern string capVideoStreamCallback(int hwnd, WebCamera.VideohdrTag videohdr_tag);
        [DllImport("vicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool capSetCallbackOnFrame(int hwnd, string s);

        #endregion
    }
}