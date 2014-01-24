using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.IO.Search
{
    /// <summary>
    /// 
    /// </summary>
    public class FileSearch
    {
        #region 变量


        private readonly Thread _thread;

        private readonly ProgressChangeArgs _progress;


        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string[] StartPath
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Filter
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Recursive
        {
            get;
            set;
        }
        #endregion

        #region 委托,事件


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        public delegate void OnFindFile(object sender, FileInfo file);

        /// <summary>
        /// 
        /// </summary>
        public event OnFindFile FindFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public void InvokeFindFile(FileInfo file)
        {
            OnFindFile handler = FindFile;
            if (handler != null) handler(this, file);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dir"></param>
        public delegate void OnFindFolder(object sender, DirectoryInfo dir);

        /// <summary>
        /// 
        /// </summary>
        public event OnFindFolder FindFolder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        public void InvokeFindFolder(DirectoryInfo dir)
        {
            OnFindFolder handler = FindFolder;
            if (handler != null) handler(this, dir);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void OnProgressChange(object sender, ProgressChangeArgs args);
        
        /// <summary>
        /// 
        /// </summary>
        public event OnProgressChange ProgressChange;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void InvokeProgressChange(ProgressChangeArgs args)
        {
            OnProgressChange handler = ProgressChange;
            if (handler != null) handler(this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void OnSearchCompalite(object sender, SearchCompaliteArgs args);

        /// <summary>
        /// 
        /// </summary>
        public event OnSearchCompalite SearchCompalite;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void InvokeSearchCompalite(SearchCompaliteArgs args)
        {
            _progress.Percent = 100;
            InvokeProgressChange(_progress);
            OnSearchCompalite handler = SearchCompalite;
            if (handler != null) handler(this, args);
        }
        #endregion

        #region 构造函数

        /// <summary>
        /// 
        /// </summary>
        public FileSearch()
        {
            _thread = new Thread(DoSearch);
            _progress = new ProgressChangeArgs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <param name="recursive"></param>
        public FileSearch(string[] path, string filter, bool recursive)
        {
            StartPath = path;
            Filter = filter;
            Recursive = recursive;
        }

        #endregion

        #region 析构函数

        ~FileSearch()
        {
            if (_thread.ThreadState == ThreadState.Stopped)
                return;
            _thread.Abort();
        }
        #endregion

        #region 核心函数

        /// <summary>
        /// 查找文件
        /// </summary>
        /// <param name="path"></param>
        private void FindFiles(string path)
        {
            string[] files = new string[] { };
            try
            {
                if (String.IsNullOrEmpty(Filter))
                    Filter = @"*";
                files = Directory.GetFiles(path, Filter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);

                _progress.Value += info.Length;

                double value = (double)_progress.Value / _progress.Length;

                _progress.Percent = (int)(value * 100.000);

                InvokeProgressChange(_progress);

                InvokeFindFile(info);
            }
        }
        /// <summary>
        /// 查找目录
        /// </summary>
        /// <param name="path"></param>
        private void FindFolders(string path)
        {
            string[] folders = new string[] { };
            try
            {
                folders = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            foreach (string folder in folders)
            {
                DirectoryInfo dir = new DirectoryInfo(folder);
                if (Recursive)
                {
                    DoSearch(dir.FullName);
                }
                InvokeFindFolder(dir);
            }

        }
        #endregion

        #region 计算任务长度

        void TotalSize(IEnumerable<string> list)
        {
            if (list != null)
                foreach (string path in list.Where(path => path != null))
                {
                    TotalSize(path);
                }
        }

        private void TotalSize(string path)
        {
            if (path.Length <= (@"A:\\").Length)
            {
                DriveInfo drive = new DriveInfo(path);
                long total = (drive.TotalSize - drive.AvailableFreeSpace);
                _progress.Length += total;
            }
            else
            {

                string[] files = new string[] { };
                try
                {
                    if (String.IsNullOrEmpty(Filter))
                        Filter = @"*";
                    files = Directory.GetFiles(path, Filter);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                foreach (FileInfo info in files.Select(file => new FileInfo(file)))
                {
                    _progress.Length += info.Length;
                }
                string[] folders = new string[] { };
                try
                {
                    folders = Directory.GetDirectories(path);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                foreach (string folder in folders)
                {
                    if (Recursive)
                    {
                        TotalSize(folder);
                    }
                }
            }
        }
        #endregion

        #region 执行搜索任务

        private void DoSearch()
        {
            if (StartPath != null)
            {
                TotalSize(StartPath);
                foreach (string path in StartPath)
                {
                    if (path != null) 
                        DoSearch(path);
                }
            }
            SearchCompaliteArgs args = new SearchCompaliteArgs();
            InvokeSearchCompalite(args);
        }
        private void DoSearch(string path)
        {
            FindFiles(path);
            FindFolders(path);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (_thread == null || _thread.ThreadState == ThreadState.Running)
                return;
            _thread.Start();
        }
        #endregion

        #region 任务控制
        /// <summary>
        /// 
        /// </summary>
        public void Suppend()
        {
            if (_thread.ThreadState == ThreadState.Stopped)
                return;
            #pragma warning disable 612,618
            _thread.Suspend();
            #pragma warning restore 612,618
        }
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (_thread.ThreadState == ThreadState.Stopped)
                return;
            _thread.Abort();
        }
        #endregion
    }
}
