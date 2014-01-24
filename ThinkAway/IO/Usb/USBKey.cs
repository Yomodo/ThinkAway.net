using System;
using System.Collections.Generic;
using System.Text;
using ThinkAway.Core;

namespace ThinkAway.IO.Usb
{
    public class USBKey
    {
        public static USBKey GetDevice(int start, string hashFlag)
        {
            USBKey usbKey = null;
            StringBuilder devicePath = new StringBuilder("", 260);
            if (Win32API.FindPort_2(start, hashFlag, devicePath) == 0)
            {
                usbKey = new USBKey(devicePath.ToString());
            }
            return usbKey;
        }

        public static USBKey GetDevice(int start)
        {
            USBKey usbKey = null;
            StringBuilder outDevicePath = new StringBuilder("", 260);
            if (Win32API.FindPort(start, outDevicePath) == 0)
            {
                usbKey = new USBKey(outDevicePath.ToString());
            }
            return usbKey;
        }

        public static USBKey[] GetDevice()
        {
            int index = 0;
            List<USBKey> list = new List<USBKey>();
            USBKey usbKey;
            while ((usbKey = GetDevice(index)) != null)
            {
                list.Add(usbKey);
                index++;
            }
            return list.ToArray();
        }

        public string GetIDVersion(ref short version)
        {
            StringBuilder id = new StringBuilder(100);
            Win32API.GetIDVersion(id, ref version, DevicePath);
            return id.ToString();
        }

        public readonly String DevicePath;

        private string _hkey = "ffffffff";
        private string _lkey = "ffffffff";

        public string Hkey
        {
            get { return _hkey; }
            set { _hkey = value; }

        }
        public string Lkey
        {
            get { return _lkey; }
            set { _lkey = value; }
        }

        public USBKey(string devicePath)
        {
            this.DevicePath = devicePath;
        }
        /// <summary>
        /// 获取或设置属性 Name 的值
        /// </summary>
        public string Name
        {
            get
            {
                StringBuilder outstring = new StringBuilder();
                Win32API.GetName(outstring, Hkey, Lkey, DevicePath);
                return outstring.ToString();
            }
            set
            {
                Win32API.SetName(value, Hkey, Lkey, DevicePath);
            }
        }

        /// <summary>
        /// PWD
        /// </summary>
        public string Password
        {
            get
            {
                StringBuilder outstring = new StringBuilder();
                Win32API.GetPWD(outstring, Hkey, Lkey, DevicePath);
                return outstring.ToString();
            }
            set
            {
                Win32API.SetPWD(value, Hkey, Lkey, DevicePath);
            }
        }

        /// <summary>
        /// 设置特征码到锁中
        /// </summary>
        public string Feature
        {
            set
            {
                Win32API.SetFeature(value, Hkey, Lkey, DevicePath);
            }
        }

        /// <summary>
        /// 设置U盘是否只读模式
        /// </summary>
        public bool IsReadOnly
        {
            set { Win32API.SetReadOnly(value, DevicePath); }
        }

        /// <summary>
        /// HASH开发商标志
        /// </summary>
        public string Sha1Flag1
        {
            get
            {
                StringBuilder outstring = new StringBuilder();
                Win32API.Sha1Flag(outstring, DevicePath);
                return outstring.ToString();
            }
            set
            {
                Win32API.SetFlag(value, DevicePath);
            }
        }

        /// <summary>
        /// HASH用户数据
        /// ID->特征区数据->用户名->用户登录密码->inHashData
        /// </summary>
        /// <param name="hashData"></param>
        /// <returns></returns>
        public string Sha1Data(string hashData)
        {
            StringBuilder outHashData = new StringBuilder();
            Win32API.Sha1Data(hashData, outHashData, DevicePath);
            return outHashData.ToString();
        }

        /// <summary>
        /// 从加密锁中读取一批字节
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] Read(int pos, int length)
        {
            byte[] outData = new byte[1024];
            Win32API.YRead(pos, length, Hkey, Lkey, outData, DevicePath);
            return outData;
        }

        /// <summary>
        /// 从加密锁中读一个长整形
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public long Read(int pos)
        {
            int outData = 0;
            Win32API.YReadLong(ref outData, pos, Hkey, Lkey, DevicePath);
            return outData;
        }

        /// <summary>
        /// 从加密锁中读字符串
        /// </summary>
        /// <param name="address"></param>
        /// <param name="mylen"></param>
        /// <returns></returns>
        public int ReadString(int address, int mylen)
        {
            StringBuilder outstring = new StringBuilder();
            return Win32API.YReadString(outstring, address, mylen, Hkey, Lkey, DevicePath);
        }

        /// <summary>
        /// 写一批字节到加密锁中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int Write(byte[] data, int pos)
        {
            return Win32API.YWrite(data, pos, data.Length, Hkey, Lkey, DevicePath);
        }

        /// <summary>
        /// 写一个字符串到锁中
        /// </summary>
        /// <param name="str"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public int Write(string str, int address)
        {
            return Win32API.YWriteString(str, address, Hkey, Lkey, DevicePath);
        }

        /// <summary>
        /// 写一个长整形到锁中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public int Write(int data, int address)
        {
            return Win32API.YWriteLong(data, address, Hkey, Lkey, DevicePath);
        }

        /// <summary>
        /// 初始化加密锁
        ///  </summary>
        /// <returns></returns>
        public int Reset()
        {
            return Win32API.ReSet(DevicePath);
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="lKey"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public int SetKey(string hKey, string lKey, int flag)
        {
            int password = Win32API.SetPassword(Hkey, Lkey, hKey, lKey, flag, DevicePath);
            Hkey = hKey;
            Lkey = lKey;
            return password;
        }

        /// <summary>
        /// 打开PIN
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public int OpenPin(int pin)
        {
            return Win32API.OpenPin(pin, DevicePath);
        }

        /// <summary>
        /// 设置PIN
        /// </summary>
        /// <param name="oldPin"></param>
        /// <param name="newPin"></param>
        /// <returns></returns>
        public int SetPin(int oldPin, int newPin)
        {
            return Win32API.SetPin(oldPin, newPin, DevicePath);
        }
        /// <summary>
        /// 设置Pin模式
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="inMode"></param>
        /// <returns></returns>
        public int SetPinMode(int pin, int inMode)
        {
            return Win32API.SetPinMode(pin, inMode, DevicePath);
        }

        /// <summary>
        /// 返回错误码相关信息
        /// </summary>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public string GetKeyErrorInfo(int errCode)
        {
            StringBuilder outErrInfo = new StringBuilder();
            Win32API.GetKeyErrorInfo(errCode, outErrInfo);
            return outErrInfo.ToString();
        }
    }
}
