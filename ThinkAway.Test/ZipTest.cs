using System.Collections.Generic;
using System.IO;
using ThinkAway.IO.ZipLib.Zip;

namespace ThinkAway.Test
{
    class ZipTest
    {
        private static void ZipStream(IEnumerable<string> files, string zipfile)
        {
            FileStream fileStream = new FileStream(zipfile, FileMode.Create);
            using (ZipOutputStream zipOutputStream = new ZipOutputStream(fileStream))
            {
                foreach (string file in files)
                {
                    zipOutputStream.PutNextEntry(new ZipEntry(Path.GetFileName(file)));
                    FileStream stream = new FileStream(file, FileMode.Open);
                    byte[] buffer = new byte[1024];
                    int len;
                    while ((len = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        zipOutputStream.Write(buffer, 0, len);
                    }
                    stream.Close();
                    stream.Dispose();
                }
                zipOutputStream.Flush();
                zipOutputStream.Close();
                zipOutputStream.Dispose();
            }
        }
    }
}
