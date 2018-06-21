using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class Player : MonoBehaviour {
        // Constants
        private const float peakPosYNeutral = 500f; // HARDCODED for now
        // Components
        [SerializeField] private Image i_body=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isDead;
        private Vector2 gravity;
        private Vector2 vel;
        // References
        [SerializeField] private GameController gameController=null;

        // Getters/Setters
        private Vector2 pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        private PaintSpace[] paintSpaces { get { return gameController.PaintSpaces; } }
        //private Rect spacesRect { get { return gameController.SpacesRect; } }
        private PaintSpace GetSpaceTouching() {
            foreach (PaintSpace space in paintSpaces) {
                if (space.IsPainted) { continue; }
                if (space.HitRect.Contains(pos)) {
                    return space;
                }
            }
            return null;
        }
        private PaintSpace GetRandomUnpaintedSpace() {
            int[] randOrder = MathUtils.GetShuffledIntArray(paintSpaces.Length);
            for (int i=0; i<paintSpaces.Length; i++) {
                int index = randOrder[i];
                if (!paintSpaces[index].IsPainted) {
                    return paintSpaces[index];
                }
            }
            return null;
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        public void Reset(int levelIndex) {
            isDead = false;
            PaintSpace startingSpace = GetRandomUnpaintedSpace();
            pos = new Vector2(startingSpace.HitRect.center.x, peakPosYNeutral);
            vel = Vector2.zero;
            gravity = new Vector2(0, GetGravityY(levelIndex));
            i_body.color = new Color(1f, 0.7f, 0f);
        }
        private float GetGravityY(int levelIndex) {
            return -0.6f - levelIndex*0.05f;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        public void OnPressJumpButton() {
            PaintSpace paintSpaceTouching = GetSpaceTouching();
            if (paintSpaceTouching != null) {
                BounceInPaint(paintSpaceTouching);
            }
            else {
                Explode();
            }
        }

        private void BounceInPaint(PaintSpace space) {
            // Paint the space!
            space.PaintMe();
            // Choose the next space to go to!!
            PaintSpace nextSpace = GetRandomUnpaintedSpace();
            if (nextSpace == null) {
                vel = new Vector2(0, -vel.y);
                gameController.OnPlayerPaintLastSpace();
            }
            else {
				// Find how fast we have to move upward to reach this y pos, and set our vel to that!
                float peakPosY = peakPosYNeutral + Random.Range(-100,100);
                float yDist = Mathf.Max (0, peakPosYNeutral-pos.y);
                float yVel = Mathf.Sqrt(2*-gravity.y*yDist); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)

                float timeOfFlight = 2f*yVel / gravity.y;
                float xDist = pos.x - nextSpace.HitRect.center.x;
                float xVel = xDist / timeOfFlight;

                vel = new Vector2(xVel, yVel);
            }
        }

        private void Explode() {
            isDead = true;
            i_body.color = Color.red; // TEMP
            gameController.OnPlayerDie();
        }


        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate () {
            if (isDead) { return; }

            // test
            //pos = Input.mousePosition;
            //PaintSpace spaceOver = GetSpaceTouching();
            //if (spaceOver != null) {
            //    spaceOver.PaintMe();
            //}
            ApplyGravity();
            ApplyVel();
            ApplyBounds();
        }
        private void ApplyGravity() {
            vel += gravity;
        }
        private void ApplyVel() {
            pos += vel;
        }
        private void ApplyBounds() {
            // If I've dropped off the face of the earth, explode me!
            if (pos.y < 210) { // HACK HARDCODED
                Explode();
            }
            //// Safety checks (because why not?)
            //if (pos.x<0
        }

    }
}