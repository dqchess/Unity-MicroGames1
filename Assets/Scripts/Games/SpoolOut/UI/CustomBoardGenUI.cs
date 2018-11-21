using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpoolOut {
    public class CustomBoardGenUI : MonoBehaviour {
        // Components
        [SerializeField] private CanvasGroup cg_savedPopup=null;
        [SerializeField] private GameObject go_params=null;
        [SerializeField] private GameObject go_paintPallete=null;
        [SerializeField] private GameObject go_saveButtons=null;
        [SerializeField] private Slider sl_minPathLength=null;
        [SerializeField] private TextMeshProUGUI t_minPathLength=null;
        [SerializeField] private TextMeshProUGUI t_savedPopup=null;
        // Properties
        private string customLayoutXMLs; // all the XML nodes in one big-ass string.
        // References
        [SerializeField] private Level level=null;
        
        // Getters (Public)
        static public int GetNumLayouts(string savedLayoutsString) {
            return TextUtils.CountOccurances(savedLayoutsString, "layout=");
        }
        // Getters (Private)
        private GameController gameController { get { return level.GameController; } }
        private RandGenParams rgp { get { return gameController.randGenParams; } }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            // Load layouts!
            LoadCustomLayouts();
            // Load params!
            UpdateSliderValuesFromParams();
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
        public void TogglePaintPaletteVisible() {
            go_paintPallete.SetActive(!go_paintPallete.activeSelf);
        }
        public void ToggleSaveButtonsVisible() {
            go_saveButtons.SetActive(!go_saveButtons.activeSelf);
        }
        private void HideSaveButtons() {
            go_saveButtons.SetActive(false);
        }
        private void UpdateParamsTextsFromValues() {
            t_minPathLength.text = rgp.MinPathLength.ToString();
        }
        private void UpdateSliderValuesFromParams() {
            sl_minPathLength.value = rgp.MinPathLength;
        }
        private void SetSavedPopupAlpha(float alpha) {
            cg_savedPopup.alpha = alpha;
        }
        
        // ----------------------------------------------------------------
        //  UI Events
        // ----------------------------------------------------------------
        public void OnSlVal_MinPathLength() {
            rgp.MinPathLength = (int)sl_minPathLength.value;
            UpdateParamsTextsFromValues();
        }
        

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void LoadCustomLayouts() {
            customLayoutXMLs = SaveStorage.GetString(SaveKeys.SpoolOut_Debug_CustomLayouts);
        }
        private void SaveCustomLayouts() {
            SaveStorage.SetString(SaveKeys.SpoolOut_Debug_CustomLayouts, customLayoutXMLs);
        }
        
        public void MakeNewLayout() {
            level.GameController.RestartLevel();
        }
        
        private void CopyLayoutsAsXMLToClipboard() {
            GameUtils.CopyToClipboard(customLayoutXMLs);
        }
        private void ShowPopupText() {
            int numLayouts = GetNumLayouts(customLayoutXMLs);
            t_savedPopup.text = numLayouts + " layouts copied to clipboard";
            SetSavedPopupAlpha(1);
            LeanTween.value(gameObject, SetSavedPopupAlpha, 1,0, 1.4f).setDelay(1.5f);
        }
        
        public void AddLayout(int difficulty) {
            // First off, REMOVE any layouts identical to this new one. We're replacing it with this updated difficulty.
            MaybeRemoveLastIdenticalLayoutFromXMLString();
        
            // Set the Board's difficulty and add to big XML string.
            level.Board.Debug_SetDifficulty(difficulty);
            customLayoutXMLs += level.Board.Debug_GetAsXML(true);
            
            // Saaaaave!
            SaveCustomLayouts();
            CopyLayoutsAsXMLToClipboard();
            ShowPopupText();
            HideSaveButtons();
        }
        private void MaybeRemoveLastIdenticalLayoutFromXMLString() {
            // First off, if this exact layout is the SAME as the last saved fella, REMOVE the last saved fella! 
            string boardLayoutString = level.Board.Debug_GetLayout(true);
            if (customLayoutXMLs.Contains(boardLayoutString)) {
                int startIndex = customLayoutXMLs.LastIndexOf("    <Level", System.StringComparison.InvariantCulture);
                customLayoutXMLs = customLayoutXMLs.Substring(0, startIndex);
            }
        }
        
        
    }
}