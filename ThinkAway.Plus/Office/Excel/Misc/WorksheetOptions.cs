using System;
using System.Xml.Serialization;
using ThinkAway.Plus.Office.Excel.Misc;

namespace ThinkAway.Plus.Office.Excel
{
    [Serializable, XmlRoot]
    public class WorksheetOptions
    {
        public object PageSetup { get; set; }

        public object Unsynced { get; set; }

        public PaneCollection Panes { get; set; }

        public bool ProtectObjects { get; set; }

        public bool ProtectScenarios { get; set; }

        public WorksheetOptions()
        {
            Panes = new PaneCollection();
        }


    }
}
