using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    /** Walls are between spaces, at the top or left of their col/row (so a vert. wall at 0,0 would be against the board edge and serve no purpose). */
    public class Wall : BoardObject {
        // Properties
		public bool IsVertical { get; private set; }
        public Vector2 BetweenPos { get; private set; }
        // References
        private BoardSpace spaceA,spaceB; // the space I'm "on", and the adjacent space I'm also spiritually on.
    
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public Wall (Board _boardRef, WallData _data) {
            base.InitializeAsBoardObject(_boardRef, _data);
			IsVertical = _data.IsVertical();
            // Add me to my (two) BoardSpaces!
            Vector2Int adjacentSpaceDir = MathUtils.GetDir(SideFacing);
            BetweenPos = new Vector2(Col+adjacentSpaceDir.x*0.5f, Row+adjacentSpaceDir.y*0.5f);
            spaceA = GetSpace(Col,Row);
            spaceB = GetSpace(Col+adjacentSpaceDir.x,Row+adjacentSpaceDir.y);
            spaceA.SetWallOnMe(this, SideFacing);
			if (spaceB != null) { // Safety check. ;)
            	spaceB.SetWallOnMe(this, MathUtils.GetOppositeSide(SideFacing));
			}
			else {
				Debug.LogError("Whoa! We put a Wall on the edge of the Board. Don't do that-- it's pointless and will confuse the player.");
			}
        }
        public WallData SerializeAsData() {
            WallData data = new WallData (BoardPos);
            return data;
        }
        
    }
}