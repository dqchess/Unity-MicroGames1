using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class Player : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
        [SerializeField] private ParticleSystem ps_dieBurst=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isDead;
        private float stretch=0;
        private float stretchVel=0;
        private float fallHeightNeutral; // the peak y distance between me and the space I'm headed to! I bounce UP to this height and fall from there.
        private Vector2 gravity;
        private Vector2 vel;
        // References
        [SerializeField] private GameController gameController=null;
        [SerializeField] private Sprite s_bodyNormal=null;
        [SerializeField] private Sprite s_bodyDashedOutline=null;
        private PaintSpace spaceHeadingTo;

        // Getters (Public)
        public static Color GetRandomHappyColor() { return new ColorHSB(Random.Range(0f,1f), 0.9f, 1f).ToColor(); }
        // Getters/Setters (Private)
        private Color bodyColor {
            get { return i_body.color; }
            set { i_body.color = value; }
        }
        private Vector2 pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        private bool IsLevelComplete { get { return gameController.IsLevelComplete; } }
        private PaintSpace[] paintSpaces { get { return gameController.PaintSpaces; } }
        private PaintSpace GetSpaceTouching() {
            foreach (PaintSpace space in paintSpaces) {
                if (space.HitRect.Contains(pos)) { return space; }
            }
            return null;
        }
        private PaintSpace GetUnpaintedSpaceTouching() {
            foreach (PaintSpace space in paintSpaces) {
                if (space.IsPainted) { continue; }
                if (space.HitRect.Contains(pos)) { return space; }
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
            fallHeightNeutral = GetFallHeightNeutral(levelIndex);
            spaceHeadingTo = GetRandomUnpaintedSpace();
            gravity = new Vector2(0, GetGravityY(levelIndex));
            pos = new Vector2(spaceHeadingTo.HitRect.center.x, spaceHeadingTo.HitRect.center.y + fallHeightNeutral);
            vel = Vector2.zero;

            bodyColor = GetRandomHappyColor();
            i_body.sprite = s_bodyNormal;
            ps_dieBurst.Clear();
        }
        private float GetGravityY(int levelIndex) {
            return -0.30f - levelIndex*0.01f;
        }
        private float GetFallHeightNeutral(int levelIndex) {
            // return Mathf.Min(360, 200+levelIndex*10f); // TESTing out these values
            return 260f;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        public void OnPressJumpButton() {
            PaintSpace spaceTouching = GetUnpaintedSpaceTouching();
            if (spaceTouching != null) {
                BounceOnSpace(spaceTouching);
            }
            else {
                Explode();
            }
        }

        private void BounceOnSpace(PaintSpace space) {
            stretch = -0.2f;//-0.45f;

            // Randomize my color!
            if (!IsLevelComplete) {
                bodyColor = GetRandomHappyColor();
            }
            // Paint the space!
            if (space != null) {
                space.OnPlayerBounceOnMe(bodyColor);
                gameController.OnPlayerPaintSpace();
            }

            // Choose the next space to go to!!
            PaintSpace nextSpace = GetRandomUnpaintedSpace();
            if (nextSpace != null) { // Is there a space to go to? Let's go to it!
                spaceHeadingTo = nextSpace;
            }
            
            // Find how fast we have to move upward to reach this y pos, and set our vel to that!
            Vector2 spaceToPos = spaceHeadingTo.HitRect.center;
            float peakPosY = spaceToPos.y+fallHeightNeutral + Random.Range(-50,50);
            float displacementY = spaceToPos.y - pos.y;
            float yDist = Mathf.Max (0, peakPosY-pos.y);
            float yVel = Mathf.Sqrt(2*-gravity.y*yDist); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)

            // float timeOfFlight = 2f*yVel / gravity.y;
            float g = gravity.y;
            float timeOfFlight = (-yVel - Mathf.Sqrt(yVel*yVel + 2*g*displacementY)) / g; // note that this is in seconds DIVIDED by FixedUpdate FPS.

            float xDist = spaceToPos.x - pos.x;
            float xVel = xDist / timeOfFlight;

            vel = new Vector2(xVel, yVel);
        }

        private void Explode() {
            isDead = true;
            i_body.sprite = s_bodyDashedOutline;
            bodyColor = new Color(1f, 0.1f, 0f);
            stretch = stretchVel = 0;
            ApplyStretch();
            //ps_dieBurst.Emit(50);
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

            UpdateScaleRotation();
        }
        private void ApplyGravity() {
            vel += gravity;
        }
        private void ApplyVel() {
            pos += vel;
        }
        private void ApplyBounds() {
            // If I've passed too far into the space I'm heading towards, explode me!
            float boundsY = spaceHeadingTo.HitRect.yMin;//IsLevelComplete ? 270f : 255f; // HACK HARDCODED
            if (vel.y<0 && pos.y<boundsY) {
                pos = new Vector2(pos.x, boundsY);
                if (IsLevelComplete) {
                    PaintSpace paintSpaceTouching = GetSpaceTouching();
                    BounceOnSpace(paintSpaceTouching);
                }
                else {
                    Explode();
                }
            }
            //// Safety checks (because why not?)
            //if (pos.x<0
        }
        private void UpdateScaleRotation() {
            // Stretch me!
            stretch += stretchVel;
            float stretchTarget = 0;// Mathf.Max(0, -vel.y)*0.01f;
            stretchVel += (stretchTarget-stretch) / 4f;
            stretchVel *= 0.8f;
            ApplyStretch();

            float rotation = Mathf.Atan2(-vel.x, vel.y) * Mathf.Rad2Deg;
            this.transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        private void ApplyStretch() {
            //this.transform.localScale = new Vector3(
            //1-vel.magnitude*0.4f,
            //1+vel.magnitude*0.4f,
            //1);
            this.transform.localScale = new Vector3(1-stretch, 1+stretch, 1);
        }

    }
}



/*
Trajectory math! #quadraticformula
dispY = yVel*t + (g*t^2)*0.5
0 = g*0.5*t^2 + yVel*t + -dispY
t = (-B +/- sqrt(B^2-4AC)) / 2A
t = (-yVel - sqrt(yVel*yVel-4*(g*0.5f)*(-dispY))) / (2*g*0.5f)
t = (-yVel - sqrt(yVel*yVel + 2*g*dispY)) / g
*/

