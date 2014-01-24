using System;
using System.IO;

namespace ThinkAway.IO
{
    public class FileSplit
    {
        /// <summary>
        /// Max read data size
        /// </summary>
        private readonly int _maxReadSize = 1000000;

        public FileSplit()
        {
        
        }
        public FileSplit(int maxReadSize)
        {
            _maxReadSize = maxReadSize;
        }
        

        #region 


        public event EventHandler<SplitEventArgs> SplitStateChange;

        public void OnSplitStateChange(SplitEventArgs e)
        {
            EventHandler<SplitEventArgs> handler = SplitStateChange;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<MergeEventArgs> MergeStateChange;

        public void OnMergeStateChange(MergeEventArgs e)
        {
            EventHandler<MergeEventArgs> handler = MergeStateChange;
            if (handler != null) handler(this, e);
        }

        public class SplitEventArgs : EventArgs
        {
            public long Length;
            public int Percent;
            public string FileName;
        }
        public class MergeEventArgs : EventArgs
        {
            public long Length;
            public int Percent;
            public string FileName;
        }
        #endregion

        #region 
        public string[] DoSplit(string file, string path, long size)
        {
            SplitEventArgs arg = new SplitEventArgs();
            //以文件的全路对应的字符串和文件打开模式来初始化FileStream文件流实例
            string[] files;
            using (FileStream splitFileStream = new FileStream(file, FileMode.Open))
            {
                long length = 0;
                using (BinaryReader splitFileReader = new BinaryReader(splitFileStream))
                {
                    //获得分割后文件的个数
                    int fileCount = (int)(splitFileStream.Length / size);
                    if (splitFileStream.Length % size != 0) fileCount++;
                    files = new string[fileCount];
                    //循环将大文件分割成多个小文件
                    for (int i = 1; i <= fileCount; i++)
                    {
                        //小文件名
                        string filename = Path.GetFileName(file);
                        //确定小文件的文件名称
                        string tempFileName = Path.Combine(path, String.Concat(filename, ".fsp", i));
                        files[i - 1] = tempFileName;
                        //根据文件名称和文件打开模式来初始化FileStream文件流实例
                        FileStream tempStream = new FileStream(tempFileName, FileMode.OpenOrCreate);
                        //以FileStream实例来创建、初始化BinaryWriter书写器实例
                        BinaryWriter tempWriter = new BinaryWriter(tempStream);
                        //如果读取的数据大于限定的最大缓冲值 强制设置缓冲大小
                        int readLength = (int) (size > _maxReadSize ? _maxReadSize : size);
                        //如果尺寸大于限定 分段写入
                        int readCount = (int) (size > _maxReadSize ? size / _maxReadSize : 1);
                        //获得分段数
                        readCount = size > _maxReadSize && size % _maxReadSize != 0 ? readCount + 1 : readCount;
                        //写入分段
                        for (int j = 0; j < readCount;j++)
                        {
                            readLength = (int) (size - tempStream.Length > _maxReadSize ? readLength :size - tempStream.Length);
                            //从大文件中读取指定大小数据
                            byte[] tempBytes = splitFileReader.ReadBytes(readLength);
                            //把此数据写入小文件
                            tempWriter.Write(tempBytes);
                            //关闭书写器，形成小文件
                            arg.FileName = tempFileName;
                            arg.Length = tempStream.Length;
                            arg.Percent = (int) (((double) (tempStream.Length + length)/splitFileStream.Length)*100);
                            OnSplitStateChange(arg);
                        }
                        length += tempStream.Length;
                        tempWriter.Close();
                        //关闭文件流
                        tempStream.Close();
                    }
                    splitFileReader.Close();
                }
                //关闭大文件阅读器
                splitFileStream.Close();
            }
            return files;
        }
        public void DoMerge(string[] arrFileNames, string saveFile)
        {
            MergeEventArgs arg = new MergeEventArgs();
            //以合并后的文件名称和打开方式来创建、初始化FileStream文件流
            FileStream addStream = new FileStream(saveFile, FileMode.Create);
            //以FileStream文件流来初始化BinaryWriter书写器，此用以合并分割的文件
            long size = 0;
            using (BinaryWriter addWriter = new BinaryWriter(addStream))
            {
                foreach(string file in arrFileNames)
                {
                    FileInfo info = new FileInfo(file);
                    size += info.Length;
                }
                for (int index = 0; index < arrFileNames.Length; index++)
                {
                    string file = arrFileNames[index];
                    // ReSharper disable AssignNullToNotNullAttribute
                    file = Path.Combine(Path.GetDirectoryName(file),
                                        String.Concat(Path.GetFileNameWithoutExtension(file), ".fsp", index + 1));
                    // ReSharper restore AssignNullToNotNullAttribute
                    //以小文件所对应的文件名称和打开模式来初始化FileStream文件流，起读取分割作用
                    FileStream tempStream = new FileStream(file, FileMode.Open);
                    //用FileStream文件流来初始化BinaryReader文件阅读器，也起读取分割文件作用
                    BinaryReader tempReader = new BinaryReader(tempStream);
                    int readLength = (int)(tempStream.Length > _maxReadSize ? _maxReadSize : tempStream.Length);
                    int readCount = (int) (tempStream.Length > _maxReadSize && tempStream.Length%_maxReadSize != 0
                                               ? (tempStream.Length/_maxReadSize) + 1
                                               : tempStream.Length/_maxReadSize);
                    readCount = tempStream.Length > _maxReadSize ? readCount : 1;
                    for (int i = 0; i < readCount; i++)
                    {
                        
                        //读取分割文件中的数据，并生成合并后文件
                        addWriter.Write(tempReader.ReadBytes(readLength));

                        arg.FileName = file;
                        arg.Percent = (int) (((double) addStream.Length/size)*100);
                        arg.Length = tempStream.Length;
                        OnMergeStateChange(arg);
                    }
                    //关闭BinaryReader文件阅读器
                    tempReader.Close();
                    //关闭FileStream文件流
                    tempStream.Close();
                }
                //关闭BinaryWriter文件书写器
                addWriter.Close();
            }
            //关闭FileStream文件流
            addStream.Close();
        }
        #endregion

    }
}