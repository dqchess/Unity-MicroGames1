using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
public class LevSelProgressBar : MonoBehaviour {
        // Components
        [SerializeField] private Image i_fill=null;
        [SerializeField] private RectTransform myRectTransform=null;
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetFillPercent(float percent) {
            float widthFull = myRectTransform.rect.width;
            float fillWidth = widthFull * percent;
            i_fill.rectTransform.sizeDelta = new Vector2(fillWidth, i_fill.rectTransform.sizeDelta.y);
        }
        
        
        public void UpdateVisuals(LevelAddress address, Color color) {
            i_fill.color = color;
            
            PackData packData = LevelsManager.Instance.GetPackData(address);
            float percent = packData.NumLevelsCompleted / (float)packData.NumLevels;
            
            SetFillPercent(percent);
        }
        
        
    }
}