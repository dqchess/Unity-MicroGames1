﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    /** Walls are between spaces, at the top or left of their col/row (so a vert. wall at 0,0 would be against the board edge and serve no purpose). */
    public class Wall : BoardObject {
    
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public Wall (Board _boardRef, WallData _data) {
            base.InitializeAsBoardObject (_boardRef, _data.boardPos);
    //      isVertical = SideFacing==1 || SideFacing==3; // note that 3 should never be passed in. 0 and 1 only (so we can never have two walls in the same space accidentally).
            // Add me to my (two) BoardSpaces!
            Vector2Int adjacentSpaceDir = MathUtils.GetDir(SideFacing);
            GetSpace (Col,Row).SetWallOnMe (this, SideFacing);
            GetSpace (Col+adjacentSpaceDir.x,Row+adjacentSpaceDir.y).SetWallOnMe(this, MathUtils.GetOppositeSide(SideFacing));
    //      // Initialize body!
    //      (body as WallBody).Initialize (this);
        }
        public WallData SerializeAsData() {
            WallData data = new WallData (BoardPos);
            return data;
        }
    }
}