using ThinkAway.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using Convert = System.Convert;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(TreeView))]
    public class TreeView : System.Windows.Forms.TreeView
    {
        public TreeView()
        {
            base.HotTracking = true;
            base.ShowLines = false;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Win32API.SetWindowTheme(base.Handle, "explorer", null);
            int lParam = Win32API.SendMessage(base.Handle, Convert.ToUInt32(0x112d), 0, 0) | 0x60;
            Win32API.SendMessage(base.Handle, 0x112c, 0, lParam);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style |= 0x8000;
                return createParams;
            }
        }

        [Browsable(false)]
        public new bool HotTracking
        {
            get
            {
                return base.HotTracking;
            }
            set
            {
                base.HotTracking = true;
            }
        }

        [Browsable(false)]
        public new bool ShowLines
        {
            get
            {
                return base.ShowLines;
            }
            set
            {
                base.ShowLines = false;
            }
        }
    }
}

