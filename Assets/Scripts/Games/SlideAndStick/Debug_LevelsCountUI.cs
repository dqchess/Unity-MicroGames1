using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class Debug_LevelsCountUI : MonoBehaviour {
        // Components
        [SerializeField] private Text t_counts;
        
        // Getters (Private)
        private LevelsManager lm { get { return LevelsManager.Instance; } }
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            UpdateCountsText();
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void UpdateCountsText() {
            string str = "";
            ModeCollectionData mcdStandard = lm.GetModeCollectionData(GameModes.StandardIndex);
            for (int i=2; i<mcdStandard.CollectionDatas.Count; i++) { // start at Tutorial collection.
                PackCollectionData pcData = mcdStandard.CollectionDatas[i];
                foreach (PackData pData in pcData.PackDatas) {
                    str += pcData.CollectionName + ", " + pData.PackName + ":    " + pData.NumLevels + "\n";
                }
            }
            t_counts.text = str;
        }
        
    }
}