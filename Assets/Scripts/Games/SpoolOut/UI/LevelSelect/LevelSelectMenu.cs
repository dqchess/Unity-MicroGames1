using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
namespace SpoolOut {
    public class LevelSelectMenu : BaseSelectMenu {
    	// Constants
    	private const float spoolHeight = 60f;
    	private const float spoolGapY = 8f;
    	// Components
    	[SerializeField] private TextMeshProUGUI t_packName=null;
    	[SerializeField] private RectTransform rt_scrollContent=null;
    	private List<LevelSpool> levelSpools;
    	// References
    	private PackData myPackData=null;
    
    	// Getters
    	private float GetScrollStartingPos() {
    		return -GetLastPlayedSpoolPosY() - 100; // move it down a little bit more so the last played spool isn't the topmost one.
    	}
    	private float GetLastPlayedSpoolPosY() {
    		LevelAddress lastPlayedLevelAddress = lm.GetLastPlayedLevelAddress();
    		foreach (LevelSpool spool in levelSpools) {
    			if (spool.LevelData.myAddress == lastPlayedLevelAddress) {
    				return spool.transform.localPosition.y;
    			}
    		}
    		return 0;
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Open
    	// ----------------------------------------------------------------
    	override public void Show() {
    		base.Show();
    
    		PackData ppackData = myPackData;
    		myPackData = lm.GetPackData (selectedAddress);
    
    		// Is this a DIFFERENT mode?? Remake my stuff!
    		if (myPackData != ppackData) {
    			t_packName.text = GetPackNameText();
    
    			RemakeSpools ();
    		}
    	}
    
    	private string GetPackNameText() {
    		PackCollectionData collectionData = lm.GetPackCollectionData (selectedAddress);
			return collectionData.CollectionName + " " + myPackData.PackName; // e.g. "Easy Food"
    	}
    	private void RemakeSpools () {
    		DestroySpools();
    
    		levelSpools = new List<LevelSpool>();
    		for (int i=0; i<myPackData.NumLevels; i++) {
    			AddLevelSpool (myPackData.GetLevelData(i));
    		}
    		PositionSpools ();
    	}
    	private void AddLevelSpool (LevelData ld) {
    		//LevelSpool newSpool = Instantiate(ResourcesHandler.Instance.levelSpool).GetComponent<LevelSpool>();
    		//newSpool.Initialize (this, rt_scrollContent, ld);
    		//levelSpools.Add (newSpool);
    	}
    	private void DestroySpools() {
    		if (levelSpools!=null) {
    			foreach (LevelSpool spool in levelSpools) {
    				Destroy(spool.gameObject);
    			}
    			levelSpools.Clear();
    		}
    	}
    
    	override public void PositionSpools () {
    		Vector2 levelSpoolsContainerSize = rt_scrollContent.GetComponent<RectTransform>().rect.size;
    
    		float tempY = -20;
    		for (int i=0; i<myPackData.NumLevels; i++) {
    			LevelSpool spool = levelSpools[i];
    			spool.SetPosSize (0, tempY, spoolHeight);
    			tempY -= spoolHeight + spoolGapY;
    		}
    
    		// Set scroll layer's height, now that we know the bottommost element!
    		tempY -= 300; // Give us some extra scroll room.
    		rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, -tempY);
    		// Reset the scroll layer pos.
    		rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, GetScrollStartingPos());
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void LoadLevel (LevelAddress address) {
    		levSelController.OpenLevel(address);
    	}
    
    
    
    }
}


//	private void SetWorld (int _worldIndex) {
//		// Set the value!
//		selectedWorldIndex = _worldIndex;
//		// Update containers' visibilities!
//		for (int i=0; i<gos_spoolWorldContainers.Length; i++) {
//			bool isVisible = i==selectedWorldIndex;
//			gos_spoolWorldContainers[i].SetActive (isVisible);
//		}
//		// Update header text!
//		t_packName.text = selectedWorldIndex + ": " + dataManager.GetWorldData(selectedWorldIndex).CollectionName;
//		// Update world-selecting buttons!
//		UpdateWorldSelectButtons ();
//	}

//		float containerWidth = ScreenHandler.RelativeScreenSize.x + levelSpoolsContainerSize.x; // let's use this value to determine how much horz. space I've got for the spools.
//		float containerWidth = levelSpoolsContainerSize.x; // let's use this value to determine how much horz. space I've got for the spools.

*/