using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ThinkAway.IO;

namespace ThinkAway.Test
{
    class FileSearchTest
    {

        [Test]
        public void Search()
        {
            FileSearch fileSearch = new FileSearch();

            fileSearch.SearchPath = new List<string>
                                      {
                                          "C:\\",
                                          "D:\\"
                                      };

            //fileSearch.Compalite += (x, y) => ZipStream(fileSearch.FileList.ConvertAll(z => z.FullName));

            //fileSearch.Recursive = true;

            //fileSearch.FindFile += fileSearch_FindFile;
            //fileSearch.FindFolder += fileSearch_FindFolder;
            //fileSearch.Exception += fileSearch_Exception;
            //fileSearch.Compalite += fileSearch_Compalite;

            fileSearch.Filters.CheckFileSize = true;
            //fileSearch.Filters.CheckFileSizeLeast = 102400;
            fileSearch.Filters.CheckFileSizeMost = 102400;

            //fileSearch.Filters.FileDateMode = new int();
            //fileSearch.Filters.FileDateAfter = true;
            //fileSearch.Filters.FileDateBefore = true;
            //fileSearch.Filters.DateTimeAfter = new DateTime();
            //fileSearch.Filters.DateTimeBefore = new DateTime();

            fileSearch.Filters.ExcludedFolders = new List<string>();
            fileSearch.Filters.ExcludedFileTypes = new List<string> { "*.img" };
            //
            fileSearch.Filters.IncludedFolders = new List<string>();
            fileSearch.Filters.IncludedFileType = new List<string> { "*" };
            //
            fileSearch.Filters.IgnoreReadOnly = true;
            fileSearch.Filters.IgnoreZeroByte = true;
            fileSearch.Filters.FileAttributes = FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.System;


            fileSearch.Start();

            //string zipFileName = Path.Combine(savePath, string.Concat(zipName, ".zip"));
        }
    }
}
