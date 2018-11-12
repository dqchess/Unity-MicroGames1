using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (selectedAddress == LevelAddress.undefined) { selectedAddress = LevelAddress.zero; } // hacky make sure it's not -1s. (Why do we even have it start at undefined?..)
            // Save data? Use it!
            string key = SaveKeys.SlideAndStick_LastPlayedLevelAddress(selectedAddress);
            if (SaveStorage.HasKey(key)) { 
                return LevelAddress.FromString (SaveStorage.GetString (key));
            }
            // No save data. Default to the first level, I guess.
            else {
                return new LevelAddress(0,0,0,0);
            }
        }
        
        private bool DoesLevelExist(LevelAddress address) {
            if (address.mode >= modeDatas.Length) { return false; } // Outta bounds? Return false!
            return modeDatas[address.mode].DoesLevelExist(address); // Ok, ask the next guy.
        }
        public bool IsLastLevelInPack(LevelAddress address) {
            // Return TRUE if the next level is outta bounds!
            return !DoesLevelExist(address.NextLevel);
        }
        
        public LevelData GetFallbackEmptyLevelData() {
            return new LevelData {
                myAddress = new LevelAddress(0, 0, 0, 0),
                boardData = new BoardData(1, 1)
            };
        }
        
        // TODO: Remove or fix these! They're incorrect.
        public int GetNumLevelsPlayable(int collection) {
            PackCollectionData packCollectionData = GetPackCollectionData(0, collection);
            return packCollectionData.PackDatas[0].NumLevelsPlayable;
        }
        public int GetNumLevelsCompleted(int collection) {
            PackCollectionData packCollectionData = GetPackCollectionData(0, collection);
            return packCollectionData.PackDatas[0].NumLevelsCompleted;
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
        public void Reset () {
            ReloadModeDatas ();
            //playerCoins = SaveStorage.GetInt(SaveKeys.PlayerCoins, GameProperties.NumStartingCoins);
            //Debug_PrintTotalNumLevels();
            //Debug_PrintNumLevelsInEachPack();
            Debug_PrintDuplicateLevelLayouts();
        }
    
    
        private void ReloadModeDatas () {
            modeDatas = new ModeCollectionData[GameModes.NumModes];
    
            //modeDatas[GameModes.TutorialIndex] = new ModeCollectionData(GameModes.TutorialIndex, GameModes.Tutorial, "Tutorial");
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
        
        
        
        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        /** Dangerous function! Powerful. This overwrites our Board layout in the XML file. */
        public void Debug_SaveReplaceXMLLayout(Level level) {
            // First, find the file.
            
        }
        private void Debug_PrintTotalNumLevels() {
            int total=0;
            foreach (ModeCollectionData modeCollectionData in modeDatas) {
                total += modeCollectionData.NumLevels();
            }
            Debug.Log("Total SlideAndStick levels: " + total);
        }
        private void Debug_PrintNumLevelsInEachPack() {
            string str = "TOTALS:\n";
            foreach (ModeCollectionData mcData in modeDatas) {
                foreach (PackCollectionData pcData in mcData.CollectionDatas) {
                    foreach (PackData pData in pcData.PackDatas) {
                        str += pcData.CollectionName + ", " + pData.PackName + " total: " + pData.NumLevels + "\n";
                    }
                }
            }
            Debug.Log(str);
        }
        public void Debug_OrderLevelsAndCopyToClipboard(LevelAddress address) {
            PackData packData = GetPackData(address);
            // Order list by difficulty/size!
            List<LevelData> ldsSorted = packData.LevelDatas.OrderBy(o=>o.boardData.difficulty).ThenBy(o=>o.boardData.numCols*o.boardData.numRows).ToList();
            // Pack them into a big-ass string, yo.
            string str = "";
            foreach (LevelData ld in ldsSorted) {
                str += ld.boardData.Debug_GetAsXML(true);
            }
            // Copy them to the clip-clopboard!
            GameUtils.CopyToClipboard(str);
        }
        private void Debug_PrintDuplicateLevelLayouts() {
            Dictionary<string,LevelData> layoutDict = new Dictionary<string, LevelData>();
            foreach (ModeCollectionData mcData in modeDatas) {
                foreach (PackCollectionData pcData in mcData.CollectionDatas) {
                    foreach (PackData pData in pcData.PackDatas) {
                        foreach (LevelData levelData in pData.LevelDatas) {
                            // You know, skip empty layouts, ok?
                            if (levelData.boardData.tileDatas.Count == 0) { continue; }
                            // This layout's not empty! ;)
                            string xmlLayout = levelData.boardData.Debug_GetLayout(true);
                            if (!layoutDict.ContainsKey(xmlLayout)) {
                                layoutDict.Add(xmlLayout, levelData);
                            }
                            else {
                                //string addA = layoutDict[xmlLayout].myAddress.ToString();
                                //string addB = layoutDict[xmlLayout].myAddress.ToString();
                                Debug.LogWarning("Duplicate level layout! Layout: " + xmlLayout);//Addresses: " + addA + ", " + addB);
                            }
                        }
                    }
                }
            }
        }
    
    
    
    
    }
}
