using System.Runtime.InteropServices;
using System.Text;

namespace ThinkAway.Config
{
    /// <summary>
    /// ini文件操作类
    /// </summary>
    public class IniConfig
    {
        /// <summary>
        /// ini文件路径
        /// </summary>
        public string Inipath;
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iniPath">文件路径</param>
        public IniConfig(string iniPath)
        {
            Inipath = iniPath;
        }
        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.Inipath);
        }

        /// <summary>
        /// 读出INI文件
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue"></param>
        public string IniReadValue(string section, string key,string defaultValue)
        {
            StringBuilder temp = new StringBuilder(500);
            GetPrivateProfileString(section, key, defaultValue, temp, 500, this.Inipath);
            return temp.ToString();
        }
    }
}