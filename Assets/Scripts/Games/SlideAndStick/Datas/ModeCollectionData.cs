using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class AllLevelsData {
    	// Properties
        private string[] collectionsOrder; // the NAMES of the world xml files in order. Loaded from its own file.
    	private List<PackCollectionData> collectionDatas;
    
    
    	// ----------------------------------------------------------------
    	//  Getters
    	// ----------------------------------------------------------------
        public bool DoesLevelExist(LevelAddress ad) {
            if (ad.collection<0 || ad.collection>=collectionDatas.Count) { return false; } // Outta bounds? Return false!
            return collectionDatas[ad.collection].DoesLevelExist(ad); // Ok, ask the next guy.
        }
        public int NumPackCollections { get { return collectionDatas.Count; } }
    	public System.Collections.ObjectModel.ReadOnlyCollection<PackCollectionData> CollectionDatas { get { return collectionDatas.AsReadOnly(); } }
    	public PackCollectionData GetPackCollectionData (int collectionIndex) {
    		if (collectionIndex<0 || collectionIndex>=collectionDatas.Count) { return null; }
    		return collectionDatas [collectionIndex];
    	}
        public int NumLevels() {
            int total = 0;
            foreach (PackCollectionData data in CollectionDatas) {
                total += data.NumLevels();
                //Debug.Log("PackCollectionData total: " + data.NumLevels());
            }
            return total;
        }
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public AllLevelsData () {
    		LoadAllCollectionDatas ();
    	}
    
    
    	// ----------------------------------------------------------------
    	//  PackDatas
    	// ----------------------------------------------------------------
    	private void LoadAllCollectionDatas () {
    		// Collections Order!
    		string pathSuffix = "Games/" + GameNames.SlideAndStick + "/Levels/";
            // A/B test paths differ!
            pathSuffix += ABTestsManager.Instance.IsEasies ? "Easies/" : "NoEasies/";
            
    		collectionsOrder = TextUtils.GetStringArrayFromResourcesTextFile(pathSuffix + "_CollectionOrder");
    		collectionDatas = new List<PackCollectionData>();
    		for (int i=0; i<collectionsOrder.Length; i++) {
    			string collectionPath = pathSuffix + collectionsOrder[i];// + ".xml";//Application.streamingAssetsPath + "/" + 
    			LevelAddress collectionAddress = new LevelAddress(0, i, -1, -1);
    			PackCollectionData newData = new PackCollectionData(collectionAddress, collectionPath);
    			collectionDatas.Add (newData);
    		}
    	}
    }
}
