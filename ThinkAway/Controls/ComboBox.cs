using ThinkAway.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(ComboBox))]
    public class ComboBox : System.Windows.Forms.ComboBox
    {
        private string _cueBannerText = string.Empty;

        public ComboBox()
        {
            FlatStyle = FlatStyle.System;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void SetCueText()
        {
            Win32API.SendMessage(Handle, 0x1703, IntPtr.Zero, this._cueBannerText);
        }

        [DefaultValue(""), Category("Appearance"), Description("Gets or sets the cue text that is displayed on a ComboBox control.")]
        public string CueBannerText
        {
            get
            {
                return this._cueBannerText;
            }
            set
            {
                this._cueBannerText = value;
                this.SetCueText();
            }
        }


    }
}

