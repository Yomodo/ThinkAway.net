using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    [Serializable,XmlRoot]
    public class DocumentProperties
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Keywords { get; set; }

        public string Category { get; set; }

        public string LastAuthor { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastSaved { get; set; }

        public double Version { get; set; }


        public DocumentProperties()
        {
            Author = "Lsong";

            Created = System.DateTime.Now;

            Version = 1.0;
        }
    }
}
