using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SlideAndStick {
    /** i.e. Easy, Medium, Hard. */
    public class PackCollectionData {
    	// Properties
        public string CollectionName { get; private set; } // the display name.
    	private LevelAddress myAddress;
        private List<PackData> packDatas;
    
    
    	// ----------------------------------------------------------------
    	//  Getters
    	// ----------------------------------------------------------------
        public bool DoesLevelExist(LevelAddress ad) {
            if (ad.pack<0 || ad.pack>=packDatas.Count) { return false; } // Outta bounds? Return false!
            return packDatas[ad.pack].DoesLevelExist(ad); // Ok, ask the next guy.
        }
    	public int NumPacks { get { return packDatas.Count; } }
        public System.Collections.ObjectModel.ReadOnlyCollection<PackData> PackDatas { get { return packDatas.AsReadOnly(); } }
        public int NumLevels() {
            int total = 0;
            foreach (PackData data in PackDatas) {
                total += data.NumLevels;
            }
            return total;
        }
    
    	public PackData GetPackData (int index) {
    		if (index<0 || index>=packDatas.Count) { return null; } // Outta bounds.
    		return packDatas[index];
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public PackCollectionData (LevelAddress myAddress, string collectionXMLFilePath) {
    		this.myAddress = myAddress;
    
    		LoadAllPackDatas (collectionXMLFilePath);
    	}
    
    
    	// ----------------------------------------------------------------
    	//  PackDatas
    	// ----------------------------------------------------------------
    	private void LoadAllPackDatas (string filePath) {
    		PackCollectionDataXML collectionXML;
    		TextAsset textAsset = Resources.Load<TextAsset>(filePath);
    		if (textAsset != null) {
    			XmlSerializer serializer = new XmlSerializer(typeof(PackCollectionDataXML));
    			StringReader reader = new StringReader(textAsset.text);
    			collectionXML = serializer.Deserialize(reader) as PackCollectionDataXML;
    		}
    		else {
    			Debug.LogError("Can't find PackCollectionData file! \"" + filePath + "\"");
    			collectionXML = new PackCollectionDataXML();
    		}
    
    		// MY properties!
    		CollectionName = collectionXML.collectionName;
    
    		// Make those PackDatas!
    		packDatas = new List<PackData>();
    		for (int i=0; i<collectionXML.packDataXMLs.Count; i++) {
    			LevelAddress packAddress = new LevelAddress(myAddress.mode, myAddress.collection, i, -1);
    			PackData newData = new PackData(packAddress, collectionXML.packDataXMLs[i]);
    			AddPackData (newData);
    		}
    	}
    	private void AddPackData (PackData newData) {
    		packDatas.Add (newData);
    	}


}

}




