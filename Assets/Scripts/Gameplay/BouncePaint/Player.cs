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
        [SerializeField] private ParticleSystem ps_dieBurst=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isDead;
        private float stretch=0;
        private float stretchVel=0;
        private Vector2 gravity;
        private Vector2 vel;
        // References
        [SerializeField] private GameController gameController=null;
        [SerializeField] private Sprite s_bodyNormal=null;
        [SerializeField] private Sprite s_bodyDashedOutline=null;

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
            PaintSpace startingSpace = GetRandomUnpaintedSpace();
            pos = new Vector2(startingSpace.HitRect.center.x, peakPosYNeutral);
            vel = Vector2.zero;
            gravity = new Vector2(0, GetGravityY(levelIndex));
            bodyColor = GetRandomHappyColor();
            i_body.sprite = s_bodyNormal;
            ps_dieBurst.Clear();
        }
        private float GetGravityY(int levelIndex) {
            return -0.35f - levelIndex*0.05f;
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
            }

            // Choose the next space to go to!!
            PaintSpace nextSpace = GetRandomUnpaintedSpace();
            if (nextSpace == null) { // No next space? No worries! Just flip our vel.
                vel = new Vector2(0, -vel.y);
            }
            else {
                // Find how fast we have to move upward to reach this y pos, and set our vel to that!
                float peakPosY = peakPosYNeutral + Random.Range(-50,50);
                float yDist = Mathf.Max (0, peakPosY-pos.y);
                float yVel = Mathf.Sqrt(2*-gravity.y*yDist); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)

                float timeOfFlight = 2f*yVel / gravity.y;
                float xDist = pos.x - nextSpace.HitRect.center.x;
                float xVel = xDist / timeOfFlight;

                vel = new Vector2(xVel, yVel);
            }

            // No next space?? Tell GameController we're so good at this level!
            if (nextSpace == null) {
                gameController.OnPlayerPaintLastSpace();
            }
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
            // If I've dropped off the face of the earth, explode me!
            float boundsY = IsLevelComplete ? 270f : 255f; // HACK HARDCODED
            if (pos.y < boundsY) {
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