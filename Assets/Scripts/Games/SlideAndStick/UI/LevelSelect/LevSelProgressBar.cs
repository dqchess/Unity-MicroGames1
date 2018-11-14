using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
public class LevSelProgressBar : MonoBehaviour {
		// Components
		[SerializeField] private Image i_fill=null;
		[SerializeField] private Image i_fillMask=null;
        [SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private float widthFull;
//		private float height;
        

		private void Start() {
			widthFull = myRectTransform.rect.width;
//			height = myRectTransform.rect.height;
			// Fill image is always the full width. (We size the mask instead.)
			i_fill.rectTransform.sizeDelta = new Vector2(widthFull, i_fill.rectTransform.sizeDelta.y);
		}

        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetFillPercent(float percent) {
            float fillWidth = widthFull * percent;
			i_fillMask.rectTransform.sizeDelta = new Vector2(fillWidth, i_fillMask.rectTransform.sizeDelta.y);
        }
        
        
        public void UpdateVisuals(LevelAddress address, Color color) {
            i_fill.color = color;
            
            PackData packData = LevelsManager.Instance.GetPackData(address);
            float percent = packData.NumLevelsCompleted / (float)packData.NumLevels;
            
            SetFillPercent(percent);
        }
        
        
    }
}