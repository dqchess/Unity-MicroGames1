using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    /** Currently unused! There's only one Mode. */
    public class ModeCollectionData {
    	// Properties
    	private int modeIndex;
    	private string modeName; // for code
    	private string modeDisplayName; // for user's eyes
    	private string[] collectionsOrder; // the NAMES of the world xml files in order. Loaded from its own file.
    	private List<PackCollectionData> collectionDatas;
    
    
    	// ----------------------------------------------------------------
    	//  Getters
    	// ----------------------------------------------------------------
    	public string ModeDisplayName { get { return modeDisplayName; } }
    	public int NumPackCollections { get { return collectionDatas.Count; } }
    	public System.Collections.ObjectModel.ReadOnlyCollection<PackCollectionData> CollectionDatas { get { return collectionDatas.AsReadOnly(); } }
    	public PackCollectionData GetPackCollectionData (int collectionIndex) {
    		if (collectionIndex<0 || collectionIndex>=collectionDatas.Count) { return null; }
    		return collectionDatas [collectionIndex];
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public ModeCollectionData (int modeIndex, string modeName, string modeDisplayName) {
    		this.modeIndex = modeIndex;
    		this.modeName = modeName;
    		this.modeDisplayName = modeDisplayName;
    
    		LoadAllCollectionDatas ();
    	}
    
    
    	// ----------------------------------------------------------------
    	//  PackDatas
    	// ----------------------------------------------------------------
    	private void LoadAllCollectionDatas () {
    		// Collections Order!
    		string pathSuffix = "Games/" + GameNames.SlideAndStick + "/Levels/";//TODO: Clarify this! TEST all modes are the same! + modeName + "/";
    		collectionsOrder = TextUtils.GetStringArrayFromResourcesTextFile(pathSuffix + "_CollectionOrder");
    
    		collectionDatas = new List<PackCollectionData>();
    		for (int i=0; i<collectionsOrder.Length; i++) {
    			string collectionPath = pathSuffix + collectionsOrder[i];// + ".xml";//Application.streamingAssetsPath + "/" + 
    			LevelAddress collectionAddress = new LevelAddress(modeIndex, i, -1, -1);
    			PackCollectionData newData = new PackCollectionData(collectionAddress, collectionPath);
    			collectionDatas.Add (newData);
    		}
    	}
    }
}
