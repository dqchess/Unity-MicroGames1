using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
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
        private string savedLayoutXMLs; // all the XML nodes in one big-ass string.
        // References
        [SerializeField] private Level level=null;
        
        // Getters (Private)
        private GameController gameController { get { return level.GameController; } }
        private RandGenParams rgp { get { return gameController.randGenParams; } }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            // Load layouts string!
            savedLayoutXMLs = SaveStorage.GetString(SaveKeys.SlideAndStick_Debug_SavedLayouts);
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
        private void SaveLayoutsString() {
            SaveStorage.SetString(SaveKeys.SlideAndStick_Debug_SavedLayouts, savedLayoutXMLs);
            // Automatically copy it to our clipboard, Joe.
            GameUtils.CopyToClipboard(savedLayoutXMLs);
            // Show the popup text, mm!
            int numLayouts = TextUtils.CountOccurances(savedLayoutXMLs, "layout=");
            t_savedPopup.text = numLayouts + " layouts copied to clipboard";
            SetSavedPopupAlpha(1);
            LeanTween.value(gameObject, SetSavedPopupAlpha, 1,0, 1.4f).setDelay(1.5f);
        }
        
        public void MakeNewLayout() {
            level.GameController.ReloadScene();
        }
        
        public void AddLayout(int difficulty) {
            // Add to the big string and save!
            level.Board.Debug_SetDifficulty(difficulty);
            string layout = level.Board.Debug_GetAsXML(true);
            savedLayoutXMLs += layout;
            SaveLayoutsString();
            // Hide the save buttons now.
            HideSaveButtons();
        }
        
        
        
    }
}