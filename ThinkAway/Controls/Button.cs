using ThinkAway.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(Button))]
    public class Button : System.Windows.Forms.Button
    {
        private System.Drawing.Icon _icon;
        private Bitmap _image;
        private bool _showshield;
        private bool _useicon = true;

        public Button()
        {
            FlatStyle = FlatStyle.System;
        }

        [Description("Refreshes the image displayed on the button.")]
        public void SetImage()
        {
            IntPtr zero = IntPtr.Zero;
            if (!this._useicon)
            {
                if (this._image != null)
                {
                    zero = this._image.GetHicon();
                }
            }
            else if (this._icon != null)
            {
                zero = this.Icon.Handle;
            }
            Win32API.SendMessage(base.Handle, 0xf7, 1, (int) zero);
        }

        public void SetShield(bool Value)
        {
            Win32API.SendMessage(base.Handle, 0x160c, IntPtr.Zero, new IntPtr(this._showshield ? 1 : 0));
        }

        [Description("Gets or sets the icon that is displayed on a button control."), DefaultValue((string) null), Category("Appearance")]
        public System.Drawing.Icon Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                this._icon = value;
                if (this._icon != null)
                {
                    this._useicon = true;
                }
                this.SetShield(false);
                this.SetImage();
            }
        }

        [Category("Appearance"), Description("Gets or sets the image that is displayed on a button control."), DefaultValue((string) null)]
        public new Bitmap Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
                if (value != null)
                {
                    this._useicon = false;
                    this.Icon = null;
                }
                this.SetShield(false);
                this.SetImage();
            }
        }

        [DefaultValue(false), Description("Gets or sets whether if the control should use an elevated shield icon."), Category("Appearance")]
        public bool ShowShield
        {
            get
            {
                return this._showshield;
            }
            set
            {
                this._showshield = value;
                this.SetShield(value);
                if (!value)
                {
                    this.SetImage();
                }
            }
        }
    }
}

