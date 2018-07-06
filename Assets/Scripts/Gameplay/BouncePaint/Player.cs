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
        private float fallHeightNeutral; // the peak y distance between me and the block I'm headed to! I bounce UP to this height and fall from there.
        private Vector2 gravity;
        private Vector2 vel;
        // References
        [SerializeField] private GameController gameController=null;
        [SerializeField] private Sprite s_bodyNormal=null;
        [SerializeField] private Sprite s_bodyDashedOutline=null;
        private Block blockHeadingTo;

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
        private List<Block> blocks { get { return gameController.Blocks; } }
        private Block GetBlockTouching() {
            foreach (Block obj in blocks) {
                if (obj.HitRect.Contains(pos)) { return obj; }
            }
            return null;
        }
        private Block GetUnpaintedBlockTouching() {
            foreach (Block obj in blocks) {
                if (obj.IsPainted) { continue; }
                if (obj.HitRect.Contains(pos)) { return obj; }
            }
            return null;
        }
        private Block GetRandomUnpaintedBlock() {
            int[] randOrder = MathUtils.GetShuffledIntArray(blocks.Count);
            for (int i=0; i<blocks.Count; i++) {
                int index = randOrder[i];
                if (!blocks[index].IsPainted) {
                    return blocks[index];
                }
            }
            return null;
		}
		/** ONLY works for HORIZONTALLY moving Blocks. Returns the predicted x position of the block by the time we reach its y pos. */
		private float GetBlockPosX(Block block, float startY, float yVel) {
			float blockY = block.HitRect.center.y;
			float displacementY = blockY - startY;
			float g = gravity.y;
			float timeOfFlight = (-yVel - Mathf.Sqrt(yVel*yVel + 2*g*displacementY)) / g; // note that this is in seconds DIVIDED by FixedUpdate FPS.
//			print("displacementY: " + displacementY + "  timeOfFlight: " + timeOfFlight);
			return block.GetPredictedPos(timeOfFlight).x;
		}


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        public void Reset(int levelIndex) {
            isDead = false;
            fallHeightNeutral = GetFallHeightNeutral(levelIndex);
            SetBlockHeadingTo(GetRandomUnpaintedBlock());
            gravity = new Vector2(0, GetGravityY(levelIndex));
			vel = new Vector2(0, 5); // Start with a little toss-up.
			float startY = blockHeadingTo.HitRect.center.y + fallHeightNeutral*0.85f; // 0.85f HARDCODED to taste, based on where we want to start the toss-up (note: we could totally calculate this to be perfect, but I don't want to right now)
			float startX = GetBlockPosX(blockHeadingTo, startY, vel.y); // calculate where the Block is gonna be when I reach its y pos.
            pos = new Vector2(startX, startY);

            bodyColor = GetRandomHappyColor();
            i_body.sprite = s_bodyNormal;
            ps_dieBurst.Clear();
        }
        private float GetGravityY(int levelIndex) {
            return -0.30f - levelIndex*0.008f;
        }
        private float GetFallHeightNeutral(int levelIndex) {
            // return Mathf.Min(360, 200+levelIndex*10f); // TESTing out these values
            return 250f;
        }



        // ----------------------------------------------------------------
        //  Events / Doers
        // ----------------------------------------------------------------
        private void SetBlockHeadingTo(Block _block) {
            blockHeadingTo = _block;
            gameController.OnPlayerSetBlockHeadingTo(blockHeadingTo);
        }

        public void OnPressJumpButton() {
            Block blockTouching = GetUnpaintedBlockTouching();
			// I AM touching a block!...
            if (blockTouching != null) {
				// Standard block? Bounce!
				if (blockTouching.DoTap) {
                	BounceOnBlock(blockTouching);
				}
				// Ooh, a DON'T-tap block. Explode!
				else {
                    blockTouching.OnPlayerPressJumpOnMeInappropriately();
					Explode();
				}
            }
			// I'm NOT touching any block...
            else {
                Explode();
            }
        }

        private void BounceOnBlock(Block block) {
            stretch = -0.25f;//-0.45f;

            // Randomize my color!
            if (!IsLevelComplete) {
                bodyColor = GetRandomHappyColor();
            }
            // Paint the block!
            if (block != null) {
                block.OnPlayerBounceOnMe(bodyColor);
                gameController.OnPlayerPaintBlock();
            }

            // Choose the next block to go to!!
            Block nextBlock = GetRandomUnpaintedBlock();
            if (nextBlock != null) { // Is there a block to go to? Let's go to it!
                SetBlockHeadingTo(nextBlock);
            }
            
            // Find how fast we have to move upward to reach this y pos, and set our vel to that!
            Vector2 blockToPos = blockHeadingTo.HitRect.center;
            float peakPosY = blockToPos.y+fallHeightNeutral + Random.Range(-30,30);
            float displacementY = blockToPos.y - pos.y;
            float yDist = Mathf.Max (0, peakPosY-pos.y);
            float yVel = Mathf.Sqrt(2*-gravity.y*yDist); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)
//			Debug.Log("displacementY: " + displacementY);

            // float timeOfFlight = 2f*yVel / gravity.y;
            float g = gravity.y;
            float timeOfFlight = (-yVel - Mathf.Sqrt(yVel*yVel + 2*g*displacementY)) / g; // note that this is in seconds DIVIDED by FixedUpdate FPS.

			float predBlockPosX = GetBlockPosX(blockHeadingTo, pos.y, yVel); // calculate where the Block is gonna be when I reach its y pos.
			float xDist = predBlockPosX - pos.x;
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
            // If I've passed too far into the block I'm heading towards, explode me!
			float boundsY = blockHeadingTo.HitRect.yMin;
            if (IsLevelComplete) { boundsY += 14f; } // When the level's over, make my bounds HIGHer, so I look more like I'm bouncing on the Blocks.
            if (vel.y<0 && pos.y<boundsY) {
                pos = new Vector2(pos.x, boundsY);
				// Level's complete? Just happily bounce on the block's head. ;)
                if (IsLevelComplete) {
					BounceOnBlock(blockHeadingTo);
                }
				// Level's NOT complete?
				else {
					// It's a don't-tap block?? Do that bounce!
					if (!blockHeadingTo.DoTap) {
						BounceOnBlock(blockHeadingTo);
					}
					// It's a NORMAL block. We've gone too far! Die.
					else {
                    	Explode();
					}
                }
            }
            //// Safety checks (because why not?)
            //if (pos.x<0
        }
        private void UpdateScaleRotation() {
            // Stretch me!
            stretch += stretchVel;
            float stretchTarget = 0;// Mathf.Max(0, -vel.y)*0.01f;
            stretchVel += (stretchTarget-stretch) / 5f;
            stretchVel *= 0.82f;
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


// test
//pos = Input.mousePosition;
//PaintSpace spaceOver = GetSpaceTouching();
//if (spaceOver != null) {
//    spaceOver.PaintMe();
//}

/*
Trajectory math! #quadraticformula
dispY = yVel*t + (g*t^2)*0.5
0 = g*0.5*t^2 + yVel*t + -dispY
t = (-B +/- sqrt(B^2-4AC)) / 2A
t = (-yVel - sqrt(yVel*yVel-4*(g*0.5f)*(-dispY))) / (2*g*0.5f)
t = (-yVel - sqrt(yVel*yVel + 2*g*dispY)) / g
*/

