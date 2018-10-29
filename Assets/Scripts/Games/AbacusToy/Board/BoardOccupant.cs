using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
	[System.Serializable]
	abstract public class BoardOccupant : BoardObject {
		// Properties
        public int GroupID { get; set; } // For finding "islands" of BoardOccupants.
		private List<Vector2Int> footprintLocal; // at least contains Vector2Int.zero.
		private List<Vector2Int> footprintGlobal; // just footprintLocal, plus my boardPos. Updated when A) boardPos changes, and B) footprintLocal changes.

		// Getters (Public)
		public List<Vector2Int> FootprintLocal { get { return footprintLocal; } }
		public List<Vector2Int> FootprintGlobal { get { return footprintGlobal; } }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		protected void InitializeAsBoardOccupant (Board _boardRef, BoardOccupantData _data) {
			footprintLocal = _data.footprintLocal;
			base.InitializeAsBoardObject (_boardRef, _data.boardPos);
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
            boardRef.NumFootprintsDown ++;
        }
        override public void RemoveMyFootprint () {
            for (int i=0; i<FootprintGlobal.Count; i++) {
                BoardSpace space = GetSpace(FootprintGlobal[i].x, FootprintGlobal[i].y);
                if (space != null) {
                    space.RemoveMyOccupant(this);
                }
            }
            boardRef.NumFootprintsDown --;
        }
		override public void SetColRow (int _col, int _row) {
			base.SetColRow(_col,_row);
			UpdateFootprintGlobal();
		}

		public void AppendMyFootprint(List<Vector2Int> newFootGlobal) {
			// Remove me from the board, append my footprint, and put me back down.
			RemoveMyFootprint();
			// Append footprintLocal!
			for (int i=0; i<newFootGlobal.Count; i++) {
				footprintLocal.Add(new Vector2Int(newFootGlobal[i].x-Col, newFootGlobal[i].y-Row));
			}
			// Update footprintGlobal now.
			UpdateFootprintGlobal();
			// Put me back on the Board!
			AddMyFootprint();
		}
		private void UpdateFootprintGlobal() {
			footprintGlobal = new List<Vector2Int>(footprintLocal);
			for (int i=0; i<footprintLocal.Count; i++) {
				footprintGlobal[i] += new Vector2Int(Col,Row);
			}
		}


	}
}