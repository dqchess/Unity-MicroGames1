using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class LevelsManager {
    
        // Instance
        static private LevelsManager instance;
        static public LevelsManager Instance {
            get {
                if (instance==null) { instance = new LevelsManager(); }
                return instance;
            }
        }
    
        // Properties
        public LevelAddress selectedAddress = LevelAddress.undefined; // used for navigating menus! :)
        private ModeCollectionData[] modeDatas; // FindTheWord and LetterSmash.
        //private int playerCoins; // our soft currency!
    
    
        // ----------------------------------------------------------------
        //  Getters
        // ----------------------------------------------------------------
        //public int PlayerCoins { get { return playerCoins; } }
        public ModeCollectionData GetModeCollectionData (int modeIndex) {
            if (modeIndex<0 || modeIndex>=modeDatas.Length) { return null; }
            return modeDatas [modeIndex];
        }
        public PackCollectionData GetPackCollectionData (int modeIndex, int collectionIndex) {
            ModeCollectionData modeData = GetModeCollectionData (modeIndex);
            if (modeData == null) { return null; } // Safety check.
            return modeData.GetPackCollectionData (collectionIndex);
        }
        public PackData GetPackData (int modeIndex, int collectionIndex, int packIndex) {
            PackCollectionData collectionData = GetPackCollectionData (modeIndex, collectionIndex);
            if (collectionData == null) { return null; } // Safety check.
            return collectionData.GetPackData (packIndex);
        }
        public LevelData GetLevelData (int modeIndex, int collectionIndex, int packIndex, int levelIndex) {
            PackData packData = GetPackData (modeIndex, collectionIndex, packIndex);
            if (packData == null) { return null; } // Safety check.
            return packData.GetLevelData (levelIndex);
        }
        public bool DidCompleteLevel (int modeIndex, int collectionIndex, int packIndex, int levelIndex) {
            LevelData levelData = GetLevelData (modeIndex, collectionIndex, packIndex, levelIndex);
            if (levelData == null) { return false; } // Safety check.
            return levelData.DidCompleteLevel;
        }
        public PackCollectionData GetPackCollectionData (LevelAddress address) {
            return GetPackCollectionData (address.mode, address.collection);
        }
        public PackData GetPackData (LevelAddress address) {
            return GetPackData (address.mode, address.collection, address.pack);
        }
        public LevelData GetLevelData (LevelAddress address) {
            return GetLevelData (address.mode, address.collection, address.pack, address.level);
        }
    
        public LevelAddress GetLastPlayedLevelAddress() {
            // Save data? Use it!
            if (SaveStorage.HasKey (SaveKeys.SlideAndStick_LastPlayedLevelAddress)) { 
                return LevelAddress.FromString (SaveStorage.GetString (SaveKeys.SlideAndStick_LastPlayedLevelAddress));
            }
            // No save data. Default to the first level, I guess.
            else {
                return new LevelAddress(0,0,0,0);
            }
        }
    
    
        // ----------------------------------------------------------------
        //  Setters
        // ----------------------------------------------------------------
        //public void SetPlayerCoins(int _numCoins) {
            //playerCoins = _numCoins;
            //SaveStorage.SetInt(SaveKeys.PlayerCoins, PlayerCoins);
            //if (!GameManagers.IsInitializing) {
            //    GameManagers.Instance.EventManager.OnSetPlayerCoins(PlayerCoins);
            //}
        //}
        //public void ChangePlayerCoins(int _change) {
        //    SetPlayerCoins(playerCoins + _change);
        //}
    
    
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public LevelsManager() {
            Reset ();
        }
        private void Reset () {
            ReloadModeDatas ();
            //playerCoins = SaveStorage.GetInt(SaveKeys.PlayerCoins, GameProperties.NumStartingCoins);
        }
    
    
        public void ReloadModeDatas () {
            modeDatas = new ModeCollectionData[2];
    
            modeDatas[GameModes.TutorialIndex] = new ModeCollectionData(GameModes.TutorialIndex, GameModes.Tutorial, "Tutorial");
            modeDatas[GameModes.StandardIndex] = new ModeCollectionData(GameModes.StandardIndex, GameModes.Standard, "Standard");
        }
    
    
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void OnCompleteLevel (LevelAddress levelAddress) {
            PackData packData = GetPackData (levelAddress);
            packData.OnCompleteLevel (levelAddress);
        }
        public void ClearAllSaveData() {
            // NOOK IT
            SaveStorage.DeleteAll ();
            Reset ();
            Debug.Log ("All SaveStorage CLEARED!");
        }
    
    
    
    
    }
}
