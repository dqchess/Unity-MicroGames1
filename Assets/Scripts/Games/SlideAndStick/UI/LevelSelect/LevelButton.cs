using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelButton : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private Image i_backing=null;
        // Properties
        //private LevelAddress myAddress;
        // References
        [SerializeField] private Sprite s_completed=null;
        [SerializeField] private Sprite s_notCompleted=null;
        private LevelData myLevelData;
        private PackSelectMenu packSelectMenu;
        
        
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(PackSelectMenu _psm, Transform tf_parent) {
            this.packSelectMenu = _psm;
            
            // Parent jazz!
            GameUtils.ParentAndReset(this.gameObject, tf_parent);
        }
        
        
        // ----------------------------------------------------------------
        //  Spawn / Despawn
        // ----------------------------------------------------------------
        public void Despawn() {
            this.gameObject.SetActive(false);
        }
        public void Spawn(LevelData _levelData) {
            this.gameObject.SetActive(true);
            this.myLevelData = _levelData;
            
            int levelIndexDisplay = myLevelData.myAddress.level + 1;
            
            this.name = "Level_" + levelIndexDisplay;
            t_levelName.text = levelIndexDisplay.ToString();
            
            // Completed-ness visuals!
            if (myLevelData.DidCompleteLevel) {
                i_backing.sprite = s_completed;
                i_backing.color = packSelectMenu.CurrentPackColor;
                t_levelName.color = Color.white;
            }
            else {
                i_backing.sprite = s_notCompleted;
                i_backing.color = new Color(0.65f,0.65f,0.65f);
                t_levelName.color = packSelectMenu.CurrentPackColor;
            }
        }
        public void SetPosSize(Vector2 _pos, Vector2 _size) {
            myRectTransform.anchoredPosition = _pos;
            myRectTransform.sizeDelta = _size;
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            packSelectMenu.OnClickPackButton(myLevelData.myAddress);
        }
    }
}