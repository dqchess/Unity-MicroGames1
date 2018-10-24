using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
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
        public LevelAddress selectedAddress = LevelAddress.zero; // used for navigating menus! :)
        private ModeCollectionData[] modeDatas; // currently just Tutorial and Standard!
    
    
        // ----------------------------------------------------------------
        //  Getters
        // ----------------------------------------------------------------
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
            string key = SaveKeys.AbacusToy_LastPlayedLevelAddress(selectedAddress);
            if (SaveStorage.HasKey(key)) { 
                return LevelAddress.FromString (SaveStorage.GetString (key));
            }
            // No save data. Default to the first level, I guess.
            else {
                return new LevelAddress(0,0,0,0);
            }
        }
        
        public int GetNumLevelsPlayable(int collection) {
            PackCollectionData packCollectionData = GetPackCollectionData(0, collection);
            return packCollectionData.PackDatas[0].NumLevelsPlayable;
        }
        public int GetNumLevelsCompleted(int collection) {
            PackCollectionData packCollectionData = GetPackCollectionData(0, collection);
            return packCollectionData.PackDatas[0].NumLevelsCompleted;
        }
    
    
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public LevelsManager() {
            Reset ();
        }
        private void Reset () {
            ReloadModeDatas ();
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
