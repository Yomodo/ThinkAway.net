using System;
using System.Globalization;
using System.Text;
using System.IO;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Media.Tag
{
    /// <summary>
    /// 获取MP3文件的ID3 V1版本的TAG信息的类
    /// </summary>
    internal class Mp3TagId3V1
    {
        //流派分类，共有148种，我只列举了前21种，大家可以自己补充
        private readonly string[] _genre = {
                                              "Blues",
                                              "Classic Rock",
                                              "Country",
                                              "Dance",
                                              "Disco",
                                              "Funk",
                                              "Grunge",
                                              "Hip-Hop",
                                              "Jazz",
                                              "Metal",
                                              "New Age",
                                              "Oldies",
                                              "Other",
                                              "Pop",
                                              "R&B",
                                              "Rap",
                                              "Reggae",
                                              "Rock",
                                              "Techno",
                                              "Industrial",
                                              "Alternative"
                                          };

        private readonly string _title = string.Empty;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }
        }

        private readonly string _artist = string.Empty;
        /// <summary>
        /// 艺术家，演唱者
        /// </summary>
        public string Artist
        {
            get { return _artist; }
        }

        private readonly string _album = string.Empty;
        /// <summary>
        /// 所属专辑
        /// </summary>
        public string Album
        {
            get { return _album; }
        }

        private readonly string _pubYear = string.Empty;
        /// <summary>
        /// 发行年份
        /// </summary>
        public string PublishYear
        {
            get { return _pubYear; }
        }

        private readonly string _comment = string.Empty;
        /// <summary>
        /// 备注、说明
        /// </summary>
        public string Comment
        {
            get
            {
                if (_comment.Length == 30)
                {
                    //如果是 ID3 V1.1的版本，那么comment只占前28个byte，第30个byte存放音轨信息
                    if (TagVersion(_comment)) return _comment.Substring(0, 28).TrimEnd();
                }

                return _comment.TrimEnd();
            }
        }

        /// <summary>
        /// 音轨
        /// </summary>
        public string Track
        {
            get
            {
                if (_comment.Length == 30)
                {
                    //如果是 ID3 V1.1的版本，读取音轨信息
                    if (TagVersion(_comment)) return ((int)_comment[29]).ToString(CultureInfo.InvariantCulture);
                }

                return string.Empty;
            }
        }

        private readonly string genre;
        /// <summary>
        /// 流派
        /// </summary>
        public string Genre
        {
            get { return genre; }
        }

        /// <summary>
        /// 判断MP3的TAG信息的版本，是V1.0 还是 V1.1
        /// </summary>
        /// <param name="comment"></param>
        /// <returns>true表示是 1.1，false表示是 1.0</returns>
        private bool TagVersion(string comment)
        {
            return comment[28].Equals('\0') && comment[29] > 0 || comment[28] == 32 && comment[29] != 32;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mp3FilePath">MP3文件的完整路径</param>
        public Mp3TagId3V1(string mp3FilePath)
        {
            byte[] tagBody = new byte[128];

            if (!File.Exists(mp3FilePath))
                throw new FileNotFoundException("指定的MP3文件不存在！", mp3FilePath);

            //读取MP3文件的最后128个字节的内容
            using (FileStream fs = new FileStream(mp3FilePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(-128, SeekOrigin.End);
                fs.Read(tagBody, 0, 128);
                fs.Close();
            }
            Encoding encoding = Encoding.GetEncoding("GB2312");
            //取TAG段的前三个字节
            string tagFlag = encoding.GetString(tagBody, 0, 3);

            //如果没有TAG信息，则直接返回
            if (!"TAG".Equals(tagFlag, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
                //throw new InvalidDataException("指定的MP3文件没有TAG信息！");
            }

            //按照MP3 ID3 V1 的tag定义，依次读取相关的信息
            this._title = encoding.GetString(tagBody, 3, 30).TrimEnd();
            this._artist = encoding.GetString(tagBody, 33, 30).TrimEnd();
            this._album = encoding.GetString(tagBody, 62, 30).TrimEnd();
            this._pubYear = encoding.GetString(tagBody, 93, 4).TrimEnd();
            this._comment = encoding.GetString(tagBody, 97, 30);
            Int16 g = tagBody[127];
            this.genre = g >= _genre.Length ? "未知" : _genre[g];
        }
    }
}
