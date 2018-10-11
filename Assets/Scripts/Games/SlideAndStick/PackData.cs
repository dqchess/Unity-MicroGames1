﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    /** e.g. Food, Transport, Nature. */
    public class PackData {
    	// Properties
    	private LevelAddress myAddress;
    	private int numLevelsCompleted;
    	private string packName;
    	private List<LevelData> levelDatas; // by levelIndex. ALL level datas in this world! Loaded up when WE'RE loaded up.
    //	// References
    //	private PackCollectionData myCollectionData;
    
    	// Getters
    	public bool DidCompleteAllLevels { get { return numLevelsCompleted >= NumLevels; } }
    	public LevelAddress MyAddress { get { return myAddress; } }
    	public int NumLevels { get { return levelDatas.Count; } }
    	public int NumLevelsCompleted { get { return numLevelsCompleted; } }
    	public string PackName { get { return packName; } }
    	public System.Collections.ObjectModel.ReadOnlyCollection<LevelData> LevelDatas { get { return levelDatas.AsReadOnly(); } }
    	public LevelData GetLevelData (LevelAddress levelAddress) { return GetLevelData (levelAddress.level); }
    	public LevelData GetLevelData (int index) {
    		if (index<0 || index>=levelDatas.Count) { return null; } // Outta bounds.
    		return levelDatas[index];
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public PackData (LevelAddress myAddress, PackDataXML packDataXML) {//PackCollectionData myCollectionData, 
    //		this.myCollectionData = myCollectionData;
    		this.myAddress = myAddress;
    		this.packName = packDataXML.packName;
    
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
    			LevelAddress levelAddress = new LevelAddress(myAddress.mode, myAddress.collection, myAddress.pack, i);
    			LevelData newLD = new LevelData(levelAddress, packDataXML.boardDataXMLs[i]);
    			levelDatas.Add (newLD);
    		}
    		// Update this value now that we've got our datas.
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
    	public void UpdateNumLevelsCompleted () {
    		numLevelsCompleted = 0;
    		for (int i=0; i<NumLevels; i++) {
    			if (GetLevelData(i).DidCompleteLevel) { numLevelsCompleted ++; }
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