using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class LevelData {
    	// Properties
    	public BoardData boardData;
    	public bool isLocked; // this is calculated by my WorldData.
    	public LevelAddress myAddress;
    	public string levelKey;
    	// Variable properties
    	private bool didCompleteLevel;
    
    	// Getters
    	public bool DidCompleteLevel { get { return didCompleteLevel; } }
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public LevelData (LevelAddress myAddress, BoardDataXML ldxml) {
    		// Basic properties
    		this.myAddress = myAddress;
    		boardData = new BoardData(ldxml);
    		levelKey = myAddress.ToString(); // whatevs.
    
    		// LOAD up stats!
    		didCompleteLevel = SaveStorage.GetInt (SaveKeys.SlideAndStick_DidCompleteLevel(myAddress)) == 1;
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void SetDidCompleteLevel (bool _didCompleteLevel) {
    		if (didCompleteLevel != _didCompleteLevel) {
    			didCompleteLevel = _didCompleteLevel;
    			SaveStorage.SetInt (SaveKeys.SlideAndStick_DidCompleteLevel(myAddress), didCompleteLevel?1:0);
    		}
    	}
    
    }

}