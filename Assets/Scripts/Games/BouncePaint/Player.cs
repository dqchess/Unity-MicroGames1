using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class Player : MonoBehaviour {
        // Constants
        private const float minBounceHeight = 120f; // this prevents us rocketing down directly after a bounce (if we're going from a high to a very low block).
        // Components
        [SerializeField] private CircleCollider2D myCollider=null;
        [SerializeField] private Image i_body=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        static private Color startingPlayerColor=Color.clear; // used for keeping colors consistent between levels.
        private bool isDead;
        private float radius;
        private float fallHeightNeutral; // the peak y distance between me and the block I'm headed to! I bounce UP to this height and fall from there.
        private float stretch=0;
        private float stretchVel=0;
        private int playerIndex; // which ball I am (relevant for multi-ball lvls)
        private Vector2 gravity;
        private Vector2 vel;
        // References
        [SerializeField] private Sprite s_bodyNormal=null;
        [SerializeField] private Sprite s_bodyDashedOutline=null;
        private List<Block> blocksTouching; // added/removed from by OnCollisionEnter/Exit!
        private Block blockHeadingTo;
        private GameController gameController=null;
        private Level myLevel;

        // Getters (Public)
        public static Color GetRandomHappyColor() {
            //float h = Random.Range(0.25f, 1.1f) % 1; // this avoids yellow, which is hard to see against the white bg.
            float h = Random.Range(0f, 1f); // taste the whole rainbow!!
            return new ColorHSB(h, 0.9f, 1f).ToColor();
        }
        public float YVel { get { return vel.y; } }
        public float BottomY { get { return bottomY; } }
        // Getters/Setters (Private)
        //private float fts { get { return TimeController.FrameTimeScale; } }
        private float timeScale { get { return Time.timeScale; } }
        private Color bodyColor {
            get { return i_body.color; }
            set { i_body.color = value; }
        }
        private Vector2 pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        private float bottomY {
            get { return pos.y-radius; }
            set { pos = new Vector2(pos.x, value+radius); }
        }
        private List<Block> blocks { get { return gameController.Blocks; } }
        //private Block GetBlockTouching() {
        //    Vector2 bottomPos = new Vector2(pos.x, bottomY);
        //    foreach (Block obj in blocks) {
        //        if (obj.HitBox.Contains(bottomPos)) { return obj; }
        //    }
        //    return null;
        //}
        public Block GetUnpaintedBlockTouching() {
            Block block_numbersMethod = GetUnpaintedBlockTouching_NumbersMethod();
            Block block_colliderMethod = GetUnpaintedBlockTouching_ColliderMethod();
            if (block_numbersMethod != block_colliderMethod) {
                Debug.Log("YO! Block-touching NUMBERS and COLLIDER methods are returning TWO different blocks!!");
            }
            //return block_colliderMethod;
            return block_numbersMethod;
        }
        public Block GetUnpaintedBlockTouching_NumbersMethod() {
            // NUMBERS method: Check my bottom pos against all other blocks' hitBoxes.
            Vector2 bottomPos = new Vector2(pos.x, bottomY);
            foreach (Block obj in blocks) {
                if (obj.IsPainted) { continue; }
                if (obj.HitBox.Contains(bottomPos)) { return obj; }
            }
            return null;
        }
        public Block GetUnpaintedBlockTouching_ColliderMethod() {
            // COLLIDER method: Simply return the first unpainted one I'm touching.
            foreach (Block b in blocksTouching) {
                if (!b.IsPainted) { return b; }
            }
            return null;
        }
        private Block GetRandomAvailableBlock(Block blockFrom) {
            int[] randOrder = MathUtils.GetShuffledIntArray(blocks.Count);

            // If we're coming from a DON'T-TAP Block, force us to go to a DO-tap Block next (if possible)!
            bool preferDoTapBlock = blockFrom!=null && !blockFrom.DoTap;
            if (preferDoTapBlock) {
                for (int i=0; i<blocks.Count; i++) {
                    Block block = blocks[randOrder[i]];
                    if (block.IsAvailable && block.DoTap) {
                        return block;
                    }
                }
            }
            // Just return ANY available Block.
            for (int i=0; i<blocks.Count; i++) {
                Block block = blocks[randOrder[i]];
                if (block.IsAvailable) {
                    return block;
                }
            }
            // Everyone's painted/taken. Return null.
            return null;
		}
		/** ONLY works for HORIZONTALLY moving Blocks. Returns the predicted x position of the block by the time we reach its y pos. */
		private float GetBlockPosX(Block block, float startY, float yVel) {
			float blockY = block.HitBox.center.y;
			float displacementY = blockY - startY;
			float g = gravity.y;
			float timeOfFlight = (-yVel - Mathf.Sqrt(yVel*yVel + 2*g*displacementY)) / g; // note that this is in seconds DIVIDED by FixedUpdate FPS.
			return block.GetPredictedPos(timeOfFlight).x;
        }
        //private Block GetBlockFromCollision(Collision2D collision) {
        //    return collision.otherCollider.GetComponent<Block>();
        //}
        private Block GetBlockFromCollider(Collider2D col) {
            return col.GetComponent<Block>();
        }

        private float GetGravityY(int levelIndex) {
            float baseGravity = -0.19f - levelIndex*0.001f;
            //float baseGravity = (-0.35f - levelIndex*0.003f) * 0.8f; // TEMP TEST! We upped FixedUpdate iterations, so bringing down gravity to compensate.
            return baseGravity * gameController.PlayerGravityScale;
        }
        private float GetFallHeightNeutral(int levelIndex) {
            return 200f;//250f; //Mathf.Min(360, 200+levelIndex*10f);
        }



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        public void Initialize(GameController _gameController, Level _myLevel, int _playerIndex, float _radius) {
            gameController = _gameController;
            myLevel = _myLevel;
            playerIndex = _playerIndex;

            myRectTransform.SetParent(myLevel.transform);
            myRectTransform.localPosition = Vector2.zero;
            myRectTransform.localScale = Vector2.one;
            myRectTransform.localEulerAngles = Vector2.zero;
            
            radius = _radius;//gameController.PlayerDiameter*0.5f;
            myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);

            // Set myCollider offset/size!
            myCollider.radius = 2f; // teeny tiny little bead...
            myCollider.offset = new Vector2(0, radius-myCollider.radius); // ...at the bottom of my circle!
        }
        public void Reset(int levelIndex) {
            this.transform.SetAsLastSibling(); // Put me in front of all other props!

            // Reset basic values.
            isDead = false;
            blocksTouching = new List<Block>();
            i_body.sprite = s_bodyNormal;
            // If we have a color we know we wanna be, be that!
            if (startingPlayerColor != Color.clear) {
                bodyColor = startingPlayerColor;
                startingPlayerColor = Color.clear;
            }
            // Otherwise, just be a random happy color. :)
            else {
                bodyColor = GetRandomHappyColor();
            }

            fallHeightNeutral = GetFallHeightNeutral(levelIndex);
            SetBlockHeadingTo(GetRandomAvailableBlock(null));
            gravity = new Vector2(0, GetGravityY(levelIndex));
            // Start with a little toss-up, and an EXTRA toss-up for additional balls!
            float startFallHeight = fallHeightNeutral + (playerIndex*50);
            float startLoc = 0.6f - playerIndex*0.5f;
            float distToApex = startFallHeight*(1-startLoc); // how far we're gonna travel up until our yVel hits 0.
            float yVel = Mathf.Sqrt(2*-gravity.y*distToApex); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)
            vel = new Vector2(0, yVel);
            if (blockHeadingTo != null) {
                float startY = blockHeadingTo.HitBox.center.y + startFallHeight*startLoc;
    			float startX = GetBlockPosX(blockHeadingTo, startY, vel.y); // calculate where the Block is gonna be when I reach its y pos.
                pos = new Vector2(startX, startY);
            }
        }



        // ----------------------------------------------------------------
        //  Events / Doers
        // ----------------------------------------------------------------
        private void SetBlockHeadingTo(Block _block) {
            blockHeadingTo = _block;
            if (blockHeadingTo != null) {
                blockHeadingTo.BallTargetingMe = this;
            }
            gameController.OnPlayerSetBlockHeadingTo(blockHeadingTo);
        }

        public void BounceOnBlock(Block block) {
            // FIRST, tell the Block I've targeted that I'm not interested in anymore (it could be a different Block, for the record)!
            blockHeadingTo.BallTargetingMe = null;

            // Wait a frame or two, THEN squish me! (Squashing right away makes the collision look a little off.)
            Invoke("SquishFromBounce", 0.05f);

            // If the level's over, AND we haven't defined the startingPlayerColor (aka no one's bounced up yet)? Bounce all the way up off-screen instead!
            //bool doBounceUpOffscreen = gameController.IsLevelComplete && startingPlayerColor==Color.clear;
            // Decide NOW if we're gonna bounce up offscreen! Do it if we're the winning player.
            bool doBounceUpOffscreen = gameController.WinningPlayer == this;

            // Bounce and paint!
            bool wasBlockPainted = block.IsPainted;
            if (!wasBlockPainted) {
                bodyColor = GetRandomHappyColor(); // Rando my colo!
            }
            block.OnPlayerBounceOnMe(bodyColor, vel);
            bool didPaintBlock = !wasBlockPainted && block.IsPainted;
            gameController.OnPlayerBounceOnBlock(this, didPaintBlock);

            bool isWinningPaintHit = !wasBlockPainted && gameController.IsLevelComplete;
            // Winning hit??
            if (isWinningPaintHit) {
                block.DipExtraFromWinningBounce(vel);
                stretch -= 0.75f;
            }
            // Regular, non-winning hit??
            else {
                // Choose a new block to go to!!
                Block nextBlock = GetRandomAvailableBlock(block);
                if (nextBlock != null) { // Is there a block to go to? Let's go to it!
                    SetBlockHeadingTo(nextBlock);
                }
            }


            // Make sure I start my bounce on the top of the block.
            //bottomY = block.BlockTop; // NOTE: Disabled!
            //bottomY = block.BlockTop - block.BlockHeight*0.3f; // TEST!
            bottomY -= 12f; // TEST!
            // Find how fast we have to move upward to reach this y pos, and set our vel to that!
            float fallHeight = fallHeightNeutral;//note: Disabled random fall-height! + Random.Range(-30,30); // slightly randomize how high we go.
            float blockToTop = blockHeadingTo.BlockTop;//HitBox.center;
            float peakBPosY = blockToTop + fallHeight;
            peakBPosY = Mathf.Max(peakBPosY, bottomY+minBounceHeight); // enforce minimum bounce height.
            float displacementY = blockToTop - bottomY;
            float yDist = Mathf.Max (0, peakBPosY-pos.y);
            float yVel = Mathf.Sqrt(2*-gravity.y*yDist); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)

            // float timeOfFlight = 2f*yVel / g;
            float g = gravity.y;
            float timeOfFlight = (-yVel - Mathf.Sqrt(yVel*yVel + 2*g*displacementY)) / g; // note that this is in seconds DIVIDED by FixedUpdate FPS.

            float predBlockPosX = GetBlockPosX(blockHeadingTo, bottomY, yVel); // calculate where the Block is gonna be when I reach its y pos.
            float xDist = predBlockPosX - pos.x;
            float xVel = xDist / timeOfFlight;

            vel = new Vector2(xVel, yVel);

            if (doBounceUpOffscreen) {
                vel = new Vector2(0, 40f); // rocketmaaan!
                gravity = new Vector2(0, -0.35f); // much less gravity!
                block.OnPlayerBounceUpOffscreenFromMe(); // smack dat square.
                startingPlayerColor = bodyColor; // set this now! I'M the color to emulate!
                Invoke("DisableMyGameObject", 0.8f); // HARDCODED delay. Hide me once I'm off-screen so I don't show up again when this level animates down.
            }
            //// NOT bouncing offscreen?
            //else {
            //    if (didPaintBlock) { // I just painted this guy! Now, rando my colo.
            //        bodyColor = GetRandomHappyColor();
            //    }
            //}
        }

        private void SquishFromBounce() {
            stretch -= 0.25f; // Deform a lil'!
        }
        public void Explode(LoseReasons reason) {
            isDead = true;
            i_body.sprite = s_bodyDashedOutline;
            bodyColor = new Color(1f, 0.1f, 0f);
            stretch = stretchVel = 0;
            ApplyStretch();
            gameController.OnPlayerDie(reason);
        }

        private void DisableMyGameObject() { this.gameObject.SetActive(false); } // Used when we bounce up off-screen.



        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update () {
            if (Time.timeScale == 0) { return; } // No time? No dice.
            if (myLevel.IsAnimating) { return; } // Animating in? Don't move.
            if (gameController.IsFUEPlayerFrozen) { return; } // I'm all frozen? Do nothin'.
            if (isDead) { return; }

            ApplyGravity();
            ApplyVel();
            ApplyBounds();

            UpdateScaleRotation();
        }
        private void ApplyGravity() {
            vel += gravity * timeScale;
        }
        private void ApplyVel() {
            pos += vel * timeScale;
        }
        private void ApplyBounds() {
            if (blockHeadingTo == null) { return; } // Safety check.

            float boundsY = blockHeadingTo.HitBox.yMin;
            bool autoBounce = blockHeadingTo.IsSatisfied || !blockHeadingTo.DoTap; // I automatically bounce on satisfied OR don't-tap Blocks. Matters for A) When the level's over, and B) Unpaintable/Don't-tap Blocks.
            //autoBounce = true; // qq
            if (autoBounce) { // If I'm auto-bouncing, set my bounds to the VISUAL top of the block! So it looks like a solid, proper bounce.
                boundsY = blockHeadingTo.BlockTop;
            }


            // If I've passed too far into the block I'm heading towards, explode OR bounce me!
            if (vel.y<0 && bottomY<boundsY) {
                bottomY = boundsY;
				// Auto-bounce? Just happily bounce on the block's head. ;)
                if (autoBounce) {
					BounceOnBlock(blockHeadingTo);
                }
				// Do NOT auto-bounce?
				else {
					// It's a don't-tap block?? Do dat bounce!
					if (!blockHeadingTo.DoTap) {
						BounceOnBlock(blockHeadingTo);
					}
					// It's a NORMAL block. We've gone too far! Die.
					else {
                        Explode(LoseReasons.MissedTap);
					}
                }
            }
        }
        private void UpdateScaleRotation() {
            // Stretch me!
            stretch += stretchVel;
            float stretchTarget = 0;
            if (gameController.WinningPlayer==this) { // If I'm the winning player, freely deform me based on my y vel! :)
                stretchTarget = Mathf.Abs(vel.y)*0.015f;//.Max(0, -vel.y)*0.01f;
            }
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



        // ----------------------------------------------------------------
        //  Physics Events
        // ----------------------------------------------------------------
        private void OnTriggerEnter2D(Collider2D col) {
            // Block?!
            Block block = GetBlockFromCollider(col);
            if (block != null) {
                if (!blocksTouching.Contains(block)) { // Safety check.
                    blocksTouching.Add(block);
                }
                else { Debug.LogWarning("Player's trying to ADD a Block to blocksTouching that's ALREADY in its list!"); }
            }
        }
        private void OnTriggerExit2D(Collider2D col) {
            // Block?!
            Block block = GetBlockFromCollider(col);
            if (block != null) {
                if (blocksTouching.Contains(block)) { // Safety check.
                    blocksTouching.Remove(block);
                }
                else { Debug.LogWarning("Player's trying to REMOVE a Block from blocksTouching that's NOT in its list!"); }
            }
        }


    }
}


//     public void OnPressJumpButton() {
//         Block blockTouching = GetUnpaintedBlockTouching();
//// I AM touching a block!...
//         if (blockTouching != null) {
//  // Standard block? Bounce!
//  if (blockTouching.DoTap) {
//              BounceOnBlock(blockTouching);
//  }
//  // Ooh, a DON'T-tap block. Explode!
//  else {
//                 blockTouching.OnPlayerPressJumpOnMeInappropriately();
//      Explode();
//  }
//         }
//// I'm NOT touching any block...
//    else {
//        Explode();
//    }
//}

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

