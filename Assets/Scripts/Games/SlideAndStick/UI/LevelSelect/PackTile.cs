using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class PackTile : MonoBehaviour {
    	// Components
    	[SerializeField] private Button button; // I'm like selectable, mm!
    	[SerializeField] private Image i_backing=null;
    	[SerializeField] private Image i_completedIcon=null;
    	[SerializeField] private RectTransform myRectTransform=null;
    	[SerializeField] private TextMeshProUGUI t_packName=null;
    	[SerializeField] private TextMeshProUGUI t_numLevelsCompleted=null;
    //	[SerializeField] private TextMeshProUGUI t_numLevelsTotal=null;
    	// References
    	private PackSelectMenu collectionSelectControllerRef;
    	private PackData myPackData;
    
    	// Getters
    	public PackData MyPackData { get { return myPackData; } }
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public void Initialize (PackSelectMenu collectionSelectControllerRef, RectTransform _parentTransform, PackData myPackData) {
    		this.collectionSelectControllerRef = collectionSelectControllerRef;
    		this.myPackData = myPackData;
    //		bool didCompleteLevel = GameManagers.Instance.DataManager.DidCompleteLevel (worldIndex, levelIndex);
    
    		// Parent jazz!
    		this.transform.SetParent (_parentTransform);
    		this.transform.localScale = Vector3.one;
    		this.transform.localPosition = Vector3.zero;
    		this.transform.localEulerAngles = Vector3.zero;
    		this.gameObject.name = "PackTile " + myPackData.MyAddress.collection + "-" + myPackData.PackName;
    		myRectTransform.anchoredPosition = new Vector2(0,0);
    		myRectTransform.offsetMin = new Vector2(0, 0);
    		myRectTransform.offsetMax = new Vector2(0, 0);
    
    		// Update visuals!
    		t_packName.text = myPackData.PackName.ToUpper();
    		i_completedIcon.enabled = myPackData.DidCompleteAllLevels;
    		if (myPackData.DidCompleteAllLevels) {
    			i_backing.color = new Color(0.6f,0.6f,0.6f);
    			t_packName.color = new Color(0,0,0, 0.6f);
    		}
    		t_numLevelsCompleted.text = myPackData.NumLevelsCompleted.ToString() + "/" + myPackData.NumLevels;
    //		t_numLevelsTotal.text = "/" + myPackData.NumLevels;
    //		t_numLevelsCompleted.rectTransform.anchorMax = new Vector2(0, t_numLevelsCompleted.rectTransform.anchorMax.y);
    	}
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void SetPosHeight (float posX,float posY, float height) {
    //		t_packName.fontSizeMax = Mathf.Min(_size.x,_size.y) * 0.4f;
    		// Make my width flush, THEN set my height, THEN set my position!
    		myRectTransform.offsetMax = Vector2.zero;
    		myRectTransform.offsetMin = Vector2.zero;
    		myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, height);
    		myRectTransform.anchoredPosition = new Vector2 (posX, posY);
    	}
    
    
    	public void OnClicked () {
    		collectionSelectControllerRef.OpenLevelSelect (myPackData.MyAddress);
    	}
    
    
    }
}