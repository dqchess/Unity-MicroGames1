using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
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
            base.InitializeAsBoardObject (_boardRef, _data.boardPos);
			IsVertical = SideFacing==1 || SideFacing==3; // note that 3 should never be passed in. 0 and 1 only (so we can never have two walls in the same space accidentally).
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
        
        
        public Tile GetTileStraddlingMe() {
            Tile tileA = boardRef.GetTile(spaceA.BoardPos);
            Tile tileB = boardRef.GetTile(spaceB.BoardPos);
            if (tileA == tileB) {
                return tileA;
            }
            return null;
        }
    }
}