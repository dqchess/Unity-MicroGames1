using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class Debug_LevelsCountUI : MonoBehaviour {
        // Constants
        private const int Index_Collection_Tests = 1;
        private const int Index_Collection_Tutorial = 2;
        private const int Index_Collection_D1 = 3;
        // Components
        [SerializeField] private Text t_counts=null;
        // Properties
        private bool doShowRemaining = true;
        private Dictionary<LevelAddress,int> unsortedLvlCounts; // levels in the first pack in Tests.xml!
        private Dictionary<LevelAddress,int> targetLvlCounts; // how many of each we WANT to make for game's release.
        
        // Getters (Private)
        private LevelsManager lm { get { return LevelsManager.Instance; } }
        private int GetNumUnsortedLvls(LevelAddress address) {
            if (unsortedLvlCounts.ContainsKey(address)) { return unsortedLvlCounts[address]; }
            return 0;
        }
        /** Very fragile and hardcoded. Nbd, 'cause it's just for dev lvl-making purposes. */
        private LevelAddress GetPackAddress(int diff, int numCols) {
            int mode = GameModes.StandardIndex;
            ModeCollectionData mcd = lm.GetModeCollectionData(mode);
            PackCollectionData pcd = mcd.GetPackCollectionData(diff + (Index_Collection_D1-1)); // we know there're X collections before D1.
            foreach (PackData pd in pcd.PackDatas) {
                // Correct size!
                if (pd.NumLevels>0 && pd.GetLevelData(0).boardData.numCols == numCols) {
                    return pd.MyAddress;
                }
            }
            //// Collection?
            //for (int i=0; i<mcd.NumPackCollections; i++) {
            //    PackCollectionData pcd = mcd.GetPackCollectionData(i);
            //    // Correct difficulty!
            //    if (pcd.PackDatas.Count>0 && pcd.PackDatas[0].LevelDatas.Count>0 && pcd.PackDatas[0].LevelDatas[0].boardData.difficulty == diff) {
            //        foreach (PackData pd in pcd.PackDatas) {
            //            // Correct size!
            //            if (pd.NumLevels>0 && pd.GetLevelData(0).boardData.numCols == numCols) {
            //                return pd.MyAddress;
            //            }
            //        }
            //    }
            //}
            // Hmm.
            return LevelAddress.undefined;
        }
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            SetTargetLvlCounts();
            UpdateUnsortedLvlCounts();
            UpdateCountsText();
        }
        
        private void SetTargetLvlCounts() {
            targetLvlCounts = new Dictionary<LevelAddress, int>();
            targetLvlCounts[GetPackAddress(Difficulties.Beginner, 4)]   = 50;
            targetLvlCounts[GetPackAddress(Difficulties.Beginner, 5)]   = 50;
            targetLvlCounts[GetPackAddress(Difficulties.Easy, 4)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Easy, 5)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Easy, 6)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Med, 4)]        = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Med, 5)]        = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Med, 6)]        = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Hard, 4)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Hard, 5)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.Hard, 6)]       = 100;
            targetLvlCounts[GetPackAddress(Difficulties.DoubleHard, 4)] = 50;
            targetLvlCounts[GetPackAddress(Difficulties.DoubleHard, 5)] = 50;
            targetLvlCounts[GetPackAddress(Difficulties.DoubleHard, 6)] = 50;
            targetLvlCounts[GetPackAddress(Difficulties.Impossible, 4)] = 25;
            targetLvlCounts[GetPackAddress(Difficulties.Impossible, 5)] = 25;
            targetLvlCounts[GetPackAddress(Difficulties.Impossible, 6)] = 25;
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void ToggleDoShowRemaining() {
            doShowRemaining = !doShowRemaining;
            UpdateCountsText();
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void UpdateUnsortedLvlCounts() {
            unsortedLvlCounts = new Dictionary<LevelAddress,int>();
            PackData unsortedPackData = lm.GetPackData(GameModes.StandardIndex, Index_Collection_Tests, 0);
            foreach (LevelData ld in unsortedPackData.LevelDatas) {
                // Size and difficulty?
                int diff = ld.boardData.difficulty;
                int numCols = ld.boardData.numCols;
                LevelAddress address = GetPackAddress(diff, numCols);
                // Update the dict!
                if (!unsortedLvlCounts.ContainsKey(address)) {
                    unsortedLvlCounts[address] = 0;
                }
                unsortedLvlCounts[address] ++;
            }
        }
        private void UpdateCountsText() {
            string str = "";
            ModeCollectionData mcdStandard = lm.GetModeCollectionData(GameModes.StandardIndex);
            
            str += GetTextLine(Difficulties.Beginner, 4);
            str += GetTextLine(Difficulties.Easy, 4);
            str += GetTextLine(Difficulties.Med, 4);
            str += GetTextLine(Difficulties.Hard, 4);
            str += GetTextLine(Difficulties.DoubleHard, 4);
            str += GetTextLine(Difficulties.Impossible, 4);
            
            str += GetTextLine(Difficulties.Beginner, 5);
            str += GetTextLine(Difficulties.Easy, 5);
            str += GetTextLine(Difficulties.Med, 5);
            str += GetTextLine(Difficulties.Hard, 5);
            str += GetTextLine(Difficulties.DoubleHard, 5);
            str += GetTextLine(Difficulties.Impossible, 5);
            
            str += GetTextLine(Difficulties.Easy, 6);
            str += GetTextLine(Difficulties.Med, 6);
            str += GetTextLine(Difficulties.Hard, 6);
            str += GetTextLine(Difficulties.DoubleHard, 6);
            str += GetTextLine(Difficulties.Impossible, 6);
            t_counts.text = str;
        }
        //private void UpdateCountsText() {
        //    string str = "";
        //    ModeCollectionData mcdStandard = lm.GetModeCollectionData(GameModes.StandardIndex);
        //    for (int i=Index_Collection_Tutorial; i<mcdStandard.CollectionDatas.Count; i++) { // start at Tutorial collection.
        //        PackCollectionData pcData = mcdStandard.CollectionDatas[i];
        //        foreach (PackData pData in pcData.PackDatas) {
        //            int numOfficialLvls = pData.NumLevels;
        //            int numUnsortedLvls = GetNumUnsortedLvls(pData.MyAddress);
        //            int numMade = numOfficialLvls + numUnsortedLvls;
        //            string namePrefix = pcData.CollectionName + " " + pData.PackName + ":    ";
        //            if (doShowRemaining) {
        //                int numToMake = targetLvlCounts.ContainsKey(pData.MyAddress) ? targetLvlCounts[pData.MyAddress] : 0;
        //                int numRemaining = numToMake - numMade;
        //                str += namePrefix + numRemaining;
        //            }
        //            else {
        //                str += namePrefix + numOfficialLvls;
        //                if (numUnsortedLvls > 0) { str += "+" + numUnsortedLvls; }
        //            }
        //            str += "\n";
        //        }
        //    }
        //    t_counts.text = str;
        //}
        
        private string GetTextLine(int diff, int numCols) {
            LevelAddress address = GetPackAddress(diff, numCols);
            PackCollectionData pcd = lm.GetPackCollectionData(address);
            PackData pd = lm.GetPackData(address);
            if (pd == null) { return "undefined"; } // Safety check.
            string str = "";
            int numOfficialLvls = pd.NumLevels;
            int numUnsortedLvls = GetNumUnsortedLvls(pd.MyAddress);
            int numMade = numOfficialLvls + numUnsortedLvls;
            string namePrefix = pcd.CollectionName + " " + pd.PackName + ":         ";
            if (doShowRemaining) {
                int numToMake = targetLvlCounts.ContainsKey(pd.MyAddress) ? targetLvlCounts[pd.MyAddress] : 0;
                int numRemaining = numToMake - numMade;
                str += namePrefix + numRemaining;
            }
            else {
                str += namePrefix + numOfficialLvls;
                if (numUnsortedLvls > 0) { str += "+" + numUnsortedLvls; }
            }
            str += "\n";
            return str;
        }
        
    }
}