using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
namespace SlideAndStick {
    public class LevelTile : MonoBehaviour {
    	// Components
    	[SerializeField] private Button button; // I'm like selectable, mm!
    	[SerializeField] private Image i_backing=null;
    	[SerializeField] private Image i_completedIcon=null;
    	[SerializeField] private RectTransform myRectTransform=null;
    	[SerializeField] private TextMeshProUGUI t_levelName=null;
    	// References
    	private LevelSelectMenu levelSelectControllerRef;
    	// Properties
    	private LevelData myLevelData;
    
    	// Getters
    	public LevelData LevelData { get { return myLevelData; } }
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public void Initialize (LevelSelectMenu _levelSelectControllerRef, Transform _parentTransform, LevelData myLevelData) {
    		levelSelectControllerRef = _levelSelectControllerRef;
    		this.myLevelData = myLevelData;
    //		bool didCompleteLevel = GameManagers.Instance.DataManager.DidCompleteLevel (worldIndex, levelIndex);
    
            int levelIndexDisplay = myLevelData.myAddress.level + 1;
    
    		// Parent jazz!
    		this.transform.SetParent (_parentTransform);
    		this.transform.localScale = Vector3.one;
    		this.transform.localPosition = Vector3.zero;
    		this.transform.localEulerAngles = Vector3.zero;
            this.gameObject.name = "LevelTile " + levelIndexDisplay;
    
    		// Update visuals!
            t_levelName.text = levelIndexDisplay.ToString();
            
    		i_completedIcon.enabled = myLevelData.DidCompleteLevel;
    		i_completedIcon.rectTransform.anchoredPosition = new Vector2(20, i_completedIcon.rectTransform.anchoredPosition.y); // Hmm. Its position is being funky. Set it manually here.
    		if (myLevelData.DidCompleteLevel) {
    			i_backing.color = new Color(0.6f,0.6f,0.6f);
    			t_levelName.color = new Color(0,0,0, 0.6f);
    		}
    	}
        
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void SetPosSize (float posX,float posY, float height) {
    		// Make my width flush, THEN set my height, THEN set my position!
    		myRectTransform.offsetMax = Vector2.zero;
    		myRectTransform.offsetMin = Vector2.zero;
    		myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, height);
    		myRectTransform.anchoredPosition = new Vector2 (posX, posY);
    	}
    //	private void SetIsLocked (bool _isLocked) {
    //		isLocked = _isLocked;
    //		// Update visuals!
    //		button.interactable = !isLocked;
    //		myCanvasGroup.alpha = isLocked ? 0.04f : 1f;
    //	}
    //	public void Debug_UnlockMe () {
    //		SetIsLocked (false);
    //	}
    
    
    	public void OnClicked () {
    		levelSelectControllerRef.LoadLevel (myLevelData.myAddress);
    	}

    }
}
*/