using System.Collections.Generic;
using System.Xml.Serialization;

namespace AbacusToy {
    public class PackDataXML {
    	[XmlAttribute("packName")] public string packName="undefined";
    	[XmlArray("Levels")]
    	[XmlArrayItem("Level")]
    	public List<BoardDataXML> boardDataXMLs = new List<BoardDataXML>();
    }
}