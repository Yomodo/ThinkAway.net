using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

/*LsongStudio
 *song940@gmail.com 
 *http://lsong.org 
 */

namespace ThinkAway.Core.SystemMenu
{
    public class SystemMenuNativeWindow : NativeWindow, IDisposable
    {
        private IntPtr _hMenu;
        private Dictionary<int, EventHandler> _menuClickEventList;

        public SystemMenuNativeWindow(Form owner)
        {
            AssignHandle(owner.Handle);
            GetSystemMenu();
        }

        protected Dictionary<int, EventHandler> MenuClickEventList
        {
            get { return _menuClickEventList ?? (_menuClickEventList = new Dictionary<int, EventHandler>(10)); }
        }

        public bool InsertMenu(
            int position,
            int id,
            string text,
            EventHandler menuClickEvent)
        {
            return InsertMenu(
                position,
                id,
                MenuItemFlag.MF_BYPOSITION | MenuItemFlag.MF_STRING,
                text,
                menuClickEvent);
        }

        public bool InertSeparator(int position)
        {
            return InsertMenu(
                position,
                0,
                MenuItemFlag.MF_BYPOSITION | MenuItemFlag.MF_SEPARATOR,
                "",
                null);
        }

        public bool InsertMenu(
            int position,
            int id,
            MenuItemFlag flag,
            string text,
            EventHandler menuClickEvent)
        {
            if ((flag & MenuItemFlag.MF_SEPARATOR) != MenuItemFlag.MF_SEPARATOR &&
                !ValidateID(id))
            {
                throw new ArgumentOutOfRangeException(
                    "id",
                    string.Format(
                        "菜单ID只能在{0}-{1}之间取值。", 0, 0xF000));
            }

            bool sucess = Win32API.InsertMenu(
                _hMenu,
                position,
                (int) flag,
                id,
                text);
            if (sucess && menuClickEvent != null)
            {
                MenuClickEventList.Add(id, menuClickEvent);
            }
            return sucess;
        }

        public bool AppendMenu(
            int id,
            string text,
            EventHandler menuClickEvent)
        {
            return AppendMenu(
                id,
                MenuItemFlag.MF_BYPOSITION | MenuItemFlag.MF_STRING,
                text,
                menuClickEvent);
        }

        public bool AppendSeparator()
        {
            return AppendMenu(
                0,
                MenuItemFlag.MF_BYPOSITION | MenuItemFlag.MF_SEPARATOR,
                "",
                null);
        }

        public bool AppendMenu(
            int id,
            MenuItemFlag flag,
            string text,
            EventHandler menuClickEvent)
        {
            if ((flag & MenuItemFlag.MF_SEPARATOR) != MenuItemFlag.MF_SEPARATOR &&
                !ValidateID(id))
            {
                throw new ArgumentOutOfRangeException(
                    "id",
                    string.Format(
                        "菜单ID只能在{0}-{1}之间取值。", 0, 0xF000));
            }

            bool sucess = Win32API.AppendMenu(
                _hMenu,
                (int) flag,
                id,
                text);
            if (sucess && menuClickEvent != null)
            {
                MenuClickEventList.Add(id, menuClickEvent);
            }
            return sucess;
        }

        public void Revert()
        {
            Win32API.GetSystemMenu(Handle, true);
            Dispose();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_SYSCOMMAND:
                    OnWmSysCommand(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void GetSystemMenu()
        {
            _hMenu = Win32API.GetSystemMenu(Handle, false);
            if (_hMenu == IntPtr.Zero)
            {
                throw new Win32Exception("获取系统菜单失败。");
            }
        }

        private bool ValidateID(int id)
        {
            return id < 0xF000 && id > 0;
        }

        private void OnWmSysCommand(ref Message m)
        {
            int id = m.WParam.ToInt32();

            EventHandler handler;
            if (MenuClickEventList.TryGetValue(id, out handler))
            {
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
                m.Result = NativeMethods.TRUE;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            base.ReleaseHandle();
            _hMenu = IntPtr.Zero;
            if (_menuClickEventList != null)
            {
                _menuClickEventList.Clear();
                _menuClickEventList = null;
            }
        }

        #endregion
    }
}