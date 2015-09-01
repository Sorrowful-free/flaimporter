using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.IKTree
{
    public class FlaIKTreeRaw
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("treeName")]
        public string TreeName;

        [XmlAttribute("physicsEnabled")] 
        public bool PhysicsEnabled;

        [XmlElement("IKNode")]
        public FlaIkNodeRaw IKNode;

        [XmlAttribute("connectionIndices")] 
        public string ConnectionIndices;

        [XmlArray("states")]
        [XmlArrayItem("IKState")]
        public List<FlaIkStateRaw> States;

        [XmlElement("IKContour")] 
        public FlaIkContourRaw Constour;
    }
}
