using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls.Dwm
{
    public class ThumbnailViewer : Control
    {
        private ContentAlignment _alignment = ContentAlignment.MiddleCenter;
        private bool _lastVisibilityStatus = true;
        private bool _onlyClientArea = true;
        private byte _opacity = 0xff;
        private readonly EventHandler _parentChangeHandler;
        private Control _parentControl;
        private bool _scaleSmallerThumbnails = true;
        private Thumbnail _thumbnail;
        private Form _topLevelForm;

        public ThumbnailViewer()
        {
            this._parentChangeHandler = this._parentControl_VisibleChanged;
        }

        private void _parentControl_VisibleChanged(object sender, EventArgs e)
        {
            this.UpdateThumbnail(this._parentControl.Visible);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            this.UpdateThumbnail(Visible);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.RecomputeParentForm();
            if (this._parentControl != null)
            {
                this._parentControl.VisibleChanged -= this._parentChangeHandler;
            }
            this._parentControl = Parent;
            this._parentControl.VisibleChanged += this._parentChangeHandler;
            base.OnParentChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.UpdateThumbnail(Visible);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.UpdateThumbnail(Visible);
        }

        private void originForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this._thumbnail != null)
            {
                this._thumbnail.Dispose();
                this._thumbnail = null;
            }
        }

        private void originForm_SizeChanged(object sender, EventArgs e)
        {
            this.Update();
        }

        private void RecomputeParentForm()
        {
            Form topLevelControl = base.TopLevelControl as Form;
            if (topLevelControl == null)
            {
                this._topLevelForm = null;
                if (this._thumbnail != null)
                {
                    this._thumbnail.Dispose();
                    this._thumbnail = null;
                }
            }
            else
            {
                if ((this._thumbnail != null) && (this._topLevelForm != topLevelControl))
                {
                    this._thumbnail.Dispose();
                    this._thumbnail = null;
                }
                this._topLevelForm = topLevelControl;
            }
        }

        private Rectangle RecomputeThumbnailRectangle()
        {
            if ((this._topLevelForm == null) || (this._thumbnail == null))
            {
                throw new Exception("whops, no parent or no thumbnail");
            }
            Point empty = Point.Empty;
            Control parent = this;
            do
            {
                empty = new Point(empty.X + parent.Location.X, empty.Y + parent.Location.Y);
                parent = parent.Parent;
            }
            while ((parent != null) && (parent != base.TopLevelControl));
            Size clientSize = base.ClientSize;
            Size sourceSize = this._thumbnail.SourceSize;
            if (((sourceSize.Width < clientSize.Width) && (sourceSize.Height < clientSize.Height)) && !this._scaleSmallerThumbnails)
            {
                clientSize = sourceSize;
            }
            if ((sourceSize.Width > clientSize.Width) || (sourceSize.Height > clientSize.Height))
            {
                double num = ((double) sourceSize.Width) / ((double) sourceSize.Height);
                if (sourceSize.Width < sourceSize.Height)
                {
                    clientSize.Width = (int) (clientSize.Height * num);
                }
                else
                {
                    clientSize.Height = (int) (((double) clientSize.Width) / num);
                }
            }
            int num2 = base.ClientSize.Width - clientSize.Width;
            int num3 = base.ClientSize.Height - clientSize.Height;
            if (((this.ThumbnailAlignment == ContentAlignment.MiddleCenter) || (this.ThumbnailAlignment == ContentAlignment.MiddleLeft)) || (this.ThumbnailAlignment == ContentAlignment.MiddleRight))
            {
                empty = new Point(empty.X, empty.Y + (num3 / 2));
            }
            if (((this.ThumbnailAlignment == ContentAlignment.BottomCenter) || (this.ThumbnailAlignment == ContentAlignment.BottomLeft)) || (this.ThumbnailAlignment == ContentAlignment.BottomRight))
            {
                empty = new Point(empty.X, empty.Y + num3);
            }
            if (((this.ThumbnailAlignment == ContentAlignment.BottomCenter) || (this.ThumbnailAlignment == ContentAlignment.MiddleCenter)) || (this.ThumbnailAlignment == ContentAlignment.TopCenter))
            {
                empty = new Point(empty.X + (num2 / 2), empty.Y);
            }
            if (((this.ThumbnailAlignment == ContentAlignment.BottomRight) || (this.ThumbnailAlignment == ContentAlignment.MiddleRight)) || (this.ThumbnailAlignment == ContentAlignment.TopRight))
            {
                empty = new Point(empty.X + num2, empty.Y);
            }
            return new Rectangle(empty, clientSize);
        }

        public void SetThumbnail(IntPtr originHandle)
        {
            this.RecomputeParentForm();
            if (this._topLevelForm == null)
            {
                throw new Exception("Control must have an owner.");
            }
            this._thumbnail = DwmManager.Register(this._topLevelForm, originHandle);
            this.UpdateThumbnail(base.Visible);
        }

        public void SetThumbnail(Form originForm)
        {
            this.SetThumbnail(originForm.Handle);
        }

        public void SetThumbnail(Form originForm, bool trackFormUpdates)
        {
            this.SetThumbnail(originForm.Handle);
            if (trackFormUpdates)
            {
                originForm.SizeChanged += new EventHandler(this.originForm_SizeChanged);
                originForm.FormClosed += new FormClosedEventHandler(this.originForm_FormClosed);
            }
        }

        public new void Update()
        {
            this.UpdateThumbnail(base.Visible);
        }

        protected void UpdateThumbnail(bool visible)
        {
            if (this._lastVisibilityStatus || visible)
            {
                if (this._thumbnail != null)
                {
                    this._thumbnail.Update(this.RecomputeThumbnailRectangle(), this._opacity, visible, this._onlyClientArea);
                }
                this._lastVisibilityStatus = visible;
            }
        }

        [DefaultValue((byte) 0xff), Description("Sets the opacity of the thumbnail."), Category("Appearance")]
        public byte Opacity
        {
            get
            {
                return this._opacity;
            }
            set
            {
                this._opacity = value;
                this.UpdateThumbnail(base.Visible);
            }
        }

        public bool ScaleSmallerThumbnails
        {
            get
            {
                return this._scaleSmallerThumbnails;
            }
            set
            {
                this._scaleSmallerThumbnails = value;
                this.UpdateThumbnail(base.Visible);
            }
        }

        [Category("Appearance"), Description("Determines whether to show only the client area of the window or the whole window."), DefaultValue(true)]
        public bool ShowOnlyClientArea
        {
            get
            {
                return this._onlyClientArea;
            }
            set
            {
                this._onlyClientArea = value;
                this.UpdateThumbnail(base.Visible);
            }
        }

        public ContentAlignment ThumbnailAlignment
        {
            get
            {
                return this._alignment;
            }
            set
            {
                this._alignment = value;
                this.UpdateThumbnail(base.Visible);
            }
        }
    }
}

