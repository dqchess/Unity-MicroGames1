using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	/** This class is JUST for finding InnerClusters. We convert our footprints to OccFPClusterDatas, and do recursive fill-finding. */
	class OccFPClusterData {
		public int clusterIndex=-1;
		public Vector2Int Pos { get; private set; }
		public OccFPClusterData(Vector2Int pos) {
			this.Pos = pos;
		}
	}


	[System.Serializable]
	abstract public class BoardOccupant : BoardObject {
        // Properties
        public bool DidJustMove; // set to TRUE when we move, OR if we append my footprint with another Occupant that's just moved!
        public List<Vector2Int> FootprintLocal { get; private set; } // at least contains Vector2Int.zero.
        public List<Vector2Int> FootprintGlobal { get; private set; } // just footprintLocal, plus my boardPos. Updated when A) boardPos changes, and B) footprintLocal changes.
        public List<Vector2> MergePosesLocal { get; private set; } // the BETWEEN-space poses where I connect to myself.
        public List<List<Vector2Int>> InnerClusters { get; private set; } // for splitting Occupants.

        // Getters (Public)
        public bool HasMergePosLocal(Vector2 mpLocal) {
            return MergePosesLocal.Contains(mpLocal);
        }
		// Getters (Private)
		private List<Vector2Int> LastInnerCluster { get { return InnerClusters[InnerClusters.Count-1]; } }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        protected void InitializeAsBoardOccupant (Board _boardRef, BoardOccupantData _data) {
			FootprintLocal = new List<Vector2Int>(_data.footprintLocal);
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
        
        
        public void RemakeInnerClusters() {
            InnerClusters = new List<List<Vector2Int>>();
            // for now, convert our footprints into TempTileFPs.
            Dictionary<Vector2Int, OccFPClusterData> fPs = new Dictionary<Vector2Int, OccFPClusterData>();
            for (int i=0; i<FootprintLocal.Count; i++) {
                Vector2Int fpLocal = FootprintLocal[i];
                fPs[fpLocal] = new OccFPClusterData(fpLocal);
            }
            // Do some recursive searchin'!
            foreach (OccFPClusterData fP in fPs.Values) {
                if (fP.clusterIndex == -1) {
                    InnerClusters.Add(new List<Vector2Int>());
                    RecursivelyFindClusters(fPs, fP.Pos, Vector2Int.zero);
                }
            }
        }
        private void RecursivelyFindClusters(Dictionary<Vector2Int, OccFPClusterData> fPs, Vector2Int originPos, Vector2Int dir) {
            Vector2Int pos = originPos + dir;
            OccFPClusterData fp = fPs.ContainsKey(pos) ? fPs[pos] : null;
            if (fp==null || fp.clusterIndex!=-1) { return; } // no footprint here? Stop.
            if (dir!=Vector2Int.zero && !MergePosesLocal.Contains(originPos.ToVector2()+dir.ToVector2()*0.5f)) { return; } // No connecting mergePos? Stop.
            
            fp.clusterIndex = InnerClusters.Count;
            LastInnerCluster.Add(pos);
            // Recurse!
            RecursivelyFindClusters(fPs, pos, Vector2Int.L);
            RecursivelyFindClusters(fPs, pos, Vector2Int.R);
            RecursivelyFindClusters(fPs, pos, Vector2Int.B);
            RecursivelyFindClusters(fPs, pos, Vector2Int.T);
        }


	}
}