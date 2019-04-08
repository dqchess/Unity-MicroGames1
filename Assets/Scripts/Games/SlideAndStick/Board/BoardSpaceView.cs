using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class BoardSpaceView : MonoBehaviour {
        public enum PipAnimTypes { Undefined, None, ToUp, ToDown }
		// Components
        [SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Image i_pipTop=null;
		[SerializeField] private Image i_pipShadow=null;
        // Properties
        private float pipTopYUp; // set in Initialize
        private float pipTopYDown; // set in Initialize
        //private float pipTopY_from, pipTopY_to; // for animating between board poses TODO: Remove these values.
        private PipAnimTypes pipAnimType;
		// References
		private BoardSpace mySpace;
        
        // Setters
        private float pipTopY {
            get { return i_pipTop.rectTransform.anchoredPosition.y; }
            set { i_pipTop.rectTransform.anchoredPosition = new Vector2(i_pipTop.rectTransform.anchoredPosition.x, value); }
        }
        

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (BoardView _boardView, BoardSpace mySpace) {
			this.mySpace = mySpace;
			int col = mySpace.Col;
			int row = mySpace.Row;

			// Parent me to my boooard!
			this.transform.SetParent (_boardView.tf_BoardSpaces);
			this.gameObject.name = "BoardSpace_" + col + "," + row;

			// Size/position me right!
            float us = _boardView.UnitSize;
            Vector2 us2 = new Vector2(us,us);
			this.transform.localPosition = new Vector3 (_boardView.BoardToX(col), _boardView.BoardToY(row), 0);
			this.transform.localScale = Vector3.one;
			this.transform.localEulerAngles = Vector3.zero;
            myRectTransform.sizeDelta = us2;
            
            // Size/pos pip images!
            i_pipTop.rectTransform.sizeDelta = i_pipShadow.rectTransform.sizeDelta = us2 * 0.4f;
            pipTopYUp = us * 0.1f;
            pipTopYDown = 0;
            
            // Start me in the right spot!
            pipAnimType = PipAnimTypes.None;
            SetValues_To(mySpace); // set "to" values to where I already am.
            UpdateVisualsPostMove();

			//float diameter = _boardView.UnitSize*0.25f;
            //GameUtils.SizeUIGraphic (i_fill, diameter,diameter);
            //GameUtils.SizeUIGraphic (i_shadow, diameter,diameter);
            
            //float b = BoardUtils.IsSpaceEven(col,row) ? 0.92f : 0.98f;
            //fillColor = new Color(b,b,b);
            //i_fill.color = fillColor;
            ////i_border.color = new Color(0.95f,0.95f,0.95f);
            //i_border.enabled = false;
            
            //if (!mySpace.IsPlayable) {
            //    i_fill.color = new Color(0.2f,0.2f,0.2f);
            //}
		}
        
        
        // ----------------------------------------------------------------
        //  Animations
        // ----------------------------------------------------------------
        public void PreAnimateInFreshBoard() {
            //float offset = (mySpace.Col + mySpace.Row*1.1f) * 0.1f;
            this.gameObject.transform.localScale = Vector3.zero;//Vector3.one * (0.4f-offset);
        }
        public void AnimateInFreshBoard() {
            //float offset = (mySpace.Col + mySpace.Row) * 0.05f;
            float delay = (mySpace.Col + mySpace.Row) * 0.042f;
            LeanTween.scale(this.gameObject, Vector3.one, 0.9f).setEaseOutBack().setDelay(delay);// + offset
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetValues_To(BoardSpace simSpace) {
            // Set pipAnimType!
            bool isOccTo = simSpace.HasOccupant;
            bool isOccFrom = mySpace.HasOccupant;
            if (isOccFrom == isOccTo) { pipAnimType = PipAnimTypes.None; } // Same status? Don't animate.
            else if (isOccFrom) { pipAnimType = PipAnimTypes.ToUp; } // YES Occ to NO Occ? Go up!
            else { pipAnimType = PipAnimTypes.ToDown; } // NO Occ to YES Occ? Go down!
        }
        
        //public void UpdateVisualsPreMove() {
        //    pipTopY_to = mySpace.MyOccupant==null ? pipTopYUp : pipTopYDown;
        //}
        public void UpdateVisualsPostMove() {
            pipTopY = mySpace.HasOccupant ? pipTopYDown : pipTopYUp;
            //pipTopY_from = mySpace.MyOccupant==null ? pipTopYUp : pipTopYDown;
        }
        public void GoToValues(float animLoc) {
            float loc;
            switch (pipAnimType) {
                case PipAnimTypes.None: return;
                case PipAnimTypes.ToUp:
                    //pipTopY = animLoc > 0.75f ? pipTopYUp : pipTopYDown;
                    //loc = Mathf.InverseLerp(0.6f,0.9f, animLoc);
                    loc = Mathf.InverseLerp(0.7f,0.75f, animLoc);
                    pipTopY = Mathf.Lerp(pipTopYDown,pipTopYUp, loc);
                    break;
                case PipAnimTypes.ToDown:
                    //pipTopY = animLoc > 0.25f ? pipTopYDown : pipTopYUp;
                    //loc = Mathf.InverseLerp(0.1f,0.4f, animLoc);
                    loc = Mathf.InverseLerp(0.25f,0.3f, animLoc);
                    pipTopY = Mathf.Lerp(pipTopYUp,pipTopYDown, loc);
                    break;
            }
        }
        
        
        
	}
    /*
    public class BoardSpaceView : MonoBehaviour {
        // Components
        [SerializeField] private Image i_backing=null;
        [SerializeField] private Image i_border=null;
        // Properties
        private Color fillColor;
        // References
        private BoardSpace mySpace;

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (BoardView _boardView, BoardSpace mySpace) {
            this.mySpace = mySpace;
            int col = mySpace.Col;
            int row = mySpace.Row;

            // Parent me to my boooard!
            this.transform.SetParent (_boardView.tf_BoardSpaces);
            this.gameObject.name = "BoardSpace_" + col + "," + row;

            // Size/position me right!
            this.transform.localPosition = new Vector3 (_boardView.BoardToX(col), _boardView.BoardToY(row), 0);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;

            // YES playable? Make visuals nice!
            const float spaceGap = 0f; // make this something between 0 and 5, to taste.
            float diameter = _boardView.UnitSize-spaceGap;
            GameUtils.SizeUIGraphic (i_border, _boardView.UnitSize+26,_boardView.UnitSize+26);
            GameUtils.SizeUIGraphic (i_backing, diameter,diameter);
            
            float b = BoardUtils.IsSpaceEven(col,row) ? 0.92f : 0.98f;
            fillColor = new Color(b,b,b);
            i_backing.color = fillColor;
            //i_border.color = new Color(0.95f,0.95f,0.95f);
            i_border.enabled = false;
            
            if (!mySpace.IsPlayable) {
                i_backing.color = new Color(0.2f,0.2f,0.2f);
            }
        }
        
        
        // ----------------------------------------------------------------
        //  Animations
        // ----------------------------------------------------------------
        public void PreAnimateInFreshBoard() {
            //float offset = (mySpace.Col + mySpace.Row*1.1f) * 0.1f;
            this.gameObject.transform.localScale = Vector3.zero;//Vector3.one * (0.4f-offset);
        }
        public void AnimateInFreshBoard() {
            //float offset = (mySpace.Col + mySpace.Row) * 0.05f;
            float delay = (mySpace.Col + mySpace.Row) * 0.042f;
            LeanTween.scale(this.gameObject, Vector3.one, 0.52f).setEaseOutBack().setDelay(delay);// + offset
        }
    }
    */
}