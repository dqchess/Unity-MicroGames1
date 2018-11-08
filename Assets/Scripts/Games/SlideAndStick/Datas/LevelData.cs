using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class LevelData {
    	// Properties
    	public BoardData boardData;
    	public bool isLocked; // this is calculated by my WorldData.
    	public LevelAddress myAddress;
        // Variable Properties
        public bool DidCompleteLevel { get; private set; }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public LevelData () { }
        public LevelData (LevelAddress myAddress, BoardDataXML ldxml) {
    		// Basic properties
    		this.myAddress = myAddress;
    		boardData = new BoardData(ldxml);
    
    		// LOAD up stats!
    		DidCompleteLevel = SaveStorage.GetInt (SaveKeys.SlideAndStick_DidCompleteLevel(myAddress)) == 1;
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void SetDidCompleteLevel (bool _didCompleteLevel) {
    		if (DidCompleteLevel != _didCompleteLevel) {
    			DidCompleteLevel = _didCompleteLevel;
    			SaveStorage.SetInt (SaveKeys.SlideAndStick_DidCompleteLevel(myAddress), DidCompleteLevel?1:0);
    		}
    	}
    
    }

}