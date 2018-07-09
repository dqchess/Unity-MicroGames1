using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallPaint {
    public class Player : MonoBehaviour {
        // Components
        [SerializeField] private SpriteRenderer bodySprite;
        // Properties
        private bool canSteer=true;
        private float speed = 0.2f; // in Unity units.
		private int sideFacing;
		private Rect boundsRect;
        private Vector2 vel;

        // Getters/Setters
        private Vector2 pos {
            get { return this.transform.localPosition; }
            set { this.transform.localPosition = value; }
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start () {
            // TEMP
            SetDirFacing(Sides.R);
            // TEMP
            boundsRect = new Rect(-10,-10, 20,20);
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetDirFacing(int _side) {
            if (!canSteer) { return; } // Safety check.
            if (sideFacing == _side) { return; } // Not changing my side? Do nothin'.
            // Update values!
            sideFacing = _side;
            vel = MathUtils.GetDir(_side).ToVector2() * speed;
            canSteer = false; // I can no longer steer until I hit a wall! ;)
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update () {
            AcceptKeyInput();
        }
        private void AcceptKeyInput() {
            if (canSteer) {
                if (Input.GetKeyDown(KeyCode.LeftArrow)) { SetDirFacing(Sides.L); }
                else if (Input.GetKeyDown(KeyCode.RightArrow)) { SetDirFacing(Sides.R); }
                else if (Input.GetKeyDown(KeyCode.DownArrow)) { SetDirFacing(Sides.B); }
                else if (Input.GetKeyDown(KeyCode.UpArrow)) { SetDirFacing(Sides.T); }
            }
        }


        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate () {
            ApplyVel();
            ApplyBounds();
        }
        private void ApplyVel() {
            pos += vel;
        }

        private void ApplyBounds() {
            if (pos.x < boundsRect.xMin) {
                // TO DO: More of dis stuffs
            }
        }

    }
}