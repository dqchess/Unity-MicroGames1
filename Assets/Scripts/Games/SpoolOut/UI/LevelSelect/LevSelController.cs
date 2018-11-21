using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpoolOut {
	public enum MenuTransType { Push, Pop }

    public class LevSelController : MonoBehaviour {
        // References
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup cg_scrim=null;
        [SerializeField] private CoreMenuController coreMenuController=null;
        [SerializeField] private RectTransform rt_menus=null;
        [SerializeField] private TextMeshProUGUI debug_t_deleteLayouts=null;
        [SerializeField] private ToggleLevSelButton toggleLevSelButton=null;
        [SerializeField] private BaseLevSelMenu collectionsMenu=null;
        [SerializeField] private PackSelectMenu packsMenu=null;
        // Properties
        private float menusWidth;
        public float OpenLoc { get; private set; } // how open I am! From 0 to 1.
        
        // Getters (Public)
        public static Color GetCollectionColor(int collection) {
            switch (collection) {
                // Non-Player-Facing (RandLayouts, Tests, Tutorial)
                case 0: return new ColorHSB(266/360f,0.70f,0.62f).ToColor();
                case 1: return new ColorHSB( 30/360f,0.32f,0.63f).ToColor();
                case 2: return new ColorHSB(210/360f,0.44f,0.87f).ToColor();
                // Packs
                case 3: return new Color( 59/255f,229/255f,196/255f);
                case 4: return new Color( 59/255f,229/255f, 80/255f);
                case 5: return new Color(255/255f,216/255f, 78/255f);
                case 6: return new Color(250/255f, 85/255f,200/255f);
                case 7: return new Color(144/255f,144/255f,144/255f);
                case 8: return new Color( 58/255f, 58/255f, 58/255f);
                default: return Color.red; // Hmm.
            }
        }
        // Getters (Private)
        private LevelsManager lm { get { return LevelsManager.Instance; } }
    
    
    
        // ----------------------------------------------------------------
        //  Start / Destroy
        // ----------------------------------------------------------------
        private void Awake() {
            // Set values
            menusWidth = rt_menus.rect.width;
        }
		private void Start() {
			packsMenu.Close(MenuTransType.Pop); // TODO: No animations
			collectionsMenu.Open(MenuTransType.Pop);
        }
        
        
        // ----------------------------------------------------------------
        //  Open / Close
        // ----------------------------------------------------------------
        public void Open(bool doAnimate) {
            rt_menus.gameObject.SetActive(true);
            LeanTween.cancel(gameObject);
			// Refresh my PacksMenu manually, in case we beat any levels since last opening.
			packsMenu.ManualRefreshLevelButtons();
            Debug_UpdateDeleteLayoutsText();

            if (doAnimate) {
                LeanTween.value(gameObject, SetOpenLoc, OpenLoc,1, 0.5f).setEaseOutQuart();
            }
            else {
                SetOpenLoc(1);
            }
        }
        public void Close(bool doAnimate) {
            LeanTween.cancel(gameObject);
            if (doAnimate) {
                LeanTween.value(gameObject, SetOpenLoc, OpenLoc,0, 0.5f).setEaseOutQuart().setOnComplete(OnFinishClose);
            }
            else {
                OnFinishClose();
            }
        }
        private void OnFinishClose() {
            SetOpenLoc(0);
            rt_menus.gameObject.SetActive(false);
        }
    
        private void SetOpenLoc(float _loc) {
            OpenLoc = _loc;
            
            float scaleClosed = 1f;//1.4f
            float menusX = Mathf.Lerp(-menusWidth*scaleClosed,0, OpenLoc);
            rt_menus.anchoredPosition = new Vector2(menusX, rt_menus.anchoredPosition.y);
            rt_menus.localScale = Vector3.one * Mathf.Lerp(scaleClosed, 1f, OpenLoc);
//            float togButtonX = 16 + menusX + menusWidth;
            cg_scrim.alpha = OpenLoc;
            toggleLevSelButton.SetOpenLoc(OpenLoc);
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void OpenLevel(LevelAddress address) {
            coreMenuController.OpenLevel(address);
        }
        
        
        // ----------------------------------------------------------------
        //  Menus Within Me
        // ----------------------------------------------------------------
        public void ClosePacksMenu() {
			packsMenu.Close(MenuTransType.Pop);
			collectionsMenu.Open(MenuTransType.Pop);
        }
		private void ShowPackMenu(LevelAddress address) {
			packsMenu.Open(MenuTransType.Push);
			packsMenu.SetSelectedPack(address);
			collectionsMenu.Close(MenuTransType.Push);
        }
        
        
        // ----------------------------------------------------------------
        //  Button Events
        // ----------------------------------------------------------------
        public void OnCollectionButtonClick(int collectionIndex) {
            LevelAddress address = lm.selectedAddress;
            address.collection = collectionIndex;
			address.pack = 0; // Note: Previously selected packs are not remembered. (We could add that with a little effort, though.)
            address.level = 0;
            ShowPackMenu(address);
        }
        public void OnClickScrim() {
            if (OpenLoc > 0.5f) { // If I'm open and we click the scrim, close me!
                coreMenuController.CloseLevSelController(true);
            }
        }
        
        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_UpdateDeleteLayoutsText() {
            string savedLayoutsString = SaveStorage.GetString(SaveKeys.SpoolOut_Debug_CustomLayouts);
            int numLayouts = CustomBoardGenUI.GetNumLayouts(savedLayoutsString);
            debug_t_deleteLayouts.text = "delete " + numLayouts + " saved layouts";
        }
        public void CopySavedLayoutsToClipboard() {
            string savedLayoutsString = SaveStorage.GetString(SaveKeys.SpoolOut_Debug_CustomLayouts);
            GameUtils.CopyToClipboard(savedLayoutsString);
        }
        public void DeleteSavedLayouts() {
            SaveStorage.DeleteKey(SaveKeys.SpoolOut_Debug_CustomLayouts);
            coreMenuController.ReloadScene();
        }
        public void ClearAllSaveData() {
            GameManagers.Instance.DataManager.ClearAllSaveData ();
            LevelsManager.Instance.Reset();
            coreMenuController.ReloadScene();
        }
    
    }
}



