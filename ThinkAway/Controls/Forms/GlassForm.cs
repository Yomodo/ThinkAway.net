using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ThinkAway.Controls.Dwm;
using ThinkAway.Core;

namespace ThinkAway.Controls.Forms
{
    public class GlassForm : Form
    {
        private bool _glassEnabled = true;
        private Margins _glassMargins = new Margins(0);
        private bool _handleMouseMove = true;
        private Point _lastPos;
        private bool _tracking;

        public GlassForm()
        {
            base.ResizeRedraw = true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (((e.Button == MouseButtons.Left) && this.HandleMouseMove) && ((this._glassMargins.IsMarginless || (e.X <= this._glassMargins.Left)) || (((e.X >= (base.ClientSize.Width - this._glassMargins.Right)) || (e.Y <= this._glassMargins.Top)) || (e.Y >= (base.ClientSize.Height - this._glassMargins.Bottom)))))
            {
                this._tracking = true;
                this._lastPos = base.PointToScreen(e.Location);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._tracking)
            {
                Point point = base.PointToScreen(e.Location);
                Point p = new Point(point.X - this._lastPos.X, point.Y - this._lastPos.Y);
                Point location = base.Location;
                location.Offset(p);
                base.Location = location;
                this._lastPos = point;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._tracking = false;
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this._glassMargins.IsNull && this._glassEnabled)
            {
                if (this._glassMargins.IsMarginless)
                {
                    e.Graphics.Clear(Color.Black);
                }
                else
                {
                    Rectangle[] rects = new Rectangle[] { new Rectangle(0, 0, base.ClientSize.Width, this._glassMargins.Top), new Rectangle(base.ClientSize.Width - this._glassMargins.Right, 0, this._glassMargins.Right, base.ClientSize.Height), new Rectangle(0, base.ClientSize.Height - this._glassMargins.Bottom, base.ClientSize.Width, this._glassMargins.Bottom), new Rectangle(0, 0, this._glassMargins.Left, base.ClientSize.Height) };
                    e.Graphics.FillRectangles(Brushes.Black, rects);
                }
            }
        }

        private void SetGlass()
        {
            if (!this._glassMargins.IsNull && this._glassEnabled)
            {
                DwmManager.EnableGlassFrame(this, this._glassMargins);
            }
            else
            {
                DwmManager.DisableGlassFrame(this);
            }
            base.Invalidate();
        }

        [DefaultValue(true), Description("Enables or disables the glass margin."), Category("Appearance")]
        public bool GlassEnabled
        {
            get
            {
                return this._glassEnabled;
            }
            set
            {
                this._glassEnabled = value;
                this.SetGlass();
            }
        }

        [DefaultValue((string) null), Description("The glass margins which are extended inside the client area of the window."), Category("Appearance")]
        public Margins GlassMargins
        {
            get
            {
                return this._glassMargins;
            }
            set
            {
                this._glassMargins = value;
                this.SetGlass();
            }
        }

        [Description("True if mouse dragging of the window should be handled automatically."), DefaultValue(true), Category("Behavior")]
        public bool HandleMouseMove
        {
            get
            {
                return this._handleMouseMove;
            }
            set
            {
                this._handleMouseMove = value;
                if (!value)
                {
                    this._tracking = false;
                }
            }
        }
    }
}

