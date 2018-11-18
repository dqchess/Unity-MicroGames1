using System.Collections.Generic;
using System.Xml.Serialization;

namespace SpoolOut {
    [XmlRoot("LevelsCollection")]
    public class PackCollectionDataXML {
    	[XmlAttribute("collectionName")] public string collectionName;
    	//[XmlAttribute("gameMode")] public string gameMode; // Match exactly the names of the modes in GameMode.cs.
    	[XmlArray("Packs")]
    	[XmlArrayItem("LevelPack")]
    	public List<PackDataXML> packDataXMLs = new List<PackDataXML>();
    }
}