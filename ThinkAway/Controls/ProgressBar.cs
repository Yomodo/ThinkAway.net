using ThinkAway.Core;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThinkAway.Controls
{
    [ToolboxBitmap(typeof(ProgressBar))]
    public class ProgressBar : System.Windows.Forms.ProgressBar
    {
        private States _ps;

        public ProgressBar()
        {
            Win32API.SendMessage(base.Handle, 0x410, 1, 0);
        }

        public void SetState(States State)
        {
            Win32API.SendMessage(base.Handle, 0x410, 1, 0);
            switch (State)
            {
                case States.Normal:
                    Win32API.SendMessage(base.Handle, 0x410, 1, 0);
                    return;

                case States.Error:
                    Win32API.SendMessage(base.Handle, 0x410, 2, 0);
                    return;

                case States.Paused:
                    Win32API.SendMessage(base.Handle, 0x410, 3, 0);
                    return;
            }
            Win32API.SendMessage(base.Handle, 0x410, 1, 0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 15)
            {
                this.SetState(this._ps);
            }
            base.WndProc(ref m);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style |= 0x10;
                return createParams;
            }
        }

        [Description("Gets or sets the ProgressBar state."), Category("Appearance"), DefaultValue(0)]
        public States ProgressState
        {
            get
            {
                return this._ps;
            }
            set
            {
                this._ps = value;
                this.SetState(this._ps);
            }
        }

        public enum States
        {
            Normal,
            Error,
            Paused
        }
    }
}

