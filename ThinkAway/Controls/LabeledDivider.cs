using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    public sealed class LabeledDivider : Control
    {
        private Color _dividerColor = Color.FromArgb(0xb0, 0xbf, 0xde);
        private string _text = "";

        public LabeledDivider()
        {
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._text = base.Name;
            this.ForeColor = Color.FromArgb(0, 0x33, 170);
            base.Width = 200;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SolidBrush brush = new SolidBrush(this._dividerColor);
            SolidBrush brush2 = new SolidBrush(this.ForeColor);
            e.Graphics.DrawString(this.Text, this.Font, brush2, 0f, 0f);
            SizeF ef = e.Graphics.MeasureString(this.Text, this.Font);
            if (this.DividerPosition == DividerPositions.Center)
            {
                Rectangle rect = new Rectangle((int) ef.Width, (((int) ef.Height) / 2) + 1, base.Width - ((int) ef.Width), 1);
                e.Graphics.FillRectangle(brush, rect);
            }
            else if (this.DividerPosition == DividerPositions.Below)
            {
                Rectangle rectangle2 = new Rectangle(1, (int) ef.Height, base.Width, 1);
                e.Graphics.FillRectangle(brush, rectangle2);
            }
            brush2.Dispose();
            brush.Dispose();
        }

        [Category("Appearance"), Description("The color of the divider line.")]
        public Color DividerColor
        {
            get
            {
                return this._dividerColor;
            }
            set
            {
                this._dividerColor = value;
            }
        }

        private DividerPositions _dividerPosition;

        [Category("Appearance"), DefaultValue(0), Description("The placement of the divider line.")]
        public DividerPositions DividerPosition
        {
            get { return _dividerPosition; }
            set { _dividerPosition = value; }
        }

        [Description("The text that will display as the caption."), DefaultValue("DividerLabel"), Category("Appearance")]
        public override string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
                base.Invalidate();
            }
        }

        public enum DividerPositions
        {
            Center,
            Below
        }
    }
}

