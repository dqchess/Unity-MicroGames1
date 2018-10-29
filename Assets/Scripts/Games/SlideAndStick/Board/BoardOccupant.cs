using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	abstract public class BoardOccupant : BoardObject {
        // Properties
        public List<Vector2Int> FootprintLocal { get; private set; } // at least contains Vector2Int.zero.
        public List<Vector2Int> FootprintGlobal { get; private set; } // just footprintLocal, plus my boardPos. Updated when A) boardPos changes, and B) footprintLocal changes.
        public bool DidJustMove; // set to TRUE when we move, OR if we append my footprint with another Occupant that's just moved!
        // Properties
        public List<Vector2> MergePosesLocal { get; private set; } // the BETWEEN-space poses where I connect to myself.

        // Getters (Public)
        public bool HasMergePosLocal(Vector2 mpLocal) {
            return MergePosesLocal.Contains(mpLocal);
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        protected void InitializeAsBoardOccupant (Board _boardRef, BoardOccupantData _data) {
			FootprintLocal = _data.footprintLocal;
            base.InitializeAsBoardObject (_boardRef, _data.boardPos);
            RemakeMergePoses();
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		override public void AddMyFootprint () {
			for (int i=0; i<FootprintGlobal.Count; i++) {
                BoardSpace space = GetSpace(FootprintGlobal[i].x, FootprintGlobal[i].y);
                if (space != null) {
                    space.SetMyOccupant(this);
                }
			}
		}
		override public void RemoveMyFootprint () {
			for (int i=0; i<FootprintGlobal.Count; i++) {
                BoardSpace space = GetSpace(FootprintGlobal[i].x, FootprintGlobal[i].y);
                if (space != null) {
                    space.RemoveMyOccupant(this);
                }
			}
		}
		override public void SetColRow (int _col, int _row) {
            if (Col!=_col || Row!=_row) { DidJustMove = true; } // Yep, we just moved!
			base.SetColRow(_col,_row);
			UpdateFootprintGlobal();
		}

		public void AppendMyFootprint(BoardOccupant otherOccupant) {
            List<Vector2Int> newFootGlobal = new List<Vector2Int>(otherOccupant.FootprintGlobal); // note: copy it so we don't modify the original.
            // Update if I've just moved if the other guy has!
            DidJustMove = DidJustMove || otherOccupant.DidJustMove;
            
			// Remove me from the board, append my footprint, and put me back down.
			RemoveMyFootprint();
			// Append footprintLocal!
			for (int i=0; i<newFootGlobal.Count; i++) {
				FootprintLocal.Add(new Vector2Int(newFootGlobal[i].x-Col, newFootGlobal[i].y-Row));
			}
			// Update footprintGlobal now.
			UpdateFootprintGlobal();
			// Put me back on the Board!
			AddMyFootprint();
            //// Remake my mergePosesLocal!
            //RemakeMergePosesLocal();
		}
		private void UpdateFootprintGlobal() {
			FootprintGlobal = new List<Vector2Int>(FootprintLocal);
			for (int i=0; i<FootprintLocal.Count; i++) {
				FootprintGlobal[i] += new Vector2Int(Col,Row);
			}
		}
        
        public void RemakeMergePoses() {
            MergePosesLocal = new List<Vector2>();
            foreach (Vector2Int fpLocal in FootprintLocal) {
                MaybeAddMergePosLocal(fpLocal, Vector2Int.R);
                MaybeAddMergePosLocal(fpLocal, Vector2Int.B);
            }
        }
        private void MaybeAddMergePosLocal(Vector2Int fpLocal, Vector2Int dir) {
            Vector2Int otherPos = fpLocal + dir;
            // There IS another footprint here...!
            if (FootprintLocal.Contains(otherPos)) {
                Vector2Int fpGlobal = fpLocal+BoardPos.ToVector2Int();
                // These two spots DO connect...!
                if (BoardUtils.CanOccupantEverExit(BoardRef, fpGlobal, dir)) {
                    Vector2 betweenPos = fpLocal.ToVector2() + dir.ToVector2()*0.5f;
                    MergePosesLocal.Add(betweenPos);
                }
            }
        }
        
        //public void RemoveMergePosGlobal(Vector2 mpGlobal) {
        //    Vector2 mpLocal = mpGlobal - BoardPos.ToVector2();
        //    if (HasMergePosLocal(mpLocal)) {
        //        MergePosesLocal.Remove(mpLocal);
        //    }
        //    else {
        //        Debug.LogError("Whoa! Trying to remove a mergePosLocal from a Tile, but it doesn't have that mergePos!");
        //    }
        //}


	}
}