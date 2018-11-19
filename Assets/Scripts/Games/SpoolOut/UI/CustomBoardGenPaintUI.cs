using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpoolOut {
    public class CustomBoardGenPaintUI : MonoBehaviour {
        // Components
        [SerializeField] private Button b_eraser;
        [SerializeField] private Button[] b_colorIDs;
        // Properties
        private int paintColorID=-1; // -1 is eraser.
        // References
        [SerializeField] private Level level;
        private Button prevSelectedButton; // for applying highlight.
        
        // Getters (Private)
        private Board board { get { return level.Board; } }
        private Vector2Int mousePosBoard { get { return level.MousePosBoard; } }
        private InputController inputController { get { return InputController.Instance; } }
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            // Color buttons to match Spool colors!
            for (int i=0; i<b_colorIDs.Length; i++) {
                ColorBlock cb = b_colorIDs[i].colors;
                Color bodyColor = SpoolView.GetPathColor(i);
                cb.normalColor = bodyColor;
                cb.highlightedColor = Color.Lerp(bodyColor, Color.white, 0.2f);
                cb.pressedColor = Color.Lerp(bodyColor, Color.black, 0.3f);
                b_colorIDs[i].colors = cb;
            }
        }
        
        // ----------------------------------------------------------------
        //  Button Events
        // ----------------------------------------------------------------
        public void OnClick_ColorID(int _colorID) {
            paintColorID = _colorID;
        }
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetButtonScale(Button button, float scale) {
            button.transform.localScale = Vector3.one * scale;
        }
        private void PaintSpace(BoardSpace space, int colorID) {
            BoardData boardData = board.SerializeAsData();
            
            // First, delete any footprint here. NOTE: Every spool is a 1x1, so this loop can be very simple.
            for (int i=0; i<boardData.spoolDatas.Count; i++) {
                if (boardData.spoolDatas[i].boardPos == space.BoardPos) {
                    // We're painting with the same color? Just erase, then! For convenience.
                    if (boardData.spoolDatas[i].colorID == colorID) {
                        colorID = -1; // say we wanna use the eraser instead now. (No point in painting over a color with itself anyways.)
                    }
                    boardData.spoolDatas.RemoveAt(i);
                    break;
                }
            }
            //// NOT the eraser?? Add a Spool here!
            //if (colorID >= 0) {TODO: This, I suppose.
            //    boardData.spoolDatas.Add(new SpoolData(space.BoardPos, colorID));
            //}
            
            // Remake the Board, bro!
            level.Debug_RemakeBoardAndViewFromArbitrarySnapshot(boardData);
        }
        
        
        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            UpdateButtonHighlights();
            RegisterTouchInput();
        }
        
        private void UpdateButtonHighlights() {
            // Determine which button is highlighted.
            Button highlightedButton = null;
            if (paintColorID >= -1) {
                if (paintColorID==-1) { highlightedButton = b_eraser; }
                else if (paintColorID < b_colorIDs.Length) { highlightedButton = b_colorIDs[paintColorID]; }
            }
            
            // Did the highlighted button change??
            if (prevSelectedButton != highlightedButton) {
                if (highlightedButton != null) {
                    highlightedButton.transform.SetAsLastSibling(); // put in front of the others.
                }
                if (prevSelectedButton != null) {
                    SetButtonScale(prevSelectedButton, 1);
                }
                prevSelectedButton = highlightedButton;
            }
            
            // There IS a button highlighted?? Bounce it!
            if (highlightedButton != null) {
                float bounceScale = 1 + Mathf.Abs(Mathf.Sin(Time.unscaledTime*5f))*0.2f;
                SetButtonScale(highlightedButton, bounceScale);
            }
        }
        
        private void RegisterTouchInput() {
            if (inputController == null) { return; } // For compiling during runtime.

            if (inputController.IsTouchDown()) { OnTouchDown(); }
        }

        private void OnTouchDown() {
            BoardSpace spaceOver = board.GetSpace(mousePosBoard.x,mousePosBoard.y);
            // Over a space? Paint it!!
            if (spaceOver != null) {
                PaintSpace(spaceOver, paintColorID);
            }
        }
    
    }
}