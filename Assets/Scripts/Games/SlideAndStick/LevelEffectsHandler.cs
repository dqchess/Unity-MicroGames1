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
            Transform tf_boardView = level.BoardView.transform;
            GameObject burstGO = Instantiate(ResourcesHandler.Instance.slideAndStick_tileMergeBurst);
            GameUtils.ParentAndReset(burstGO, tf_boardView);
            // Position/rotate/scale.
            burstGO.transform.localScale = Vector3.one * boardView.UnitSize/100f; // Hardcoded.
            burstGO.transform.localEulerAngles = new Vector3(0, 0, MathUtils.DirToRotation(mergeSpot.dir));
            Vector2 boardPos = mergeSpot.pos + mergeSpot.dir*0.5f; // the exact center of the merge, flat on the Board.
            boardPos += new Vector2(0, -0.08f); // push up a bit to align with Tiles' thiccness
            burstGO.transform.localPosition = boardView.BoardToLocal(boardPos);
            
            
        }
    }
}