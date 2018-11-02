using System.Xml;
using System.Xml.Serialization;

namespace SlideAndStick {
    public class BoardDataXML {
        //[XmlAttribute("desc")] public string desc;
        [XmlAttribute("noWin")] public bool debug_noWin=false;
        [XmlAttribute("diff")] public int difficulty;
        [XmlAttribute("r")] public int devRating;
        [XmlAttribute("fueID")] public string fueID;
    
        [XmlAttribute("layout")] public string layout;
    }
}