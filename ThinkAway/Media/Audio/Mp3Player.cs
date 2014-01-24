using System;
using System.Runtime.InteropServices;
using System.Text;
using ThinkAway.Media.Player;

/* 
 * 
 * MP3Player class provides basic functionallity for playing mp3 files.
 * Aside from the various methods it implemants events which notify their subscribers for opening files, pausing, etc.
 * This is done for boosting performance on your applications using this class, because instead of checking for info
 * on the player status over a certain time period and loosing performance, you can subscribe for an event and handle it when it fires.
 * This class also doesn't throw exceptions. The error handling is done by an event, because the probable errors which may occur are not
 * severe and the application just needs to be notified for these failures on the fly...
 * Share your source and modify this code to your heart's content, just don't change this section.
 * If you have questions, suggestions or just need to make your oppinion heard my email is krazymir@gmail.com
 * Krasimir kRAZY Kalinov 2006
 * 
 * PS: This source will only work on MS Windows, since it uses the MCI(Media Control Interface) integrated into this OS.
 * Sorry .Gnu and Mono fans! I hope soon to have enough time to get busy working on similar class for these engines...
 * 
 */
namespace ThinkAway.Media.Audio
{
    public class Mp3Player
    {
        private string _pcommand;
        private bool _opened, _playing, _paused, _loop, _mutedAll, _mutedLeft, _mutedRight;
        private int _rVolume, _lVolume, _aVolume, _tVolume, _bVolume, _volBalance;
        private ulong _lng;
        private long _err;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public Mp3Player()
        {
            _opened = false;
            _pcommand = "";
            _fileName = "";
            _playing = false;
            _paused = false;
            _loop = false;
            _mutedAll = _mutedLeft = _mutedRight = false;
            _rVolume = _lVolume = _aVolume = _tVolume = _bVolume = 1000;
            _lng = 0;
            _volBalance = 0;
            _err = 0;
        }

        #region Volume
        public bool MuteAll
        {
            get
            {
                return _mutedAll;
            }
            set
            {
                _mutedAll = value;
                if (_mutedAll)
                {
                    _pcommand = "setaudio MediaFile off";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
                else
                {
                    _pcommand = "setaudio MediaFile on";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }

        }

        public bool MuteLeft
        {
            get
            {
                return _mutedLeft;
            }
            set
            {
                _mutedLeft = value;
                if (_mutedLeft)
                {
                    _pcommand = "setaudio MediaFile left off";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
                else
                {
                    _pcommand = "setaudio MediaFile left on";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }

        }

        public bool MuteRight
        {
            get
            {
                return _mutedRight;
            }
            set
            {
                _mutedRight = value;
                if (_mutedRight)
                {
                    _pcommand = "setaudio MediaFile right off";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
                else
                {
                    _pcommand = "setaudio MediaFile right on";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }

        }

        public int VolumeAll
        {
            get
            {
                return _aVolume;
            }
            set
            {
                if (_opened && (value >= 0 && value <= 1000))
                {
                    _aVolume = value;
                    _pcommand = String.Format("setaudio MediaFile volume to {0}", _aVolume);
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }
        }

        public int VolumeLeft
        {
            get
            {
                return _lVolume;
            }
            set
            {
                if (_opened && (value >= 0 && value <= 1000))
                {
                    _lVolume = value;
                    _pcommand = String.Format("setaudio MediaFile left volume to {0}", _lVolume);
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }
        }

        public int VolumeRight
        {
            get
            {
                return _rVolume;
            }
            set
            {
                if (_opened && (value >= 0 && value <= 1000))
                {
                    _rVolume = value;
                    _pcommand = String.Format("setaudio MediaFile right volume to {0}", _rVolume);
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }
        }

        public int VolumeTreble
        {
            get
            {
                return _tVolume;
            }
            set
            {
                if (_opened && (value >= 0 && value <= 1000))
                {
                    _tVolume = value;
                    _pcommand = String.Format("setaudio MediaFile treble to {0}", _tVolume);
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }
        }

        public int VolumeBass
        {
            get
            {
                return _bVolume;
            }
            set
            {
                if (_opened && (value >= 0 && value <= 1000))
                {
                    _bVolume = value;
                    _pcommand = String.Format("setaudio MediaFile bass to {0}", _bVolume);
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                }
            }
        }

        public int Balance
        {
            get
            {
                return _volBalance;
            }
            set
            {
                if (_opened && (value >= -1000 && value <= 1000))
                {
                    _volBalance = value;
                    if (value < 0)
                    {
                        _pcommand = "setaudio MediaFile left volume to 1000";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        _pcommand = String.Format("setaudio MediaFile right volume to {0}", 1000 + value);
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    }
                    else
                    {
                        _pcommand = "setaudio MediaFile right volume to 1000";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        _pcommand = String.Format("setaudio MediaFile left volume to {0}", 1000 - value);
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    }
                }
            }
        }
        #endregion

        #region Main Functions

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            private set { _fileName = value; }
        }

        public bool Looping
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
            }
        }

        public void Seek(ulong millisecs)
        {
            if (_opened && millisecs <= _lng)
            {
                if (_playing)
                {
                    if (_paused)
                    {
                        _pcommand = String.Format("seek MediaFile to {0}", millisecs);
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    }
                    else
                    {
                        _pcommand = String.Format("seek MediaFile to {0}", millisecs);
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        _pcommand = "play MediaFile";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    }
                }
            }
        }

        private void CalculateLength()
        {
            StringBuilder str = new StringBuilder(128);
            mciSendString("status MediaFile length", str, 128, IntPtr.Zero);
            _lng = Convert.ToUInt64(str.ToString());
        }

        public ulong AudioLength
        {
            get
            {
                return _opened ? _lng : 0;
            }
        }

        public void Close()
        {
            if (_opened)
            {
                _pcommand = "close MediaFile";
                if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                _opened = false;
                _playing = false;
                _paused = false;
                OnCloseFile(new CloseFileEventArgs());
            }
        }

        public void Open(string sFileName)
        {
            if (!_opened)
            {
                _pcommand = string.Format("open \"{0}\" type mpegvideo alias MediaFile", sFileName);
                if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                _fileName = sFileName;
                _opened = true;
                _playing = false;
                _paused = false;
                _pcommand = "set MediaFile time format milliseconds";
                if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                _pcommand = "set MediaFile seek exactly on";
                if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                CalculateLength();
                OnOpenFile(new OpenFileEventArgs(sFileName));
            }
            else
            {
                this.Close();
                this.Open(sFileName);
            }
        }

        public void Play()
        {
            if (_opened)
            {
                if (!_playing)
                {
                    _playing = true;
                    _pcommand = "play MediaFile";
                    if (_loop) _pcommand += " REPEAT";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    OnPlayFile(new PlayFileEventArgs());
                }
                else
                {
                    if (!_paused)
                    {
                        _pcommand = "seek MediaFile to start";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        _pcommand = "play MediaFile";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        OnPlayFile(new PlayFileEventArgs());
                    }
                    else
                    {
                        _paused = false;
                        _pcommand = "play MediaFile";
                        if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                        OnPlayFile(new PlayFileEventArgs());
                    }
                }
            }
        }

        public void Pause()
        {
            if (_opened)
            {
                if (!_paused)
                {
                    _paused = true;
                    _pcommand = "pause MediaFile";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    OnPauseFile(new PauseFileEventArgs());
                }
                else
                {
                    _paused = false;
                    _pcommand = "play MediaFile";
                    if((_err=mciSendString(_pcommand, null, 0, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    OnPlayFile(new PlayFileEventArgs());
                }
            }
        }

        public void Stop()
        {
            if (_opened && _playing)
            {
                _playing = false;
                _paused = false;
                _pcommand = "seek MediaFile to start";
                if ((_err = mciSendString(_pcommand, null, 0, IntPtr.Zero)) != 0) OnError(new ErrorEventArgs(_err));
                _pcommand = "stop MediaFile";
                if ((_err = mciSendString(_pcommand, null, 0, IntPtr.Zero)) != 0) OnError(new ErrorEventArgs(_err));
                OnStopFile(new StopFileEventArgs());
            }
        }

        public ulong CurrentPosition
        {
            get
            {
                if (_opened && _playing)
                {
                    StringBuilder s = new StringBuilder(128);
                    _pcommand = "status MediaFile position";
                    if((_err=mciSendString(_pcommand, s, 128, IntPtr.Zero))!=0)OnError(new ErrorEventArgs(_err));
                    return Convert.ToUInt64(s.ToString());
                }
                return 0;
            }
        }

        #endregion

        #region Event Handling

        public delegate void OpenFileEventHandler(Object sender, OpenFileEventArgs oea);

        public delegate void PlayFileEventHandler(Object sender, PlayFileEventArgs pea);

        public delegate void PauseFileEventHandler(Object sender, PauseFileEventArgs paea);

        public delegate void StopFileEventHandler(Object sender, StopFileEventArgs sea);

        public delegate void CloseFileEventHandler(Object sender, CloseFileEventArgs cea);

        public delegate void ErrorEventHandler(Object sender, ErrorEventArgs eea);

        public event OpenFileEventHandler OpenFile;

        public event PlayFileEventHandler PlayFile;

        public event PauseFileEventHandler PauseFile;

        public event StopFileEventHandler StopFile;

        public event CloseFileEventHandler CloseFile;

        public event ErrorEventHandler Error;

        protected virtual void OnOpenFile(OpenFileEventArgs oea)
        {
            if (OpenFile != null) OpenFile(this, oea);
        }

        protected virtual void OnPlayFile(PlayFileEventArgs pea)
        {
            if (PlayFile != null) PlayFile(this, pea);
        }

        protected virtual void OnPauseFile(PauseFileEventArgs paea)
        {
            if (PauseFile != null) PauseFile(this, paea);
        }

        protected virtual void OnStopFile(StopFileEventArgs sea)
        {
            if (StopFile != null) StopFile(this, sea);
        }

        protected virtual void OnCloseFile(CloseFileEventArgs cea)
        {
            if (CloseFile != null) CloseFile(this, cea);
        }

        protected virtual void OnError(ErrorEventArgs eea)
        {
            if (Error != null) Error(this, eea);
        }

        #endregion
    }
}