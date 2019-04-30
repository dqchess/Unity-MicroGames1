using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class CollectionButton : MonoBehaviour {
        // Components
        [SerializeField] private Image i_bottom=null;
		[SerializeField] private Image i_top=null;
		[SerializeField] private TextMeshProUGUI t_name=null;
		//[SerializeField] private CollectionProgressBar progressBar=null;
        // Properties
        [SerializeField] private int collectionIndex=0;
		private Color collectionColor;
		private LevelAddress myAddress;
        // References
        [SerializeField] private LevSelController levSelController=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
			myAddress = new LevelAddress(GameModes.StandardIndex, collectionIndex, 0,0);
			PackCollectionData pcd = LevelsManager.Instance.GetPackCollectionData(myAddress);
			collectionColor = LevSelController.GetCollectionColor(collectionIndex);

            t_name.text = pcd.CollectionName;
            i_top.color = collectionColor;
            i_bottom.color = Color.Lerp(collectionColor, Color.black, 0.3f);
			//UpdateBarVisuals();
		}

		//private void UpdateBarVisuals() {
			//progressBar.UpdateVisuals(myAddress, collectionColor);
        //}


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            levSelController.OnCollectionButtonClick(collectionIndex);
            GameManagers.Instance.EventManager.OnAnyButtonClick();
        }
		//private void OnEnable() {
		//	UpdateBarVisuals();
		//}
    }
}
