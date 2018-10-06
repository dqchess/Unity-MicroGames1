using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	abstract public class BoardOccupant : BoardObject {
		// Properties
		private List<Vector2Int> footprintLocal; // at least contains Vector2Int.zero.

		// Getters (Public)
		public List<Vector2Int> FootprintLocal { get { return footprintLocal; } }
		public List<Vector2Int> GetFootprintGlobal() {
			List<Vector2Int> footprintGlobal = new List<Vector2Int>();
			for (int i=0; i<footprintLocal.Count; i++) {
				footprintGlobal.Add(new Vector2Int(Col+footprintLocal[i].x, Row+footprintLocal[i].y));
			}
			return footprintGlobal;
		}


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
			for (int i=0; i<footprintLocal.Count; i++) {
				GetSpace(Col+footprintLocal[i].x, Row+footprintLocal[i].y).SetMyOccupant(this);
			}
		}
		override public void RemoveMyFootprint () {
			for (int i=0; i<footprintLocal.Count; i++) {
				GetSpace(Col+footprintLocal[i].x, Row+footprintLocal[i].y).RemoveMyOccupant(this);
			}
		}

		public void AppendMyFootprint(List<Vector2Int> newFootGlobal) {
			// Remove me from the board, append my footprint, and put me back down.
			RemoveMyFootprint();

			for (int i=0; i<newFootGlobal.Count; i++) {
				footprintLocal.Add(new Vector2Int(newFootGlobal[i].x-Col, newFootGlobal[i].y-Row));
			}

			AddMyFootprint();
		}


	}
}