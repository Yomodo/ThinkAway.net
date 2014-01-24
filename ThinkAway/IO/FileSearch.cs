using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.IO
{
    /// <summary>
    /// <c>FileSearch</c>提供文件搜索的能力
    /// </summary>
    public class FileSearch
    {
        
        /// <summary>
        /// 任务是否在持续运行
        /// </summary>
        private bool Running { get; set; }

        /// <summary>
        /// 任务主线程
        /// </summary>
        private Thread ThreadMain { get; set; }

        /// <summary>
        /// 是否递归搜素子目录
        /// </summary>
        public bool Recursive { get; set; }
        
        /// <summary>
        /// 搜索路径
        /// </summary>
        public List<string> SearchPath { get; set; }

        /// <summary>
        /// 搜索到的文件列表
        /// </summary>
        public List<FileInfo> FileList { get; set; }

        /// <summary>
        /// 文件搜索过滤器
        /// </summary>
        public SearchFilter Filters { get; set; }

        /// <summary>
        /// 当前处理的文件
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        /// 找到文件时发生
        /// </summary>
        public event EventHandler<FindFileArgs> FindFile;

        protected void OnFindFile(FindFileArgs e)
        {
            EventHandler<FindFileArgs> handler = FindFile;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 找到文件夹时发生
        /// </summary>
        public event EventHandler<FindFolderArgs> FindFolder;

        protected void OnFindFolder(FindFolderArgs e)
        {
            EventHandler<FindFolderArgs> handler = FindFolder;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 文件搜索完成时发生
        /// </summary>
        public event EventHandler<SearchCompaliteArgs> Compalite;

        protected void OnCompalite(SearchCompaliteArgs e)
        {
            EventHandler<SearchCompaliteArgs> handler = Compalite;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 发生异常时调用
        /// </summary>
        public event Action<Exception> Exception;

        protected void OnException(Exception exception)
        {
            Action<Exception> handler = Exception;
            if (handler != null) handler(exception);
        }

        /// <summary>
        /// 使用默认构造初始化<c>FileSearch</c>实例
        /// </summary>
        public FileSearch()
        {
            SearchPath = new List<string>();
             
            Filters = new SearchFilter();

            FileList = new List<FileInfo>();

            this.ThreadMain = new Thread(Run);
        }
        /// <summary>
        /// 开始检索目录
        /// </summary>
        public void Start()
        {
            Running = true;
            this.ThreadMain.Start();
        }
        /// <summary>
        /// 停止 <c>FileSearch</c> 的任务
        /// </summary>
        public void Stop()
        {
            Running = false;
        }
        /// <summary>
        /// <c>FileSearch</c>.Run();
        /// </summary>
        private void Run()
        {
            DirectoryInfo directoryInfo;
            foreach (string path in SearchPath)
            {
                if (!Running)
                    return;
                directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                {
                    ScanFiles(directoryInfo);
                }
            }
            OnCompalite(new SearchCompaliteArgs(FileList));
        }
        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="parentInfo"></param>
        private void ScanFiles(DirectoryInfo parentInfo)
        {
            FileInfo[] fileInfos = new FileInfo[] { };
            try
            {
                fileInfos = parentInfo.GetFiles();
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!Running)
                    return;
                CurrentFile = fileInfo.FullName;

                // Check if file is exclude
                if (FileTypeIsExcluded(fileInfo.Name))
                    continue;

                if (!FileTypeIsIncluded(fileInfo.Name))
                    continue;

                // Check file attributes
                if (!FileCheckAttributes(fileInfo))
                    continue;

                // Check file dates
                if (!FileCheckDate(fileInfo))
                    continue;

                // Check file size
                if (!FileCheckSize(fileInfo))
                    continue;
                //添加到文件列表
                FileList.Add(fileInfo);

                OnFindFile(new FindFileArgs(fileInfo));
            }

            //判断是否递归子目录
            if (!this.Recursive)
                return;

            DirectoryInfo[] directoryInfos = new DirectoryInfo[] { };
            try
            {
                directoryInfos = parentInfo.GetDirectories();
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
            foreach (DirectoryInfo childInfo in directoryInfos)
            {
                if (!Running)
                    return;
                OnFindFolder(new FindFolderArgs(childInfo));
                //贪婪模式 , 包含的永远高于排除的
                if (FolderIsIncluded(childInfo.FullName))
                {
                    ScanFiles(childInfo);
                    //这里要继续 , 因为已经包含 , 防止在排除列表中重复
                    continue;
                }
                //搜索不被排除的
                if (!FolderIsExcluded(childInfo.FullName))
                    ScanFiles(childInfo);
            }
        }
        /// <summary>
        /// 是否匹配文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool FileTypeIsIncluded(string fileName)
        {
            foreach (string filter in Filters.IncludedFileType)
            {
                // Check if file matches types
                if (FileHelper.CompareWildcards(fileName, filter))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查文件大小
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool FileCheckSize(FileInfo fileInfo)
        {
            if (Filters.CheckFileSize)
            {
                long fileSize = fileInfo.Length / 1024;
                // Check for zero-byte files
                if (Filters.IgnoreZeroByte && Equals(fileInfo.Length, 0))
                    return false;

                if (Filters.CheckFileSizeLeast > 0)
                    if (fileSize <= Filters.CheckFileSizeLeast)
                        return false;

                if (Filters.CheckFileSizeMost > 0)
                    if (fileSize >= Filters.CheckFileSizeMost)
                        return false;
            }
            //贪婪模式
            return true;
        }

        /// <summary>
        /// 检查文件日期
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool FileCheckDate(FileInfo fileInfo)
        {
            bool result = false;
            DateTime dateTimeFile = DateTime.MinValue;
            if (!Filters.FileDateAfter && !Filters.FileDateBefore)
            {
                return true;
            }
            switch (Filters.FileDateMode)
            {
                case MyEnum.CreationTime:
                    dateTimeFile = fileInfo.CreationTime;
                    break;
                case MyEnum.LastAccessTime:
                    dateTimeFile = fileInfo.LastWriteTime;
                    break;
                case MyEnum.LastWriteTime:
                    dateTimeFile = fileInfo.LastAccessTime;
                    break;
            }
            if (Filters.FileDateAfter)
            {
                if (DateTime.Compare(dateTimeFile, Filters.DateTimeAfter) >= 0)
                    result = true;
            }
            if (Filters.FileDateBefore)
            {
                if (DateTime.Compare(dateTimeFile, Filters.DateTimeBefore) <= 0)
                    result = true;
            }
            return result;
        }

        /// <summary>
        /// Checks file attributes to match what user specified to search for
        /// </summary>
        /// <param name="fileInfo">File Information</param>
        /// <returns>True if file matches attributes</returns>
        private bool FileCheckAttributes(FileInfo fileInfo)
        {
            if (!Filters.CheckAttributes)
                return true;
            // Check if file is in use or write protected
            if (Filters.IgnoreReadOnly && fileInfo.IsReadOnly)
                return false;
            FileAttributes attributes = fileInfo.Attributes | Filters.FileAttributes;
            return Filters.FileAttributes == attributes;
        }
        /// <summary>
        /// 是否包含文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private bool FolderIsIncluded(string dirPath)
        {
            foreach (string includeDir in Filters.IncludedFolders)
            {
                if (System.String.CompareOrdinal(includeDir, dirPath) == 0 || FileHelper.CompareWildcard(dirPath, includeDir))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查是否排除文件目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private bool FolderIsExcluded(string dirPath)
        {
            foreach (string excludeDir in Filters.ExcludedFolders)
            {
                if (FileHelper.CompareWildcard(dirPath, excludeDir))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查文件类型是否被排除 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool FileTypeIsExcluded(string fileName)
        {
            foreach (string excludeFileType in Filters.ExcludedFileTypes)
            {
                if (FileHelper.CompareWildcard(fileName, excludeFileType))
                    return true;
            }
            return false;
        }

        public class SearchFilter
        {
            #region 文件类型过滤

            private List<string> _searchFilters;
            /// <summary>
            /// 文件过滤器
            /// </summary>
            public List<string> IncludedFileType
            {
                get { return _searchFilters; }
                set { _searchFilters = value; }
            }

            private List<string> _excludedFileTypes;
            /// <summary>
            /// 排除的文件类型
            /// </summary>
            public List<string> ExcludedFileTypes
            {
                get { return _excludedFileTypes; }
                set { _excludedFileTypes = value; }
            }

            #endregion

            #region 文件尺寸匹配

            private bool _checkFileSize;
            /// <summary>
            /// 检查文件大小
            /// </summary>
            public bool CheckFileSize
            {
                get { return _checkFileSize; }
                set { _checkFileSize = value; }
            }

            private long _checkFileSizeLeast;
            /// <summary>
            /// 最少
            /// </summary>
            public long CheckFileSizeLeast
            {
                get { return _checkFileSizeLeast; }
                set { _checkFileSizeLeast = value; }
            }

            private long _checkFileSizeMost;
            /// <summary>
            /// 最多
            /// </summary>
            public long CheckFileSizeMost
            {
                get { return _checkFileSizeMost; }
                set { _checkFileSizeMost = value; }
            }

            #endregion

            #region 时间匹配

            private MyEnum _findFilesMode;
            /// <summary>
            /// 时间匹配模式
            /// </summary>
            public MyEnum FileDateMode
            {
                get { return _findFilesMode; }
                set { _findFilesMode = value; }
            }

            private bool _findFilesAfter;
            /// <summary>
            /// 匹配时间之前
            /// </summary>
            public bool FileDateAfter
            {
                get { return _findFilesAfter; }
                set { _findFilesAfter = value; }
            }

            private bool _findFilesBefore;
            /// <summary>
            /// 匹配时间之后
            /// </summary>
            public bool FileDateBefore
            {
                get { return _findFilesBefore; }
                set { _findFilesBefore = value; }
            }
            private DateTime _dateTimeAfter;
            /// <summary>
            /// 时间之前
            /// </summary>
            public DateTime DateTimeAfter
            {
                get { return _dateTimeAfter; }
                set { _dateTimeAfter = value; }
            }

            private DateTime _dateTimeBefore;
            /// <summary>
            /// 时间之后
            /// </summary>
            public DateTime DateTimeBefore
            {
                get { return _dateTimeBefore; }
                set { _dateTimeBefore = value; }
            }

            #endregion

            #region 目录匹配

            private List<string> _includedFolders;
            /// <summary>
            /// 包含的的文件夹
            /// </summary>
            public List<string> IncludedFolders
            {
                get { return _includedFolders; }
                set { _includedFolders = value; }
            }

            private List<string> _excludedDirs;
            /// <summary>
            /// 排除的目录
            /// </summary>
            public List<string> ExcludedFolders
            {
                get { return _excludedDirs; }
                set { _excludedDirs = value; }
            }

            #endregion

            #region 文件属性

            public bool CheckAttributes { get; set; }

            private FileAttributes _fileAttributes;
            /// <summary>
            /// 文件属性
            /// </summary>
            public FileAttributes FileAttributes
            {
                get { return _fileAttributes; }
                set { _fileAttributes = value; }
            }
            private bool _searchZeroByte;
            /// <summary>
            /// 忽略零字节文件
            /// </summary>
            public bool IgnoreZeroByte
            {
                get { return _searchZeroByte; }
                set { _searchZeroByte = value; }
            }

            private bool _searchReadOnly;
            /// <summary>
            /// 忽略只读的文件
            /// </summary>
            public bool IgnoreReadOnly
            {
                get { return _searchReadOnly; }
                set { _searchReadOnly = value; }
            }

            #endregion
            
            internal SearchFilter()
            {
                _searchFilters = new List<string>();
                _excludedFileTypes = new List<string>();

                _includedFolders = new List<string>();
                _excludedDirs = new List<string>();
            }
        }
    }

    public enum MyEnum
    {
        CreationTime,
        LastWriteTime,
        LastAccessTime,
    }

    /// <summary>
    /// 文件搜索进度发生改变
    /// </summary>
    public class ProgressChangeArgs : EventArgs
    {
        /// <summary>
        /// %
        /// </summary>
        public int Percent;
        /// <summary>
        /// Value
        /// </summary>
        public long Value;
        /// <summary>
        /// Length
        /// </summary>
        public long Length;
    }

    /// <summary>
    /// SearchCompaliteArgs
    /// </summary>
    public class SearchCompaliteArgs : EventArgs
    {
        public List<FileInfo> FileList;

        public SearchCompaliteArgs(List<FileInfo> fileList)
        {
            // TODO: Complete member initialization
            this.FileList = fileList;
        }

    }

    public class FindFileArgs : EventArgs
    {
        public FileInfo Info;

        public FindFileArgs(FileInfo info)
        {
            // TODO: Complete member initialization
            this.Info = info;
        }
    }

    public class FindFolderArgs : EventArgs
    {
        public DirectoryInfo Dir;

        public FindFolderArgs(DirectoryInfo dir)
        {
            // TODO: Complete member initialization
            this.Dir = dir;
        }
    }
}
