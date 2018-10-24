using System.Xml;
using System.Xml.Serialization;

namespace AbacusToy {
    public class BoardDataXML {
        //[XmlAttribute("desc")] public string desc;
        [XmlAttribute("doTilesTow")] public bool doTilesTow=true;
        [XmlAttribute("diff")] public int difficulty;
        [XmlAttribute("r")] public int devRating;
        [XmlAttribute("parMoves")] public int parMoves;
        [XmlAttribute("randGroupSize")] public int randGroupSize=2;
        [XmlAttribute("fueID")] public string fueID;
    
        [XmlAttribute("layout")] public string layout;
    }
}