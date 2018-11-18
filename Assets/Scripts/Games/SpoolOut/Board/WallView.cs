using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    public class WallView : BoardObjectView {
    	// Components
    
    
    	// ----------------------------------------------------------------
    	//  Initialize
    	// ----------------------------------------------------------------
    	public void Initialize (BoardView _myBoardView, Transform tf_parent, Wall _wall) {
    		base.InitializeAsBoardObjectView (_myBoardView, tf_parent, _wall);
            
            //float us = MyBoardView.UnitSize;
            //float w = us*0.08f;
            //myRectTransform.sizeDelta = new Vector2(w, us);
    	}
    	
    }
}