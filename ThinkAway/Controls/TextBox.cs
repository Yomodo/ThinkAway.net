using ThinkAway.Core;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(TextBox))]
    public class TextBox : System.Windows.Forms.TextBox
    {
        private string _cueBannerText = string.Empty;
        private bool _showCueFocused;

        private void SetCueText(bool showFocus)
        {
            Win32API.SendMessage(base.Handle, 0x1501, new IntPtr(showFocus ? 1 : 0), this._cueBannerText);
        }

        [Description("Text that is displayed as Cue banner."), Category("Appearance"), DefaultValue("")]
        public string CueBannerText
        {
            get
            {
                return this._cueBannerText;
            }
            set
            {
                this._cueBannerText = value;
                this.SetCueText(this.ShowCueFocused);
            }
        }

        [Browsable(false)]
        public new bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                _showCueFocused = value;
                base.Multiline = false;
            }
        }

        [Category("Appearance"), Description("If true, the Cue text will be displayed even when the control has keyboard focus."), DefaultValue(false)]
        public bool ShowCueFocused
        {
            get
            {
                return this._showCueFocused;
            }
            set
            {
                this._showCueFocused = value;
                this.SetCueText(value);
            }
        }
    }
}

