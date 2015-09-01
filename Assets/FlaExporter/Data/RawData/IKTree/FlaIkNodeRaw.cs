using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.IKTree
{
    public class FlaIkNodeRaw
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("boneName")]
        public string BoneName;

        [XmlAttribute("location")]
        public string Location;

        [XmlAttribute("referenceID")] 
        public string ReferenceId;

        [XmlAttribute("speed")]
        public string Speed;

        [XmlAttribute("xArray")]
        public string XArray;

        [XmlAttribute("yArray")]
        public string YArray;

        [XmlAttribute("angleArray")]
        public string AngleArray;

        [XmlAttribute("nodeType")]
        public string NodeType;

        [XmlArray("childNodes")] 
        [XmlArrayItem("ChildNode")] 
        public List<FlaIkNodeRaw> ChildNodes;

        [XmlArray("states")] 
        [XmlArrayItem("State")] 
        public List<FlaIkStateRaw> States;

        [XmlElement("worldMatrix")]
        public FlaMatrixElemetRaw WorldMatrix;
    }
}