using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlideAndStick {
    public class LevelsManager {
        // Constants
        //private static readonly LevelAddress TutorialPackAddress = new LevelAddress(GameModes.StandardIndex, 2, 0, 0);
        //private static readonly LevelAddress FallbackLevelAddress = new LevelAddress(GameModes.StandardIndex, 3, 0, 0);
        // Properties
        public LevelAddress selectedAddress = LevelAddress.undefined; // used for navigating menus! :)
        public AllLevelsData AllLevelsData { get; private set; } // Contains all collections, packs, and levels.
    
        // Instance
        static private LevelsManager instance;
        static public LevelsManager Instance {
            get {
                if (instance==null) { instance = new LevelsManager(); }
                return instance;
            }
        }
    
    
        // ----------------------------------------------------------------
        //  Getters
        // ----------------------------------------------------------------
        private LevelAddress FallbackLevelAddress() { // Note: Currently these are the same, but they could differ.
            if (ABTestsManager.Instance.IsEasies) { return new LevelAddress(0, 3, 0, 0); }
            return new LevelAddress(0, 3, 0, 0);
        }
        private LevelAddress TutorialPackAddress() { // Note: Currently these are the same, but they could differ.
            if (ABTestsManager.Instance.IsEasies) { return new LevelAddress(0, 2, 0, 0); }
            return new LevelAddress(0, 2, 0, 0);
        }
        public PackCollectionData GetPackCollectionData (int collectionIndex) {
            return AllLevelsData.GetPackCollectionData (collectionIndex);
        }
        public PackData GetPackData (int collectionIndex, int packIndex) {
            PackCollectionData collectionData = GetPackCollectionData (collectionIndex);
            if (collectionData == null) { return null; } // Safety check.
            return collectionData.GetPackData (packIndex);
        }
        public LevelData GetLevelData (int collectionIndex, int packIndex, int levelIndex) {
            PackData packData = GetPackData (collectionIndex, packIndex);
            if (packData == null) { return null; } // Safety check.
            return packData.GetLevelData (levelIndex);
        }
        public bool DidCompleteLevel (int collectionIndex, int packIndex, int levelIndex) {
            LevelData levelData = GetLevelData (collectionIndex, packIndex, levelIndex);
            if (levelData == null) { return false; } // Safety check.
            return levelData.DidCompleteLevel;
        }
        public PackCollectionData GetPackCollectionData (LevelAddress address) {
            return GetPackCollectionData (address.collection);
        }
        public PackData GetPackData (LevelAddress address) {
            return GetPackData (address.collection, address.pack);
        }
        public LevelData GetLevelData (LevelAddress address) {
            return GetLevelData (address.collection, address.pack, address.level);
        }
    
        private LevelAddress GetLastPlayedLevelAddress() {
            if (selectedAddress == LevelAddress.undefined) { selectedAddress = LevelAddress.zero; } // hacky make sure it's not -1s. (Why do we even have it start at undefined?..)
            // Save data? Use it!
            string key = SaveKeys.SlideAndStick_LastPlayedLevelGlobal;//Local(selectedAddress);
            if (SaveStorage.HasKey(key)) { 
                return LevelAddress.FromString (SaveStorage.GetString (key));
            }
            // No save data. Default to the tutorial!
            else {
                return TutorialPackAddress();
            }
        }
        
        /** Rolls over to the first lvl in the next pack, or collection. */
        public LevelData GetRolloverPackNextLevelData(LevelAddress src) {
            LevelAddress nextPackAdr = new LevelAddress(src.mode, src.collection, src.pack+1, 0);
            // There's a pack after this! Roll into it.
            if (GetPackData(nextPackAdr) != null) {
                return GetLevelData(nextPackAdr);
            }
            // No pack after this. Try the next collection, then.
            else {
                LevelAddress nextCollAdr = new LevelAddress(src.mode, src.collection+1, 0,0);
                if (GetPackCollectionData(nextCollAdr) != null) {
                    return GetLevelData(nextCollAdr);
                }
            }
            // We were handed the final pack of the final collection? Return null: No level comes next.
            return null;
        }
        
        private bool DoesLevelExist(LevelAddress address) {
            return AllLevelsData.DoesLevelExist(address);
        }
        public bool IsLastLevelInPack(LevelAddress address) {
            // Return TRUE if the next level is outta bounds!
            return !DoesLevelExist(address.NextLevel);
        }
        public bool IsTutorial(LevelAddress address) {
            LevelAddress tutAddr = TutorialPackAddress();
            return address.collection == tutAddr.collection
                && address.pack == tutAddr.pack;
        }
        
        public LevelData GetFallbackEmptyLevelData() {
            return new LevelData {
                myAddress = LevelAddress.zero,
                boardData = new BoardData(1,1)
            };
        }
        
        public LevelData GetLastPlayedLevelData() {
            LevelAddress lastPlayedAdd = GetLastPlayedLevelAddress();
            LevelData ld = GetLevelData(lastPlayedAdd);
            if (ld == null) { // Oh, this level doesn't exist. Return the first Beginner level, I guess.
                ld = GetLevelData(FallbackLevelAddress());
            }
            return ld;
        }
        
    
    
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public LevelsManager() {
            Reset ();
        }
        public void Reset () {
            AllLevelsData = new AllLevelsData();
            if (Application.isEditor) {
                //Debug_PrintTotalNumLevels();
                //Debug_PrintNumLevelsInEachPack();
                Debug_PrintDuplicateLevelLayouts();
                //Debug_PrintAlreadySatisfiedTileLayouts();
            }
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
        ///** Dangerous function! Powerful. This overwrites our Board layout in the XML file. */
        //public void Debug_SaveReplaceXMLLayout(Level level) {
        //    // First, find the file.
            
        //}
        private void Debug_PrintTotalNumLevels() {
            Debug.Log("Total SlideAndStick levels: " + AllLevelsData.NumLevels());
        }
        private void Debug_PrintNumLevelsInEachPack() {
            string str = "TOTALS:\n";
            foreach (PackCollectionData pcData in AllLevelsData.CollectionDatas) {
                foreach (PackData pData in pcData.PackDatas) {
                    str += pcData.CollectionName + ", " + pData.PackName + " total: " + pData.NumLevels + "\n";
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
            foreach (PackCollectionData pcData in AllLevelsData.CollectionDatas) {
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
        private void Debug_PrintAlreadySatisfiedTileLayouts() {
            foreach (PackCollectionData pcData in AllLevelsData.CollectionDatas) {
                // HARDCODED skip rand/test/tutorial lvls.
                if (pcData.MyAddress.collection < 2) { continue; }
                foreach (PackData pData in pcData.PackDatas) {
                    foreach (LevelData levelData in pData.LevelDatas) {
                        // You know, skip empty layouts, ok?
                        if (levelData.boardData.tileDatas.Count == 0) { continue; }
                        // This layout's not empty! ;)
                        Board board = new Board(levelData.boardData);
                        if (board.AreAnyTileColorsSatisfied()) {
                            string xmlLayout = levelData.boardData.Debug_GetLayout(true);
                            Debug.LogWarning("Already-satisfied-tile-color layout! Layout: " + xmlLayout);
                        }
                    }
                }
            }
        }
    
    
    
    
    }
}
