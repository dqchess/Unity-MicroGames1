using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class LevelEffectsHandler : MonoBehaviour {
        //// Components
        //[SerializeField] private RectTransform rt_mergeBursts=null;
        // References
        [SerializeField] private Level level=null;
        private List<MergeSpot> queuedMergeSpots=new List<MergeSpot>(); // added to when call OnBoardMoveComplete. Used in OnUpdateAllViewsMoveEnd.
        
        // Getters
        private BoardView boardView { get { return level.BoardView; } }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnBoardMoveComplete() {
            // After each move is logically executed, add all the mergeSpots that happened to my list so I can make bursts for 'em when appropriate!
            for (int i=0; i<level.Board.LastMergeSpots.Count; i++) {
                queuedMergeSpots.Add(level.Board.LastMergeSpots[i]);
            }
        }
        
        public void OnUpdateAllViewsMoveEnd() {
            for (int i=0; i<queuedMergeSpots.Count; i++) {
                AddMergeBurst(queuedMergeSpots[i]);
            }
            queuedMergeSpots.Clear();
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void AddMergeBurst(MergeSpot mergeSpot) {
            TileMergeBurst burst = Instantiate(ResourcesHandler.Instance.slideAndStick_tileMergeBurst).GetComponent<TileMergeBurst>();
            burst.Initialize(level.BoardView, mergeSpot);
        }
    }
}