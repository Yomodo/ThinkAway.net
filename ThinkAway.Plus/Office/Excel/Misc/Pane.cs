using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel.Misc
{
    [Serializable, XmlRoot]
    public class Pane
    {
        public int Number { get; set; }
        public int ActiveRow { get; set; }
        public int ActiveCol { get; set; }
    }
    [Serializable, XmlRoot]
    public class PaneCollection
    {
        [XmlElement(ElementName = "Pane")]
        public List<Pane> Panes { get; set; }

        public PaneCollection()
        {
            Panes = new List<Pane>();
        }
    }
}
