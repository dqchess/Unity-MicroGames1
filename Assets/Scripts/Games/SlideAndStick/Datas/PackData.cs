using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    /** e.g. 4x4, 5x5, etc. */
    public class PackData {
        // Properties
        public LevelAddress MyAddress { get; private set; }
        public int NumLevelsCompleted { get; private set; }
        public int NumLevelsPlayable { get; private set; }
        public string PackName { get; private set; }
        private List<LevelData> levelDatas; // by levelIndex. ALL level datas in this world! Loaded up when WE'RE loaded up.
    
    	// Getters
        public bool DoesLevelExist(LevelAddress ad) {
            if (ad.level<0 || ad.level>=levelDatas.Count) { return false; } // Outta bounds? Return false!
            return true; // Yeah, this level exists!
        }
    	public bool DidCompleteAllLevels { get { return NumLevelsCompleted >= NumLevels; } }
        public int NumLevels { get { return levelDatas.Count; } }
        public System.Collections.ObjectModel.ReadOnlyCollection<LevelData> LevelDatas { get { return levelDatas.AsReadOnly(); } }
    	public LevelData GetLevelData (LevelAddress levelAddress) { return GetLevelData (levelAddress.level); }
    	public LevelData GetLevelData (int index) {
    		if (index<0 || index>=levelDatas.Count) { return null; } // Outta bounds.
    		return levelDatas[index];
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public PackData (LevelAddress myAddress, PackDataXML packDataXML) {
    		this.MyAddress = myAddress;
    		this.PackName = packDataXML.packName;
    
    		LoadAllLevelDatas(packDataXML);
    	}
    
    
    	// ----------------------------------------------------------------
    	//  LevelDatas
    	// ----------------------------------------------------------------
    	/** Makes a LevelData for every level file in our world's levels folder!! */
    	private void LoadAllLevelDatas (PackDataXML packDataXML) {
    		// Convert the XML to LevelDatas!
    		levelDatas = new List<LevelData>();
    		for (int i=0; i<packDataXML.boardDataXMLs.Count; i++) {
    			LevelAddress levelAddress = new LevelAddress(MyAddress.mode, MyAddress.collection, MyAddress.pack, i);
    			LevelData newLD = new LevelData(levelAddress, packDataXML.boardDataXMLs[i]);
    			levelDatas.Add (newLD);
    		}
    		// Update this value now that we've got our datas.
            UpdateNumLevelsPlayable();
    		UpdateNumLevelsCompleted();
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void OnCompleteLevel (LevelAddress levelAddress) {
    		LevelData levelData = GetLevelData (levelAddress);
    		if (levelData != null) {
    			levelData.SetDidCompleteLevel (true);
    		}
    		else { Debug.LogError ("LevelData is null for OnCompleteLevel. Hmm."); } // Hmm.
    		UpdateNumLevelsCompleted ();
    	}
        private void UpdateNumLevelsPlayable() {
            NumLevelsPlayable = 0;
            for (int i=0; i<NumLevels; i++) {
                if (GetLevelData(i).boardData.tileDatas.Count == 0) {
                    NumLevelsPlayable = i;
                    break;
                }
            }
        }
    	public void UpdateNumLevelsCompleted () {
    		NumLevelsCompleted = 0;
    		for (int i=0; i<NumLevels; i++) {
    			if (GetLevelData(i).DidCompleteLevel) { NumLevelsCompleted ++; }
    		}
    	}
    
    	// Events
    //	public void OnCompleteLevel (int levelIndex) {
    //		LevelData ld = GetLevelData(levelIndex);
    //		bool isFirstTimeCompleted = !ld.DidCompleteLevel;
    //		if (isFirstTimeCompleted) {
    //			// Save stats!
    //			ld.SaveDidCompleteLevel();
    //		}
    //	}
    
    }
}