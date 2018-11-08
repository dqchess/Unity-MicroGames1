using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    [System.Serializable]
    public class BoardDataList {
        // Properties
        public List<BoardData> bds = new List<BoardData>();
        
        // Getters
        public int NumDatas { get { return bds.Count; } }
        public BoardData LastData {
            get {
                if (bds==null || NumDatas==0) { return null; }
                return bds[NumDatas-1];
            }
        }
    }

    public class RandLayoutHelperUI : MonoBehaviour {
        // Components
        [SerializeField] private CanvasGroup cg_savedPopup=null;
        [SerializeField] private GameObject go_params=null;
        [SerializeField] private GameObject go_saveButtons=null;
        [SerializeField] private Slider sl_numColors=null;
        [SerializeField] private Slider sl_stickiness=null;
        [SerializeField] private Slider sl_percentTiles=null;
        [SerializeField] private TextMeshProUGUI t_numColors=null;
        [SerializeField] private TextMeshProUGUI t_stickiness=null;
        [SerializeField] private TextMeshProUGUI t_percentTiles=null;
        [SerializeField] private TextMeshProUGUI t_savedPopup=null;
        // Properties
        //private string savedLayoutXMLs; // all the XML nodes in one big-ass string.
        private BoardDataList custLayouts; // all the fellas just waiting to get printed out! Remember: These guys are in limbo until we copy to the clipboard.
        // References
        [SerializeField] private Level level=null;
        
        // Getters (Public)
        //static public int GetNumLayouts(string savedLayoutsString) {
        //    return TextUtils.CountOccurances(savedLayoutsString, "layout=");
        //}
        // Getters (Private)
        private GameController gameController { get { return level.GameController; } }
        private RandGenParams rgp { get { return gameController.randGenParams; } }
        private bool AreLayoutsIdentical(BoardData bdA, BoardData bdB) {
            if (bdA==null ^ bdB==null) { return false; } // One (but not both) is null? Nah, not identical.
            return bdA.Debug_GetLayout(true) == bdB.Debug_GetLayout(true);
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            // Load layouts!
            LoadCustLayoutsJson();
            // Load params!
            sl_numColors.value = rgp.NumColors;
            sl_stickiness.value = rgp.Stickiness;
            sl_percentTiles.value = rgp.PercentTiles;
            UpdateParamsTextsFromValues();
            // Hide t_savedPopup.
            SetSavedPopupAlpha(0);
        }
        

        // ----------------------------------------------------------------
        //  UI Doers
        // ----------------------------------------------------------------
        public void ToggleParamsVisible() {
            go_params.SetActive(!go_params.activeSelf);
        }
        public void ToggleSaveButtonsVisible() {
            go_saveButtons.SetActive(!go_saveButtons.activeSelf);
        }
        private void HideSaveButtons() {
            go_saveButtons.SetActive(false);
        }
        private void UpdateParamsTextsFromValues() {
            t_numColors.text = rgp.NumColors.ToString();
            t_stickiness.text = rgp.Stickiness.ToString();
            t_percentTiles.text = (int)(100*rgp.PercentTiles) + "%";
        }
        private void SetSavedPopupAlpha(float alpha) {
            cg_savedPopup.alpha = alpha;
        }
        
        // ----------------------------------------------------------------
        //  UI Events
        // ----------------------------------------------------------------
        public void OnSlVal_NumColors() {
            rgp.NumColors = (int)sl_numColors.value;
            UpdateParamsTextsFromValues();
        }
        public void OnSlVal_Stickiness() {
            rgp.Stickiness = (int)sl_stickiness.value;
            UpdateParamsTextsFromValues();
        }
        public void OnSlVal_PercentTiles() {
            rgp.PercentTiles = sl_percentTiles.value;
            UpdateParamsTextsFromValues();
        }
        

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void LoadCustLayoutsJson() {
            string saveKey = SaveKeys.SlideAndStick_Debug_CustomLayouts;
            string saveString = SaveStorage.GetString(saveKey);
            custLayouts = JsonUtility.FromJson<BoardDataList>(saveString);
            if (custLayouts == null) { custLayouts = new BoardDataList(); }
        }
        private void SaveCustLayoutsJson() {
            string saveKey = SaveKeys.SlideAndStick_Debug_CustomLayouts;
            string saveString = JsonUtility.ToJson(custLayouts);
            SaveStorage.SetString(saveKey, saveString);
        }
        
        public void MakeNewLayout() {
            level.GameController.ReloadScene();
        }
        
        private void CopyLayoutsAsXMLToClipboard() {
            string str = "";
            foreach (BoardData bd in custLayouts.bds) {
                str += bd.Debug_GetAsXML(true);
            }
            GameUtils.CopyToClipboard(str);
        }
        private void ShowPopupText() {
            int numLayouts = custLayouts.bds.Count; //GetNumLayouts(savedLayoutXMLs);
            t_savedPopup.text = numLayouts + " layouts copied to clipboard";
            SetSavedPopupAlpha(1);
            LeanTween.value(gameObject, SetSavedPopupAlpha, 1,0, 1.4f).setDelay(1.5f);
        }
        
        public void AddLayout(int difficulty) {
            // Set the Board's difficulty and convert to BoardData.
            level.Board.Debug_SetDifficulty(difficulty);
            BoardData boardData = level.Board.SerializeAsData();
            
            // First off, if this exact layout is the SAME as the last saved fella, REMOVE the last saved fella! We're replacing it with this updated difficulty.
            if (AreLayoutsIdentical(custLayouts.LastData, boardData)) {
                custLayouts.bds.RemoveAt(custLayouts.NumDatas-1);
            }
            
            // ADD the new layout!
            custLayouts.bds.Add(boardData);
            
            // Saaaaave!
            SaveCustLayoutsJson();
            CopyLayoutsAsXMLToClipboard();
            ShowPopupText();
            HideSaveButtons();
        }
        //    string layout = level.Board.Debug_GetAsXML(true);
        //    savedLayoutXMLs += layout;
        //    SaveLayoutsString();
        
        
        
    }
}