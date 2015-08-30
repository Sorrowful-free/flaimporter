using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    [XmlRoot]
    public class FlaDocumentRaw
    {
        [XmlArray("symbols")]
        [XmlArrayItem("Include")]
        public List<FlaIncludeRaw> IncludeSymbols;

        [XmlArray("timelines")] 
        [XmlArrayItem("DOMTimeline")]
        public List<FlaTimeLineRaw> Timelines;

        [XmlArray("persistentData")] 
        [XmlArrayItem("PD")] 
        public List<FlaPersistentDataRaw> PersistentData;

        //public List<FlaPublishingHistoryRaw> History;
        //public List<FlaPrinterSettingsRaw> PrinterSettings;

    }
}
