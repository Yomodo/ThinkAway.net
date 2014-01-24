using System.Drawing;
using System.Windows.Forms;
using ThinkAway.Core;

namespace ThinkAway.Controls.Dwm
{
    public static class GlassHelper
    {
        public static void HandleBackgroundPainting(Form form, Margins margins)
        {
            new HandleBackground(form, margins);
        }

        public static void HandleFormMovementOnGlass(Form form, Margins margins)
        {
            new HandleFormMovement(form, margins);
        }

        private class HandleBackground
        {
            private Margins _margins;

            public HandleBackground(Form form, Margins m)
            {
                this._margins = m;
                form.Paint += this.form_Paint;
            }

            private void form_Paint(object sender, PaintEventArgs e)
            {
                if (this._margins.IsMarginless)
                {
                    e.Graphics.Clear(Color.Black);
                }
                else
                {
                    Form form = (Form) sender;
                    Rectangle[] rects = new Rectangle[] { new Rectangle(0, 0, form.ClientSize.Width, this._margins.Top), new Rectangle(form.ClientSize.Width - this._margins.Right, 0, this._margins.Right, form.ClientSize.Height), new Rectangle(0, form.ClientSize.Height - this._margins.Bottom, form.ClientSize.Width, this._margins.Bottom), new Rectangle(0, 0, this._margins.Left, form.ClientSize.Height) };
                    e.Graphics.FillRectangles(Brushes.Black, rects);
                }
            }
        }

        private class HandleFormMovement
        {
            private Point _lastPos;
            private Margins _margins;
            private bool _tracking;

            public HandleFormMovement(Form form, Margins margins)
            {
                this._margins = margins;
                form.MouseDown += this.form_MouseDown;
                form.MouseUp += this.form_MouseUp;
                form.MouseMove += this.form_MouseMove;
            }

            private void form_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Form form = (Form) sender;
                    if ((this._margins.IsMarginless || (e.X <= this._margins.Left)) || (((e.X >= (form.ClientSize.Width - this._margins.Right)) || (e.Y <= this._margins.Top)) || (e.Y >= (form.ClientSize.Height - this._margins.Bottom))))
                    {
                        this._tracking = true;
                        this._lastPos = form.PointToScreen(e.Location);
                    }
                }
            }

            private void form_MouseMove(object sender, MouseEventArgs e)
            {
                if (this._tracking)
                {
                    Form form = (Form) sender;
                    Point point = form.PointToScreen(e.Location);
                    Point p = new Point(point.X - this._lastPos.X, point.Y - this._lastPos.Y);
                    Point location = form.Location;
                    location.Offset(p);
                    form.Location = location;
                    this._lastPos = point;
                }
            }

            private void form_MouseUp(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this._tracking = false;
                }
            }
        }
    }
}

