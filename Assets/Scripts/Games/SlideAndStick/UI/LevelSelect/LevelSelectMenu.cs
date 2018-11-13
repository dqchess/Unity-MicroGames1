using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelSelectMenu : BaseSelectMenu {
    	// Constants
    	private const float tileHeight = 60f;
    	private const float tileGapY = 8f;
    	// Components
    	[SerializeField] private TextMeshProUGUI t_packName=null;
    	[SerializeField] private RectTransform rt_scrollContent=null;
    	private List<LevelTile> levelTiles;
    	// References
    	private PackData myPackData=null;
    
    	// Getters
    	private float GetScrollStartingPos() {
    		return -GetLastPlayedTilePosY() - 100; // move it down a little bit more so the last played tile isn't the topmost one.
    	}
    	private float GetLastPlayedTilePosY() {
    		LevelAddress lastPlayedLevelAddress = lm.GetLastPlayedLevelAddress();
    		foreach (LevelTile tile in levelTiles) {
    			if (tile.LevelData.myAddress == lastPlayedLevelAddress) {
    				return tile.transform.localPosition.y;
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
    
    			RemakeTiles ();
    		}
    	}
    
    	private string GetPackNameText() {
    		PackCollectionData collectionData = lm.GetPackCollectionData (selectedAddress);
			return collectionData.CollectionName + " " + myPackData.PackName; // e.g. "Easy Food"
    	}
    	private void RemakeTiles () {
    		DestroyTiles();
    
    		levelTiles = new List<LevelTile>();
    		for (int i=0; i<myPackData.NumLevels; i++) {
    			AddLevelTile (myPackData.GetLevelData(i));
    		}
    		PositionTiles ();
    	}
    	private void AddLevelTile (LevelData ld) {
    		//LevelTile newTile = Instantiate(ResourcesHandler.Instance.levelTile).GetComponent<LevelTile>();
    		//newTile.Initialize (this, rt_scrollContent, ld);
    		//levelTiles.Add (newTile);
    	}
    	private void DestroyTiles() {
    		if (levelTiles!=null) {
    			foreach (LevelTile tile in levelTiles) {
    				Destroy(tile.gameObject);
    			}
    			levelTiles.Clear();
    		}
    	}
    
    	override public void PositionTiles () {
    		Vector2 levelTilesContainerSize = rt_scrollContent.GetComponent<RectTransform>().rect.size;
    
    		float tempY = -20;
    		for (int i=0; i<myPackData.NumLevels; i++) {
    			LevelTile tile = levelTiles[i];
    			tile.SetPosSize (0, tempY, tileHeight);
    			tempY -= tileHeight + tileGapY;
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
//		for (int i=0; i<gos_tileWorldContainers.Length; i++) {
//			bool isVisible = i==selectedWorldIndex;
//			gos_tileWorldContainers[i].SetActive (isVisible);
//		}
//		// Update header text!
//		t_packName.text = selectedWorldIndex + ": " + dataManager.GetWorldData(selectedWorldIndex).CollectionName;
//		// Update world-selecting buttons!
//		UpdateWorldSelectButtons ();
//	}

//		float containerWidth = ScreenHandler.RelativeScreenSize.x + levelTilesContainerSize.x; // let's use this value to determine how much horz. space I've got for the tiles.
//		float containerWidth = levelTilesContainerSize.x; // let's use this value to determine how much horz. space I've got for the tiles.