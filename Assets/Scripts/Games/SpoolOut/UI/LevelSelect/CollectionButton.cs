using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpoolOut {
    public class CollectionButton : MonoBehaviour {
        // Components
        [SerializeField] private Image i_bottom=null;
        [SerializeField] private Image i_top=null;
        [SerializeField] private TextMeshProUGUI t_name=null;
        // Properties
        [SerializeField] private int collectionIndex=0;
        // References
        [SerializeField] private LevSelController levSelController=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            PackCollectionData pcd = LevelsManager.Instance.GetPackCollectionData(GameModes.StandardIndex, collectionIndex);
            t_name.text = pcd.CollectionName;
            Color collectionColor = LevSelController.GetCollectionColor(collectionIndex);
            i_top.color = collectionColor;
            i_bottom.color = Color.Lerp(collectionColor, Color.black, 0.3f);
        }

        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            levSelController.OnCollectionButtonClick(collectionIndex);
        }
    }
}
