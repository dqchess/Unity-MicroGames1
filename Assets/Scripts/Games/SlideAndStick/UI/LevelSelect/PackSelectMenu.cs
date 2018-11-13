using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class PackSelectMenu : BaseSelectMenu {
    	// Constants
    	private const float tileHeight = 90f;
    	private const float tileGapY = 12f;
    	private const float collectionGapY = 100f; // between each collection (set of packs).
    	// Components
    	[SerializeField] private TextMeshProUGUI t_modeName=null;
    	[SerializeField] private RectTransform rt_scrollContent=null;
    	private List<TextMeshProUGUI> collectionNameTexts; // the headers for each collection of packs.
    	private List<List<PackTile>> packTiles; // first index is Collection; second is packIndex within Collection.
    	// Properties
    	private int modeIndex=-1;
    
    	// Getters / Setters
    	private Color GetCollectionHeaderColor (int collectionIndex) {
    		switch (collectionIndex) {
    			case 0: return new Color(114/255f, 255/255f, 0/255f); // Easy
    			case 1: return new Color(255/255f, 230/255f, 0/255f); // Medium
    			case 2: return new Color(255/255f, 60/255f, 0/255f); // Hard
    			case 3: return new Color(255/255f, 0/255f, 200/255f); // Very Hard
    			default: return Color.white; // Who knows
    		}
    	}
    	private float GetScrollStartingPos() {
    		return -GetLastPlayedTilePosY() - 100; // move it down a little bit more so the last played tile isn't the topmost one.
    	}
    	private float GetLastPlayedTilePosY() {
    		LevelAddress lastPlayedLevelAddress = lm.GetLastPlayedLevelAddress();
    		foreach (List<PackTile> tileList in packTiles) {
    			foreach (PackTile tile in tileList) {
    				LevelAddress address = tile.MyPackData.MyAddress;
    				if (address.mode == lastPlayedLevelAddress.mode
    				 && address.collection == lastPlayedLevelAddress.collection
    				 && address.pack == lastPlayedLevelAddress.pack) {
    					return tile.transform.localPosition.y;
    				}
    			}
    		}
    		return 0;
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Open
    	// ----------------------------------------------------------------
    	override public void Show () {
    		base.Show();
    
    		int pmodeIndex = modeIndex;
    		modeIndex = lm.selectedAddress.mode;
    
    		// Is this a DIFFERENT mode?? Remake my stuff!
    		if (modeIndex != pmodeIndex) {
    			RemakeTilesAndTexts ();
    
    			t_modeName.text = lm.GetModeCollectionData(modeIndex).ModeDisplayName;
    		}
    	}
    
    	private void RemakeTilesAndTexts () {
    		DestroyTilesAndTexts();
    
    		packTiles = new List<List<PackTile>>();
    		collectionNameTexts = new List<TextMeshProUGUI>();
    
    		int numCollections = lm.GetModeCollectionData(modeIndex).NumPackCollections;
    		for (int ci=0; ci<numCollections; ci++) {
    			// Collection Name
    			AddCollectionNameText (ci);
    			// Pack Tiles
    			packTiles.Add (new List<PackTile>());
    			PackCollectionData collectionData = lm.GetPackCollectionData(modeIndex, ci);
    			for (int pi=0; pi<collectionData.NumPacks; pi++) {
    				PackData packData = collectionData.GetPackData (pi);
    				AddPackTile (packData);
    			}
    		}
    		PositionTiles ();
    	}
    	private void AddPackTile (PackData packData) {
    		//PackTile newTile = Instantiate(ResourcesHandler.Instance.sli).GetComponent<PackTile>();
    		//RectTransform containerRT = rt_scrollContent;//gos_tileWorldContainers[packData.worldIndex];
    		//newTile.Initialize (this, containerRT, packData);
    		//int collectionIndex = packData.MyAddress.collection;
    		//packTiles[collectionIndex].Add (newTile);
    	}
    	private void AddCollectionNameText (int collectionIndex) {
    		//TextMeshProUGUI newText = Instantiate(ResourcesHandler.Instance.packCollectionName).GetComponent<TextMeshProUGUI>();
    		//newText.transform.SetParent (rt_scrollContent);
    		//newText.transform.localScale = Vector3.one;
    		//newText.transform.localEulerAngles = Vector3.zero;
    		//PackCollectionData collectionData = lm.GetPackCollectionData(modeIndex, collectionIndex);
    		//newText.text = collectionData.CollectionName + " " + TEMP_LevelProgressFraction(collectionData);
    		//newText.color = GetCollectionHeaderColor (collectionIndex);
    		////newCollectionNameText.rectTransform.offsetMin = 
    		//collectionNameTexts.Add (newText);
    	}
    	private void DestroyTilesAndTexts() {
    		if (packTiles!=null) {
    			foreach (List<PackTile> tileList in packTiles) {
    				foreach (PackTile tile in tileList) {
    					Destroy(tile.gameObject);
    				}
    			}
    			packTiles.Clear();
    		}
    		if (collectionNameTexts != null) {
    			foreach (TextMeshProUGUI text in collectionNameTexts) {
    				Destroy(text.gameObject);
    			}
    			collectionNameTexts.Clear();
    		}
    	}
    
    	private string TEMP_LevelProgressFraction(PackCollectionData collectionData) {
    		int numLevels = 0;
    		int numLevelsCompleted = 0;
    		foreach (PackData packData in collectionData.PackDatas) {
    			numLevels += packData.NumLevels;
    			numLevelsCompleted += packData.NumLevelsCompleted;
    		}
    		return "(" + numLevelsCompleted + " / " + numLevels + ")";
    	}
    
    	override public void PositionTiles () {
    		Vector2 containerSize = rt_scrollContent.GetComponent<RectTransform>().rect.size;
    //		float containerWidth = ScreenHandler.RelativeScreenSize.x + levelTilesContainerSize.x; // let's use this value to determine how much horz. space I've got for the tiles.
    //		float containerWidth = containerSize.x; // let's use this value to determine how much horz. space I've got for the tiles.
    
    		int numCollections = lm.GetModeCollectionData(modeIndex).NumPackCollections;
    		float posX = 0;
    		float tempY = -20; // where we're putting things! Added to as we go along.
    		for (int ci=0; ci<numCollections; ci++) {
    			// Collection Name
    			collectionNameTexts[ci].rectTransform.anchoredPosition = new Vector2(posX, tempY);
    			collectionNameTexts[ci].rectTransform.offsetMax = new Vector2(0, collectionNameTexts[ci].rectTransform.offsetMax.y); // size width correctly
    			tempY -= 72;
    			// PackTiles
    			int numPacks = packTiles[ci].Count;
    			for (int pi=0; pi<numPacks; pi++) {
    				PackTile packTile = packTiles[ci][pi];
    
    				packTile.SetPosHeight (posX,tempY, tileHeight);
    				tempY -= tileHeight + tileGapY;
    			}
    			tempY -= collectionGapY;
    		}
    
    		// Set scroll layer's height, now that we know the bottommost element!
    		tempY -= 100; // Give us some extra scroll room.
    		rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, -tempY);
    		// Reset the scroll layer pos.
    		rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, GetScrollStartingPos());
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void OpenLevelSelect (LevelAddress packAddress) {
    		lm.selectedAddress = packAddress;
    		//levSelController.PushMenu (MenuNames.LevelSelect);
    	}
    
    }
}


