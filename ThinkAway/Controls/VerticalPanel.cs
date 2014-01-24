using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(Panel))]
    public sealed class VerticalPanel : Panel
    {
        public VerticalPanel()
        {
            this.BackColor = Color.Transparent;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.UpdateStyles();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            LinkLabel linkLabel = e.Control as LinkLabel;
            if (linkLabel != null)
            {
                e.Control.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
                (linkLabel).LinkBehavior = LinkBehavior.HoverUnderline;
                (linkLabel).LinkColor = Color.FromArgb(0x40, 0x40, 0x40);
                (linkLabel).ActiveLinkColor = Color.Blue;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color color = Color.FromArgb(0xcc, 0xd9, 0xea);
            Color color2 = Color.FromArgb(0xd9, 0xe3, 240);
            Color color3 = Color.FromArgb(0xe8, 0xee, 0xf7);
            Color color4 = Color.FromArgb(0xed, 0xf2, 0xf9);
            Color color5 = Color.FromArgb(240, 0xf4, 250);
            Color color6 = Color.FromArgb(0xf1, 0xf5, 0xfb);
            Rectangle rect = new Rectangle(base.Width - 1, 0, 1, base.Height);
            SolidBrush brush = new SolidBrush(color);
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(base.Width + 1, 0, 1, base.Height);
            brush.Color = color;
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(base.Width - 2, 0, 1, base.Height);
            brush.Color = color2;
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(base.Width - 3, 0, 1, base.Height);
            brush.Color = color3;
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(base.Width - 4, 0, 1, base.Height);
            brush.Color = color4;
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(base.Width - 5, 0, 1, base.Height);
            brush.Color = color5;
            e.Graphics.FillRectangle(brush, rect);
            rect = new Rectangle(0, 0, base.Width - 5, base.Height);
            brush.Color = color6;
            e.Graphics.FillRectangle(brush, rect);
            brush.Dispose();
        }

        public void RedrawControlAsBitmap(IntPtr hwnd)
        {
            Control control = Control.FromHandle(hwnd);
            using (Bitmap bitmap = new Bitmap(control.Width, control.Height))
            {
                control.DrawToBitmap(bitmap, control.ClientRectangle);
                using (Graphics graphics = control.CreateGraphics())
                {
                    Point point = new Point(-1, -1);
                    graphics.DrawImage(bitmap, point);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            const int num = 15;
            if ((m.Msg == num) && this.RenderOnGlass)
            {
                this.RedrawControlAsBitmap(base.Handle);
            }
        }

        private bool _renderOnGlass;

        [Description("Gets or sets whether the control can render on an Aero glass surface."), DefaultValue(false), Category("Appearance")]
        public bool RenderOnGlass
        {
            get { return _renderOnGlass; }
            set { _renderOnGlass = value; }
        }
    }
}

