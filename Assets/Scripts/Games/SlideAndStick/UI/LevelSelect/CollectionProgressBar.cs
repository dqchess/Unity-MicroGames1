using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class CollectionProgressBar : MonoBehaviour {
		// Components
		[SerializeField] private Image i_fill=null;
		[SerializeField] private Image i_fillMask=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private float widthFull;


		private void Start() {
			widthFull = myRectTransform.rect.width;
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
//			i_fill.color = color;

			PackCollectionData pcd = LevelsManager.Instance.GetPackCollectionData(address);
			if (pcd != null || pcd.NumLevels()==0) { // Safety check.
				float percent = pcd.NumLevelsCompleted() / (float)pcd.NumLevels();
				SetFillPercent(percent);
			}
		}


	}
}