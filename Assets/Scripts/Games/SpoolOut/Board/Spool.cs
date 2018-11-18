using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
	public class Spool : BoardObject {
        // Properties
        public List<Vector2Int> PathSpaces { get; private set; }
        public int ColorID { get; private set; }
        public int numSpacesToFill { get; private set; }
        public int numSpacesLeft { get; private set; } // updated every time we change pathSpaces.
        
        
        // Getters (Public)
        public bool IsSatisfied { get { return PathSpaces.Count >= numSpacesToFill; } }
        public Vector2Int LastSpacePos { get { return PathSpaces[PathSpaces.Count-1]; } }
        public bool IsSecondLastSpacePos(Vector2Int pos) {
            Vector2Int secondLastPos = secondLastSpacePos();
            if (secondLastPos == Vector2Int.none) { return false; } // Don't even have second-to-last space? False.
            return secondLastPos.x==pos.x && secondLastPos.y==pos.y;
        }
        // Getters (Private)
        private Vector2Int secondLastSpacePos() {
            if (PathSpaces.Count < 2) { return Vector2Int.none; } // No second-last space? Return null.
            return PathSpaces[PathSpaces.Count-2];
        }
        private bool IsLastSpace(int col,int row) {
            Vector2Int lps = LastSpacePos;
            return lps.x==col && lps.y==row;
        }
        private bool PathContains(Vector2Int pos) {
            for (int i=0; i<PathSpaces.Count; i++) {
                if (PathSpaces[i] == pos) { return true; }
            }
            return false;
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public Spool (Board _boardRef, SpoolData _data) {
			base.InitializeAsBoardObject(_boardRef, _data);
			ColorID = _data.colorID;
            PathSpaces = new List<Vector2Int>(_data.pathSpaces);
            numSpacesToFill = _data.numSpacesToFill;
            //if (pathSpaces == null) { // Convenience: Pass in null to default to just empty path.
            //  pathSpaces = new Vector2Int[0];
            //  AddPathSpace(new Vector2Int(col,row));
            //}
            UpdateNumSpacesLeft();
		}
		public SpoolData SerializeAsData() {
			SpoolData data = new SpoolData(BoardPos, ColorID, new List<Vector2Int>(PathSpaces)); // note: COPY the Vector2Int list. DEF don't want a reference.
			return data;
		}
        
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void AddPathSpace(Vector2Int spacePos) {
            PathSpaces.Add(spacePos);
            GetSpace(spacePos.x,spacePos.y).SetMySpool(this);
            UpdateNumSpacesLeft();
        }
        public void RemovePathSpace() {
            if (PathSpaces.Count < 2) { return; } // Safety check!
            // Remove from that space.
            Vector2Int lps = LastSpacePos;
            GetSpace(lps.x,lps.y).RemoveMySpool(this);
            PathSpaces.RemoveAt(PathSpaces.Count-1);
            UpdateNumSpacesLeft();
        }
        public void RemoveAllPathSpaces() {
            foreach (Vector2Int spacePos in PathSpaces) {
                GetSpace(spacePos).RemoveMySpool(this);
            }
            PathSpaces = new List<Vector2Int>();
            AddPathSpace(new Vector2Int(BoardPos.col,BoardPos.row));
        }
        public void Truncate(Vector2Int truncPos) {
            if (!PathContains(truncPos)) { // Safety check.
                Debug.LogError("Whoa!! Trying to truncate a Spool, but it doesn't have the space we wanna truncate to!");
                return;
            }
            while (LastSpacePos != truncPos) {
                RemovePathSpace();
            }
        }
        private void UpdateNumSpacesLeft() {
            numSpacesLeft = numSpacesToFill - PathSpaces.Count;
        }
        
        
        
  
  
  
  

	}
}