using System.Xml;
using System.Xml.Serialization;

namespace AbacusToy {
    public class BoardDataXML {
        //[XmlAttribute("desc")] public string desc;
        [XmlAttribute("diff")] public int difficulty;
        [XmlAttribute("r")] public int devRating;
        [XmlAttribute("parMoves")] public int parMoves;
        [XmlAttribute("fueID")] public string fueID;
    
        [XmlAttribute("layout")] public string layout;
    }
}