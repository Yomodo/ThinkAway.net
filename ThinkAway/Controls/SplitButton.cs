using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    public class SplitButton : System.Windows.Forms.Button
    {
        [CompilerGenerated] private ContextMenu _SplitMenu;
        [CompilerGenerated] private ContextMenuStrip _SplitMenuStrip;

        [Category("Action"), Description("Occurs when the split button is clicked.")]
        public event EventHandler<SplitMenuEventArgs> SplitClick;

        [Description("Occurs when the split label is clicked, but before the associated context menu is displayed."), Category("Action")]
        public event EventHandler<SplitMenuEventArgs> SplitMenuOpening;

        protected virtual void OnSplitClick(SplitMenuEventArgs e)
        {
            if ((this.SplitMenuOpening != null) && ((this.SplitMenu != null) || (this.SplitMenuStrip != null)))
            {
                this.SplitMenuOpening(this, e);
            }
            Point pos = new Point(e.DrawArea.Left, e.DrawArea.Bottom);
            if (!e.PreventOpening)
            {
                if (this.SplitMenu != null)
                {
                    this.SplitMenu.Show(this, pos);
                }
                else if (this.SplitMenuStrip != null)
                {
                    this.SplitMenuStrip.Width = e.DrawArea.Width;
                    this.SplitMenuStrip.Show(this, pos);
                }
            }
            if (this.SplitClick != null)
            {
                this.SplitClick(this, e);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == 0x1606) && (m.WParam.ToInt32() == 1))
            {
                this.OnSplitClick(new SplitMenuEventArgs(base.ClientRectangle));
            }
            base.WndProc(ref m);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style |= base.IsDefault ? 13 : 12;
                return createParams;
            }
        }

        [DefaultValue((string) null), Description("Sets the context menu that is displayed by clicking on the split button."), Category("Behavior")]
        public ContextMenu SplitMenu
        {
            [CompilerGenerated]
            get
            {
                return this._SplitMenu;
            }
            [CompilerGenerated]
            set
            {
                this._SplitMenu = value;
            }
        }

        [Description("Sets the context menu that is displayed by clicking on the split button."), DefaultValue((string) null), Category("Behavior")]
        public ContextMenuStrip SplitMenuStrip
        {
            [CompilerGenerated]
            get
            {
                return this._SplitMenuStrip;
            }
            [CompilerGenerated]
            set
            {
                this._SplitMenuStrip = value;
            }
        }

        public class SplitMenuEventArgs : EventArgs
        {
            [CompilerGenerated]
            private Rectangle _DrawArea;
            [CompilerGenerated]
            private bool _PreventOpening;

            public SplitMenuEventArgs()
            {
                this.PreventOpening = false;
            }

            public SplitMenuEventArgs(Rectangle drawArea)
            {
                this.DrawArea = drawArea;
            }

            public Rectangle DrawArea
            {
                [CompilerGenerated]
                get
                {
                    return this._DrawArea;
                }
                [CompilerGenerated]
                set
                {
                    this._DrawArea = value;
                }
            }

            public bool PreventOpening
            {
                [CompilerGenerated]
                get
                {
                    return this._PreventOpening;
                }
                [CompilerGenerated]
                set
                {
                    this._PreventOpening = value;
                }
            }
        }
    }
}

