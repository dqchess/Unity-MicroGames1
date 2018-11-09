using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private GameObject go_randLayoutHelperUI=null;
        [SerializeField] private RectTransform rt_levelName=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private TextMeshProUGUI t_packName=null;
        [SerializeField] private LevelCompletePopup levelCompletePopup=null;
		// References
		[SerializeField] private Level level=null;
        
        // Getters (Public)
        public LevelCompletePopup LevelCompletePopup { get { return levelCompletePopup; } }
        // Getters (Private)
        private string GetCollectionName() {
            PackCollectionData data = LevelsManager.Instance.GetPackCollectionData(level.MyAddress);
            return data.CollectionName;
        }
        private string GetPackName() {
            PackData data = LevelsManager.Instance.GetPackData(level.MyAddress);
            return data.PackName;
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
            t_levelName.text = "LEVEL " + (level.MyAddress.level+1).ToString();
            if (level.Board.DidRandGen) { t_levelName.text += " (RAND)"; }
            t_packName.text = GetCollectionName() + ",  " + GetPackName() + " (D" + level.Board.Difficulty + ")";
            
            levelCompletePopup.Hide();
            
            go_randLayoutHelperUI.SetActive(level.Board.DidRandGen); // only show rand-gen UI for rand-gen boards.
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnBoardMade() {
            // Center levelName texts between top of screen and board.
            float x = rt_levelName.anchoredPosition.x;
            float y = level.BoardView.MyRectTransform.anchoredPosition.y*0.5f;
            rt_levelName.anchoredPosition = new Vector2(x,y);
        }
        
        
        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            // Debug!
            if (Input.GetKeyDown(KeyCode.D)) {
                go_randLayoutHelperUI.SetActive(!go_randLayoutHelperUI.activeSelf);
            }
        }


	}
}