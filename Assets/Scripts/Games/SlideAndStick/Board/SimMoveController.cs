using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    /** Owned and managed by Level.cs, this class translates mobile touch input into gameplay input (SIMULATING and MAKING moves!). */
    public class SimMoveController {
        // Constants
        private const float FlickVelThreshold = 4f; // if our touchVel is greater than this in OnTouchUp, we count it as a flick and execute the move!
        private float minDragOffset; // In screen space. Any drag less than this will be ignored.
        // Properties
        private readonly float unitSize;
        private int simMoveSide; // just matches simMoveDir. For optimization.
        private Vector2 dragAxes; // screen distance from dragAnchorPos to current touch pos.
        private Vector2 dragAnchorPos; // BASICALLY touchDownPos, EXCEPT set (to mouse pos) on touch down, AND whenever we execute a move.
        private Vector2 prevTouchPos; // for determining finger velocity, which allows flicking tiles.
		// References
		private Level level;

        // Getters (Public)
        public float SimMovePercent { get; private set; } // from 0 to 1. BoardView uses this value.
        public Vector2Int SimMoveDir { get; private set; }
        // Getters (Private)
        private bool CanExecuteSimMove { get { return level.BoardView.CanExecuteSimMove; } } // NOTE: This access is sloppy. *I* should be managing the whole sim board and move results!!

//        private bool IsTouch() { return (Input.touchSupported && Input.touchCount>0) || Input.GetMouseButton(0); }
        private Vector2 GetTouchPos() {
            if (Input.touchSupported && Input.touchCount>0) { return Input.touches[0].position; }
            return Input.mousePosition;
        }

        private Vector2Int GetSimMoveDir() {
            if (dragAxes.magnitude > minDragOffset) {
                if (Mathf.Abs(dragAxes.x) > Mathf.Abs(dragAxes.y)) {
                    return dragAxes.x<0 ? Vector2Int.L : Vector2Int.R;
                }
                else {
                    return dragAxes.y<0 ? Vector2Int.B : Vector2Int.T;
                }
            }
            else {
                return Vector2Int.zero;
            }
        }
        private float GetSimMovePercent() {
			if (!level.GameController.FUEController.CanTouchBoard) { return 0; } // Locked out by FUE? No sim move, then.
            switch (simMoveSide) {
                case Sides.L: return Mathf.Max(0, -dragAxes.x/unitSize*2f);
				case Sides.R: return Mathf.Max(0,  dragAxes.x/unitSize*2f);
				case Sides.B: return Mathf.Max(0, -dragAxes.y/unitSize*2f);
				case Sides.T: return Mathf.Max(0,  dragAxes.y/unitSize*2f);
                default: Debug.LogError("Whoa, side not recognized: " + simMoveSide); return 0;
            }
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
		public SimMoveController(Level _level) {
			this.level = _level;
			this.unitSize = level.BoardView.UnitSize;
			this.minDragOffset = 2;//_unitSize * 0.1f;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        public void Update() {
			if (InputController.Instance == null) { return; } // For runtime compile.

			if (InputController.Instance.IsTouchDown()) { OnTouchDown(); }
			if (InputController.Instance.IsTouchUp())   { OnTouchUp();   }
			if (InputController.Instance.IsTouchHold()) { OnTouchHeld(); }
            
            prevTouchPos = GetTouchPos();
        }

        private void OnTouchHeld() {
            UpdateDragAxes();
            UpdateSimMove();
        }
        private void OnTouchDown() {
            dragAnchorPos = GetTouchPos();
        }
        private void OnTouchUp() {
            UpdateDragAxes();

			if (level.CanMakeAnyMove()) {
	            // Far enough into LEGAL simulated move? Execute it!
                float touchVel = Vector2.Distance(GetTouchPos(), prevTouchPos);
                bool isFlick = touchVel > FlickVelThreshold;
				if ((SimMovePercent>0.5f || isFlick) && CanExecuteSimMove) {
	                ExecuteSimMove();
	            }
				// Otherwise, cancel it.
				else {
					CancelSimMove();
				}
                level.ReleaseTileGrabbing();
			}
        }

        private void UpdateDragAxes() {
            //// TEST
            //Tile tileGrabbing = level.Temp_TileGrabbing;
            //if (tileGrabbing != null) {
            //    Canvas canvas = Object.FindObjectOfType<Canvas>();
            //    Vector2 tilePos = level.BoardView.BoardToGlobal(tileGrabbing.BoardPos);
            //    tilePos = new Vector2(tilePos.x, canvas.GetComponent<RectTransform>().rect.height + tilePos.y); // meh. flip y.
            //    tilePos *= canvas.scaleFactor;
            //    dragAnchorPos = tilePos;
            //    Debug.Log(Time.frameCount + " tilePos: " + tilePos + "     touchPos: " + GetTouchPos());
            //}
            
            dragAxes = GetTouchPos() - dragAnchorPos;
        }

        private void UpdateSimMove() {
            Vector2Int thisSimMoveDir = GetSimMoveDir();
            // This simMoveDir is *different* from the current one, AND our simMovePercent is low (enough to switch sim-move directions)?...
            if (thisSimMoveDir != SimMoveDir && SimMovePercent<0.1f) {
                SetSimMoveDir(thisSimMoveDir);
            }
            // We have a simMoveDir?? Update simMovePercent!
            if (SimMoveDir != Vector2Int.zero) {
                SimMovePercent = GetSimMovePercent();
                // We've gone all the way with the simulated move??
                if (SimMovePercent >= 1) {
                    // We CAN move?! Do!!
                    if (CanExecuteSimMove) {
                        ExecuteSimMoveSansAnimation();
                    }
                    // We CAN'T move. Limit the percent a bunch.
                    else {
                        SimMovePercent = Mathf.Pow(SimMovePercent, 0.2f);
                    }
                }
            }
        }
        private void ExecuteSimMove() {
			// Say we're swipin'.
			if (SimMoveDir==Vector2Int.B) level.ExecuteMoveAttempt(Vector2Int.B);
			else if (SimMoveDir==Vector2Int.T) level.ExecuteMoveAttempt(Vector2Int.T);
			else if (SimMoveDir==Vector2Int.L) level.ExecuteMoveAttempt(Vector2Int.L);
			else if (SimMoveDir==Vector2Int.R) level.ExecuteMoveAttempt(Vector2Int.R);
            // Reset dragAnchorPos!
			dragAnchorPos = GetTouchPos();
			// Nix any sim move.
			SetSimMoveDir(Vector2Int.zero);
            //Debug.Log(Time.frameCount + " ExecuteSimMoveExecuteSimMoveExecuteSimMoveExecuteSimMove!");
        }
		private void CancelSimMove() {
			SetSimMoveDir(Vector2Int.zero);
			level.OnCancelSimMove();
            //Debug.Log(Time.frameCount + " CANCEL sim move!");
		}
        /** 1) Executes the move. 2) Skips animation and tells BoardView to just go to end result. */
        private void ExecuteSimMoveSansAnimation() {
            // 1) Execute the move.
            ExecuteSimMove();
            // 2) Skip animation to end result.
            level.BoardView.UpdateAllViewsMoveEnd();
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetSimMoveDir(Vector2Int _dir) {
            SimMoveDir = _dir;
            simMoveSide = MathUtils.GetSide(SimMoveDir);
            // Reset our dragAnchorPos right next to finger! As if we juust touched down and dragged just enough.
            dragAnchorPos = GetTouchPos();
            dragAnchorPos += new Vector2(-_dir.x, _dir.y) * (minDragOffset+1); // hacky flipping y?
            UpdateDragAxes(); // update this for safety.
            SimMovePercent = 0; // reset this for safety.
        }

    }
}


/*
            // Touch-down/-up/-hold!
isTouch = IsTouch();
isTouchUp = !isTouch && pisTouch;
isTouchDown = isTouch && !pisTouch;

if (isTouchDown) { OnTouchDown (); }
if (isTouchUp) { OnTouchUp (); }
if (isTouch) { OnTouchHeld (); }

pisTouch = isTouch;
*/

/*
    // Constants
private const float PUSH_REQUEST_INTERVAL = 0.18f; // in seconds.
private const float SWIPE_THRESHOLD = 15f; // how many pixels we gotta move a finger to count a swipe.
// Properties
private bool isTouch, isTouchDown, isTouchUp; // updated at the beginning of every frame.
private bool pisTouch; // isTouch for the previous frame. Used to detect touch-downs/ups.
private int pushSide; // a "push" is a request to move. If -1, no "push". If 0-3, we're requesting a "push" towards that side.
private int pushRequestSide; // POORLY WORDED. It's for registering the actual push.
private float timeUntilPushRequest; // so we can hold our finger down and keep making a move every 0.4 seconds.
//  private float simulatedMovePercent; // from 0 to 1. BoardView uses this value.
private Vector2 touchDownPos;
//  private Vector2Int simulatedMoveDir;

// Getters
//public int PushRequestSide { get { return pushRequestSide; } }
        public bool IsSwipe_L { get { return pushRequestSide==Sides.L; } }
        public bool IsSwipe_R { get { return pushRequestSide==Sides.R; } }
        public bool IsSwipe_D { get { return pushRequestSide==Sides.B; } }
        public bool IsSwipe_U { get { return pushRequestSide==Sides.T; } }
public float SimulatedMovePercent { get { return 0;}}//simulatedMovePercent; } }
public Vector2Int SimulatedMoveDir { get { return Vector2Int.zero;}}//simulatedMoveDir; } }

private bool IsTouch() { return (Input.touchSupported && Input.touchCount>0) || Input.GetMouseButton(0); }
private Vector2 GetTouchPos() {
    if (Input.touchSupported && Input.touchCount>0) { return Input.touches[0].position; }
    return Input.mousePosition;
}
public Vector2 Debug_TouchUpPos { get; set; }
public Vector2 Debug_TouchDownPos { get { return touchDownPos; } }




// ----------------------------------------------------------------
//  Update
// ----------------------------------------------------------------
public void Update () {
    // Reset pushRequestSide every frame.
    pushRequestSide = -1;

    // Touch-down/-up/-hold!
    isTouch = IsTouch();
    isTouchUp = !isTouch && pisTouch;
    isTouchDown = isTouch && !pisTouch;

    if (isTouchDown) { OnTouchDown (); }
    if (isTouchUp) { OnTouchUp (); }
    if (isTouch) { OnTouchHeld (); }

    CountdownToPushRequest ();

    pisTouch = isTouch;
}

private void OnTouchHeld () {
    Debug_TouchUpPos = GetTouchPos(); // For debugging!

    // Are we NOT already pushing?? Maybe register a push!!
    if (pushSide == -1) {
        Vector2 touchPos = GetTouchPos();
        if (touchPos.x-touchDownPos.x > SWIPE_THRESHOLD) {
            RegisterPush (1);
        }
        else if (touchDownPos.x-touchPos.x > SWIPE_THRESHOLD) {
            RegisterPush (3);
        }
        else if (touchPos.y-touchDownPos.y > SWIPE_THRESHOLD) {
            RegisterPush (0);
        }
        else if (touchDownPos.y-touchPos.y > SWIPE_THRESHOLD) {
            RegisterPush (2);
        }
    }
}
private void OnTouchDown () {
    touchDownPos = GetTouchPos();
}
private void OnTouchUp () {
    // NOW we're not pushing anymore. :)
    pushSide = -1;
    timeUntilPushRequest = -1;
}

private void RegisterPush (int _side) {
    pushSide = _side;
    pushRequestSide = _side;
    timeUntilPushRequest = PUSH_REQUEST_INTERVAL;
}

private void CountdownToPushRequest () {
    if (timeUntilPushRequest > 0) {
        timeUntilPushRequest -= Time.deltaTime;
        if (timeUntilPushRequest <= 0) {
            pushRequestSide = pushSide;
            timeUntilPushRequest = PUSH_REQUEST_INTERVAL;
        }
    }
}
*/