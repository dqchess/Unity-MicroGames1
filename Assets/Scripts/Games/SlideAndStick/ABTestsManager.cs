using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class ABTestsManager {
        // Properties
        public static bool IsInitializing { get; private set; } = false;
        public bool IsEasies { get; private set; }

        /// Only for recompiling during runtime! I'm bonked after scripts recompile. Call this to hard-reset me.
        public static void Reinitialize() {
            IsInitializing = false;
            instance = null;
        }
    
    
        // Constructor / Initialize
        private ABTestsManager() {
            LoadLevDiffType();
        }
        private void LoadLevDiffType() {
            // We DO have value saved! Load IT!
            if (SaveStorage.HasKey(SaveKeys.SlideAndStick_ABTest_IsEasies)) {
                IsEasies = SaveStorage.GetBool(SaveKeys.SlideAndStick_ABTest_IsEasies);
            }
            // We DON'T have save value. Determine if we should!
            else {
                // This user HAS completed the tutorial, so they've already seen the original levels-- keep them as YES easies.
                if (SaveStorage.GetBool(SaveKeys.SlideAndStick_DidCompleteTutorial)) {
                    IsEasies = true;
                }
                // This user HASN'T completed tutorial. Randomly assign them to group A or B!
                else {
                    IsEasies = MathUtils.RandBool();
                    Debug.Log("Randomly assigning AB-test IsEasies: " + IsEasies);
                }
                // Save the value!
                SaveStorage.SetBool(SaveKeys.SlideAndStick_ABTest_IsEasies, IsEasies);
            }
        }
    
    
    
        // Instance
        static private ABTestsManager instance;
        static public ABTestsManager Instance {
            get {
                if (instance==null) {
                    // We're ALREADY initializing?? Uh-oh. Return null, or we'll be caught in an infinite loop of recursion!
                    if (IsInitializing) {
                        Debug.LogError ("ABTestsManager access loop infinite recursion error! It's trying to access itself before it's done being initialized.");
                        return null; // So the program doesn't freeze.
                    }
                    else {
                        IsInitializing = true;
                        instance = new ABTestsManager();
                    }
                }
                else if (IsInitializing) {
                    IsInitializing = false; // Don't HAVE to update this value at all, but it's nice to for accuracy.
                }
                return instance;
            }
        }
    
    }
}
