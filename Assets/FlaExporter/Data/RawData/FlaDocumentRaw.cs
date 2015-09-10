using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
   [XmlRoot("DOMDocument", Namespace = "http://ns.adobe.com/xfl/2008/")]

    public class FlaDocumentRaw
    {
        [XmlAttribute("backgroundColor")] 
        public string BackgroundColor;

        [XmlAttribute("width")] 
        public int Width;

        [XmlAttribute("height")]
        public int Height;

        [XmlAttribute("frameRate")]
        public int FrameRate;

        [XmlAttribute("currentTimeline")]
        public int CurrentTimeline;

        [XmlAttribute("xflVersion")]
        public string XflVersion;
        
        [XmlArray("symbols")]
        [XmlArrayItem("Include")]
        public List<FlaIncludeRaw> IncludeSymbols;

        [XmlArray("media")]
        [XmlArrayItem("DOMBitmapItem")]
        public List<FlaBitmapItemRaw> IncludeBitmaps;
        
        [XmlArray("timelines")] 
        [XmlArrayItem("DOMTimeline")]
        public List<FlaTimeLineRaw> Timelines;

        //<folders>
        //  <DOMFolderItem name="testFolder" itemID="55f144ae-00000007" isExpanded="true"/>
        //  <DOMFolderItem name="testFolder/testfolderInclude" itemID="55f144b8-0000000b"/>
        [XmlArray("folders")]
        [XmlArrayItem("DOMFolderItem")]
        public List<FlaFolderRaw> Folders;
        [XmlArray("persistentData")] 
        [XmlArrayItem("PD")] 
        public List<FlaPersistentDataRaw> PersistentData;

        public override string ToString()
        {
            return string.Format("width:{0}, height:{1}, frameRate:{2}, xflVersion:{3}, currentTimeline:{4}, backgroundColor:{5}",
                                    Width,Height,FrameRate,XflVersion,CurrentTimeline,BackgroundColor);
        }
    }
}
