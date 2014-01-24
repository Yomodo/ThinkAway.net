using ThinkAway.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(Button))]
    public class CommandLink : System.Windows.Forms.Button
    {
        private System.Drawing.Icon _icon;
        private Bitmap _image;
        private string _note = string.Empty;
        private bool _showshield;
        private bool _useicon = true;

        public CommandLink()
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

        [Description("Sets the note displayed on the button.")]
        private void SetNote(string NoteText)
        {
            Win32API.SendMessage(base.Handle, 0x1609, IntPtr.Zero, NoteText);
        }

        public void SetShield(bool Value)
        {
            Win32API.SendMessage(base.Handle, 0x160c, IntPtr.Zero, new IntPtr(this._showshield ? 1 : 0));
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style |= 14;
                return createParams;
            }
        }

        [Category("Appearance"), Description("Gets or sets the icon that is displayed on a button control."), DefaultValue((string) null)]
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

        [Description("Gets or sets the note that is displayed on a button control."), Category("Appearance"), DefaultValue("")]
        public string Note
        {
            get
            {
                return this._note;
            }
            set
            {
                this._note = value;
                this.SetNote(this._note);
            }
        }

        [Description("Gets or sets whether if the control should use an elevated shield icon."), DefaultValue(false), Category("Appearance")]
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

