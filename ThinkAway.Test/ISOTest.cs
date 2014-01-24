using System.IO;
using System.Text;
using ThinkAway.Fat;
using ThinkAway.Iso9660;

namespace ThinkAway.Test
{
    class ISOTest
    {
        public void CreateISO()
        {
            CDBuilder builder = new CDBuilder();
            builder.UseJoliet = true;
            builder.VolumeIdentifier = "A_SAMPLE_DISK";
            builder.AddFile(@"Folder\Hello.txt", Encoding.ASCII.GetBytes("Hello World!"));
            builder.Build(@"C:\sample.iso");
        }

        public void ExtractISO()
        {
            using (FileStream isoStream = File.Open(@"C:\temp\sample.iso",FileMode.Open))
            {
                CDReader cd = new CDReader(isoStream, true);
                Stream fileStream = cd.OpenFile(@"Folder\Hello.txt", FileMode.Open);
                // Use fileStream...
            }

        }

        //public void CreateVHD()
        //{
        //    long diskSize = 30 * 1024 * 1024; //30MB
        //    using (Stream vhdStream = File.Create(@"C:\TEMP\mydisk.vhd"))
        //    {
        //        Disk disk = Disk.InitializeDynamic(vhdStream, diskSize);
        //        BiosPartitionTable.Initialize(disk, WellKnownPartitionType.WindowsFat);
        //        using (FatFileSystem fs = FatFileSystem.FormatPartition(disk, 0, null))
        //        {
        //            fs.CreateDirectory(@"TestDir\CHILD");
        //            // do other things with the file system...
        //        }
        //    }
        //}


        public void CreateVFD()
        {
            using (FileStream fs = File.Create(@"myfloppy.vfd"))
            {
                using (FatFileSystem floppy = FatFileSystem.FormatFloppy(fs, FloppyDiskType.HighDensity, "MY FLOPPY  "))
                {
                    using (Stream s = floppy.OpenFile("foo.txt", FileMode.Create))
                    {
                        // Use stream...
                    }
                }
            }
        }


    }
}
