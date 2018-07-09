using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BouncePaint {
    public class Level : MonoBehaviour {
        // Components
        private List<Player> players; // oh, balls.
        private List<Block> blocks;
        // References
        [SerializeField] private GameController gameController;
        [SerializeField] private RectTransform rt_blocks=null;
        [SerializeField] private RectTransform rt_gameWorld=null;


        // Getters / Setters
        private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
        public List<Block> Blocks { get { return blocks; } }
        public List<Player> Players { get { return players; } }
        public int LevelIndex { get; set; }


        // ----------------------------------------------------------------
        //  Destroying Elements
        // ----------------------------------------------------------------
        private void DestroyLevelComponents() {
            if (blocks!=null) {
                foreach (Block obj in blocks) {
                    Destroy(obj.gameObject);
                }
            }
            blocks = null;

            if (players!=null) {
                foreach (Player obj in players) {
                    Destroy(obj.gameObject);
                }
            }
            players = null;
        }

        // ----------------------------------------------------------------
        //  Adding Elements
        // ----------------------------------------------------------------
        //      private Block AddBlock(Vector2 blockSize, float x,float y, bool doTap) {
        //          Vector2 pos = new Vector2(x,y);
        //          return AddBlock(blockSize, pos,pos, 0,0, 1, doTap);
        //      }
        private Block AddBlock(Vector2 blockSize, float x,float y) {
            Vector2 pos = new Vector2(x,y);
            return AddBlock(blockSize, pos,pos);
        }
        private Block AddBlock(Vector2 blockSize, Vector2 posA,Vector2 posB) {
            Block newBlock = Instantiate(resourcesHandler.bouncePaint_block).GetComponent<Block>();
            newBlock.Initialize(gameController,rt_blocks, blockSize, posA,posB);
            blocks.Add(newBlock);
            return newBlock;
        }

        private Player AddPlayer() {
            Player newPlayer = Instantiate(resourcesHandler.bouncePaint_player).GetComponent<Player>();
            newPlayer.Initialize(gameController,rt_gameWorld, players.Count);
            players.Add(newPlayer);
            return newPlayer;
        }


        // ----------------------------------------------------------------
        //  Making Level!
        // ----------------------------------------------------------------
        // HARDCODED Level-adding.
        public void AddLevelComponents() {
            DestroyLevelComponents(); // Just in case
            blocks = new List<Block>();
            players = new List<Player>();
            if (resourcesHandler == null) { return; } // Safety check for runtime compile.

            // Add at least one Player.
            AddPlayer();

            // Default values
            gameController.PlayerGravityScale = 1f;
            Vector2 bs = new Vector2(50,50); // block size

            // NOTE: All coordinates are based off of a 600x800 available playing space! :)

            float b = -240; // bottom.
            int li = LevelIndex;
            int i=1; // TEMP! Until we make levels into XML or Json.
            if (false) {}


            // Simple, together.
            else if (li == i++) {
                AddBlock(bs, 0,b);
            }
            else if (li == i++) {
                AddBlock(bs, -30,b);
                AddBlock(bs,  30,b);
            }
            else if (li == i++) {
                AddBlock(bs, -60,b);
                AddBlock(bs,   0,b);
                AddBlock(bs,  60,b);
            }
            else if (li == i++) {
                AddBlock(bs, -90,b);
                AddBlock(bs, -30,b);
                AddBlock(bs,  30,b);
                AddBlock(bs,  90,b);
            }

            // Larger X gaps.
            else if (li == i++) {
                AddBlock(bs, -100,b);
                AddBlock(bs,  100,b);
            }
            else if (li == i++) {
                AddBlock(bs, -200,b);
                AddBlock(bs,  100,b);
                AddBlock(bs,  200,b);
            }
            else if (li == i++) {
                AddBlock(bs, -200,b);
                AddBlock(bs, -140,b);
                AddBlock(bs,  -80,b);
                AddBlock(bs,  200,b);
            }
            else if (li == i++) {
                AddBlock(bs, -200,b);
                AddBlock(bs, -140,b);
                AddBlock(bs,    0,b+50);
                AddBlock(bs,  200,b);
                AddBlock(bs,  140,b);
            }

            // Offset Y positions
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, -80,b);
            //              AddBlock(blockSize,   0,b+100);
            //              AddBlock(blockSize,  80,b);
            //          }
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, -80,b);
            //              AddBlock(blockSize,  80,b+200);
            //              AddBlock(blockSize,  80,b);
            //          }
            else if (li == i++) {
                AddBlock(bs, -180,b+160);
                AddBlock(bs, -100,b);
                AddBlock(bs,  100,b);
                AddBlock(bs,  180,b+160);
            }
            else if (li == i++) {
                AddBlock(bs, -180,b);
                AddBlock(bs,  -60,b+100);
                AddBlock(bs,    0,b+300);
                AddBlock(bs,   60,b+100);
                AddBlock(bs,  180,b);
            }
            else if (li == i++) {
                AddBlock(bs, -220,b+100);
                AddBlock(bs,  -90,b);
                AddBlock(bs,  -30,b+160);
                AddBlock(bs,   30,b+20);
                AddBlock(bs,  130,b);
                AddBlock(bs,  180,b+80);
            }
            else if (li == i++) {
                AddBlock(bs, -240,b+300);
                AddBlock(bs, -120,b+200);
                AddBlock(bs,  -60,b);
                AddBlock(bs,    0,b+150);
                AddBlock(bs,   60,b+200);
                AddBlock(bs,  180,b);
                AddBlock(bs,  240,b+100);
            }
            else if (li == i++) {
                AddBlock(bs, -210,b);
                AddBlock(bs, -150,b+40);
                AddBlock(bs,  -90,b+80);
                AddBlock(bs,  -30,b+120);
                AddBlock(bs,   30,b+160);
                AddBlock(bs,   90,b+200);
                AddBlock(bs,  150,b+240);
                AddBlock(bs,  210,b+280);
            }

            // Vertical Stacks
            else if (li == i++) {
                AddBlock(bs, 0,b);
                AddBlock(bs, 0,b+60);
                AddBlock(bs, 0,b+120);
                AddBlock(bs, 0,b+180);
            }
            else if (li == i++) {
                AddBlock(bs, 0,b);
                AddBlock(bs, 0,b+80);
                AddBlock(bs, 0,b+160);
                AddBlock(bs, 0,b+240);
                AddBlock(bs, 0,b+320);
            }
            else if (li == i++) {
                AddBlock(bs, -120,b);
                AddBlock(bs,    0,b);
                AddBlock(bs,  120,b);
                AddBlock(bs, -120,b+180);
                AddBlock(bs,    0,b+180);
                AddBlock(bs,  120,b+180);
            }
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, -80,b);
            //              AddBlock(blockSize,   0,b);
            //              AddBlock(blockSize,   0,b+80);
            //              AddBlock(blockSize,   0,b+160);
            //              AddBlock(blockSize,   0,b+240);
            //              AddBlock(blockSize,  80,b);
            //          }
            else if (li == i++) { // 8 plus
                AddBlock(bs,    0,b);
                AddBlock(bs,    0,b+80);
                AddBlock(bs,    0,b+240);
                AddBlock(bs,    0,b+320);
                AddBlock(bs, -160,b+160);
                AddBlock(bs,  -80,b+160);
                AddBlock(bs,   80,b+160);
                AddBlock(bs,  160,b+160);
            }



            // Traveling Blocks
            else if (li == i++) {
                AddBlock(bs, new Vector2(-100,b), new Vector2(100,b));
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-60,b), new Vector2(-160,b));
                AddBlock(bs, new Vector2(60,b), new Vector2(160,b));
                AddBlock(bs, 0,b);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-100,b), new Vector2(-160,b));
                AddBlock(bs, new Vector2(140,b), new Vector2(200,b));
                AddBlock(bs, new Vector2(-100,b+100), new Vector2(100,b+100));
                AddBlock(bs, -200,b+140);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-120,b), new Vector2(-240,b));
                AddBlock(bs, new Vector2(-60,b), new Vector2(-120,b));
                AddBlock(bs, 0,b);
                AddBlock(bs, new Vector2( 120,b), new Vector2(240,b));
                AddBlock(bs, new Vector2( 60,b), new Vector2(120,b));
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-120,b), new Vector2(-240,b));
                AddBlock(bs, new Vector2(-60,b), new Vector2(-120,b));
                AddBlock(bs, 0,b);
                AddBlock(bs, new Vector2(-200,b+250), new Vector2(200,b+250));
                AddBlock(bs, new Vector2( 240,b), new Vector2(120,b));
                AddBlock(bs, new Vector2( 120,b), new Vector2(60,b));
            }
            // Faster Traveling Blocks
            else if (li == i++) {
                float w = 120;
                AddBlock(bs, new Vector2(-240,b), new Vector2(-240+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(-180,b), new Vector2(-180+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(-120,b), new Vector2(-120+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2( -60,b), new Vector2( -60+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(   0,b), new Vector2(   0+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(  60,b), new Vector2(  60+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2( 120,b), new Vector2( 120+w,b)).SetSpeed(1.4f);
            }
            else if (li == i++) {
                float w = 160;
                AddBlock(bs, -260,b+240);
                AddBlock(bs, new Vector2(-200,b+160), new Vector2(-200+w,b+160)).SetSpeed(2f);
                AddBlock(bs, new Vector2(-140,b+ 80), new Vector2(-140+w,b+ 80)).SetSpeed(2f);
                AddBlock(bs, new Vector2( -80,b    ), new Vector2( -80+w,b    )).SetSpeed(2f);
                AddBlock(bs, new Vector2( -20,b+ 80), new Vector2( -20+w,b+ 80)).SetSpeed(2f);
                AddBlock(bs, new Vector2(  40,b+160), new Vector2(  40+w,b+160)).SetSpeed(2f);
                AddBlock(bs,  260,b+240);
            }
            else if (li == i++) {
                float w = 120;
                AddBlock(bs, new Vector2(-240,b), new Vector2(-240+w,b)).SetSpeed(2f, 0f);
                AddBlock(bs, new Vector2(-180,b), new Vector2(-180+w,b)).SetSpeed(2f, 0.2f);
                AddBlock(bs, new Vector2(-120,b), new Vector2(-120+w,b)).SetSpeed(2f, 0.4f);
                AddBlock(bs, new Vector2( -60,b), new Vector2( -60+w,b)).SetSpeed(2f, 0.6f);
                AddBlock(bs, new Vector2(   0,b), new Vector2(   0+w,b)).SetSpeed(2f, 0.8f);
                AddBlock(bs, new Vector2(  60,b), new Vector2(  60+w,b)).SetSpeed(2f, 1.0f);
                AddBlock(bs, new Vector2( 120,b), new Vector2( 120+w,b)).SetSpeed(2f, 1.2f);
            }
            //else if (levelIndex == i++) {
            //    float w = 120;
            //    AddBlock(blockSize, new Vector2(-240,b), new Vector2(-240+w,b)).SetTravSpeed(2f, 0f);
            //    AddBlock(blockSize, new Vector2(-180,b), new Vector2(-180+w,b)).SetTravSpeed(2f, 0.3f);
            //    AddBlock(blockSize, new Vector2(-120,b), new Vector2(-120+w,b)).SetTravSpeed(2f, 0.6f);
            //    AddBlock(blockSize, new Vector2( -60,b), new Vector2( -60+w,b)).SetTravSpeed(2f, 0.9f);
            //    AddBlock(blockSize, new Vector2(   0,b), new Vector2(   0+w,b)).SetTravSpeed(2f, 1.2f);
            //    AddBlock(blockSize, new Vector2(  60,b), new Vector2(  60+w,b)).SetTravSpeed(2f, 1.5f);
            //    AddBlock(blockSize, new Vector2( 120,b), new Vector2( 120+w,b)).SetTravSpeed(2f, 1.8f);
            //}

            // Varying-Speed Traveling Blocks
            else if (li == i++) {
                AddBlock(bs, new Vector2(-150,b), new Vector2(-240,b)).SetSpeed(1f);
                AddBlock(bs, new Vector2(-100,b+50), new Vector2(100,b+50)).SetSpeed(2f);
                AddBlock(bs, new Vector2( 150,b), new Vector2(240,b)).SetSpeed(1f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-240,b+100), new Vector2(-150,b+100)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2(-120,b), new Vector2(40,b)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2( -40,b+200), new Vector2(120,b+200)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2( 150,b+100), new Vector2(240,b+100)).SetSpeed(2.5f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2( 160,b+200), new Vector2(-160,b+200)).SetSpeed(3f);
                AddBlock(bs, new Vector2(-150,b), new Vector2(-240,b)).SetSpeed(4f);
                AddBlock(bs, new Vector2(-160,b+100), new Vector2( 160,b+100)).SetSpeed(3f);
                AddBlock(bs, new Vector2( 150,b), new Vector2(240,b)).SetSpeed(4f);
            }
            //else if (levelIndex == i++) {
            //    AddBlock(blockSize, new Vector2(-150,b+100), new Vector2(-240,b+100)).SetTravSpeed(2f);
            //    AddBlock(blockSize, new Vector2(-160,b), new Vector2(160,b)).SetTravSpeed(2f);
            //    AddBlock(blockSize, new Vector2(-160,b+200), new Vector2(160,b+200)).SetTravSpeed(2f, 0.5f);
            //    AddBlock(blockSize, new Vector2( 240,b+100), new Vector2(150,b+100)).SetTravSpeed(2f);
            //}
            else if (li == i++) {
                AddBlock(bs, new Vector2(-240,b    ), new Vector2( -80,b    )).SetSpeed(5f);
                AddBlock(bs, new Vector2( 240,b    ), new Vector2(  80,b    )).SetSpeed(5f);
                AddBlock(bs, new Vector2( -80,b+160), new Vector2(-240,b+160)).SetSpeed(5f);
                AddBlock(bs, new Vector2(  80,b+160), new Vector2( 240,b+160)).SetSpeed(5f);
                AddBlock(bs, new Vector2(-240,b+320), new Vector2( -80,b+320)).SetSpeed(5f);
                AddBlock(bs, new Vector2( 240,b+320), new Vector2(  80,b+320)).SetSpeed(5f);
            }

            else if (li == i++) {
                AddBlock(bs, new Vector2(-200,b    ), new Vector2( 200,b    )).SetSpeed(3f);
                AddBlock(bs, new Vector2(-200,b+100), new Vector2( 200,b+100)).SetSpeed(3f, 3.142f);
                AddBlock(bs, new Vector2(-200,b+200), new Vector2( 200,b+200)).SetSpeed(3f);
                AddBlock(bs, new Vector2(-200,b+300), new Vector2( 200,b+300)).SetSpeed(3f, 3.142f);
            }



            // Weird-Shapes Interlude
            //          else if (levelIndex == i++) { // 3x3 larger grid
            //              AddBlock(blockSize, -120,b);
            //              AddBlock(blockSize,    0,b);
            //              AddBlock(blockSize,  120,b);
            //              AddBlock(blockSize, -120,b+120);
            //              AddBlock(blockSize,    0,b+120);
            //              AddBlock(blockSize,  120,b+120);
            //              AddBlock(blockSize, -120,b+240);
            //              AddBlock(blockSize,    0,b+240);
            //              AddBlock(blockSize,  120,b+240);
            //          }
            else if (li == i++) { // 3x3 tight grid
                AddBlock(bs, -60,b);
                AddBlock(bs,   0,b);
                AddBlock(bs,  60,b);
                AddBlock(bs, -60,b+60);
                AddBlock(bs,   0,b+60);
                AddBlock(bs,  60,b+60);
                AddBlock(bs, -60,b+120);
                AddBlock(bs,   0,b+120);
                AddBlock(bs,  60,b+120);
            }



            // Weird-Shapes Traveling Blocks
            else if (li == i++) {
                AddBlock(bs, -220,b);
                AddBlock(bs, new Vector2( -220,b+ 60), new Vector2(-140,b+60 )).SetSpeed(0.7f);
                AddBlock(bs, new Vector2( -220,b+120), new Vector2( -60,b+120)).SetSpeed(0.7f);
                AddBlock(bs, new Vector2( -220,b+180), new Vector2(  20,b+180)).SetSpeed(0.7f);
                AddBlock(bs, new Vector2( -220,b+240), new Vector2( 100,b+240)).SetSpeed(0.7f);
                AddBlock(bs, new Vector2( -220,b+300), new Vector2( 180,b+300)).SetSpeed(0.7f);
                AddBlock(bs, new Vector2( -220,b+360), new Vector2( 260,b+360)).SetSpeed(0.7f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2( -100,b    ), new Vector2(100,b    )).SetSpeed(1f, 0f);
                AddBlock(bs, new Vector2( -100,b+60 ), new Vector2(100,b+60 )).SetSpeed(1f, 0.5f);
                AddBlock(bs, new Vector2( -100,b+120), new Vector2(100,b+120)).SetSpeed(1f, 1f);
                AddBlock(bs, new Vector2( -100,b+180), new Vector2(100,b+180)).SetSpeed(1f, 1.5f);
                AddBlock(bs, new Vector2( -100,b+240), new Vector2(100,b+240)).SetSpeed(1f, 2f);
                AddBlock(bs, new Vector2( -100,b+300), new Vector2(100,b+300)).SetSpeed(1f, 2.5f);
                AddBlock(bs, new Vector2( -100,b+360), new Vector2(100,b+360)).SetSpeed(1f, 3f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2( -160,b    ), new Vector2( 160,b    )).SetSpeed(1f, 0f);
                AddBlock(bs, new Vector2( -160,b+60 ), new Vector2( 160,b+60 )).SetSpeed(1f, -0.8f);
                AddBlock(bs, new Vector2( -160,b+120), new Vector2( 160,b+120)).SetSpeed(1f, -1.6f);
                AddBlock(bs, new Vector2( -160,b+180), new Vector2( 160,b+180)).SetSpeed(1f, -2.4f);
                AddBlock(bs, new Vector2( -160,b+240), new Vector2( 160,b+240)).SetSpeed(1f, -3.2f);
                AddBlock(bs, new Vector2( -160,b+300), new Vector2( 160,b+300)).SetSpeed(1f, -4f);
                AddBlock(bs, new Vector2( -160,b+360), new Vector2( 160,b+360)).SetSpeed(1f, -4.8f);
            }
            else if (li == i++) {
                AddBlock(bs,  220,b);
                AddBlock(bs, new Vector2(220,b+50 ), new Vector2( 140,b+50 )).SetSpeed(1f, -0.4f);
                AddBlock(bs, new Vector2(220,b+100), new Vector2(  60,b+100)).SetSpeed(1f, -0.8f);
                AddBlock(bs, new Vector2(220,b+150), new Vector2( -20,b+150)).SetSpeed(1f, -1.2f);
                AddBlock(bs, new Vector2(220,b+200), new Vector2(-100,b+200)).SetSpeed(1f, -1.6f);
                AddBlock(bs, new Vector2(220,b+250), new Vector2(-180,b+250)).SetSpeed(1f, -2f);
                AddBlock(bs, new Vector2(220,b+300), new Vector2(-260,b+300)).SetSpeed(1f, -2.4f);
                AddBlock(bs, new Vector2(220,b+350), new Vector2(-260,b+350)).SetSpeed(1f, -2.8f);
            }
            else if (li == i++) {
                AddBlock(bs, -160,b);
                AddBlock(bs, new Vector2(-160,b+ 60), new Vector2(160,b+ 60)).SetSpeed(0.4f, Mathf.PI);
                AddBlock(bs, new Vector2(-160,b+120), new Vector2(160,b+120)).SetSpeed(0.8f);
                AddBlock(bs, new Vector2(-160,b+180), new Vector2(160,b+180)).SetSpeed(1.2f, Mathf.PI);
                AddBlock(bs, new Vector2(-160,b+240), new Vector2(160,b+240)).SetSpeed(1.6f);
                AddBlock(bs, new Vector2(-160,b+300), new Vector2(160,b+300)).SetSpeed(2f, Mathf.PI);
                AddBlock(bs, new Vector2(-160,b+360), new Vector2(160,b+360)).SetSpeed(2.4f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-160,b    ), new Vector2( 160,b    )).SetSpeed(1f, 0f);
                AddBlock(bs, new Vector2( 160,b+50 ), new Vector2(-160,b+50 )).SetSpeed(1f, 0.2f);
                AddBlock(bs, new Vector2(-160,b+100), new Vector2( 160,b+100)).SetSpeed(1f, 0.4f);
                AddBlock(bs, new Vector2( 160,b+150), new Vector2(-160,b+150)).SetSpeed(1f, 0.6f);
                AddBlock(bs, new Vector2(-160,b+200), new Vector2( 160,b+200)).SetSpeed(1f, 0.8f);
                AddBlock(bs, new Vector2( 160,b+250), new Vector2(-160,b+250)).SetSpeed(1f, 1f);
                AddBlock(bs, new Vector2(-160,b+300), new Vector2( 160,b+300)).SetSpeed(1f, 1.2f);
                AddBlock(bs, new Vector2( 160,b+350), new Vector2(-160,b+350)).SetSpeed(1f, 1.4f);
            }
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, new Vector2(-160,b    ), new Vector2( 160,b    )).SetTravSpeed(1f, 0f);
            //              AddBlock(blockSize, new Vector2( 160,b+55 ), new Vector2(-160,b+55 )).SetTravSpeed(1f, 0.2f);
            //              AddBlock(blockSize, new Vector2(-160,b+110), new Vector2( 160,b+110)).SetTravSpeed(1f, 0.4f);
            //              AddBlock(blockSize, new Vector2( 160,b+165), new Vector2(-160,b+165)).SetTravSpeed(1f, 0.6f);
            //              AddBlock(blockSize, new Vector2(-160,b+220), new Vector2( 160,b+220)).SetTravSpeed(1f, 0.8f);
            //              AddBlock(blockSize, new Vector2( 160,b+275), new Vector2(-160,b+275)).SetTravSpeed(1f, 1f);
            //              AddBlock(blockSize, new Vector2(-160,b+330), new Vector2( 160,b+330)).SetTravSpeed(1f, 1.2f);
            //          }
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, -220,b);
            //              AddBlock(blockSize, new Vector2( -220,b+60 ), new Vector2(-140,b+60 )).SetTravSpeed(0.4f);
            //              AddBlock(blockSize, new Vector2( -220,b+120), new Vector2( -60,b+120)).SetTravSpeed(0.8f);
            //              AddBlock(blockSize, new Vector2( -220,b+180), new Vector2(  20,b+180)).SetTravSpeed(1.2f);
            //              AddBlock(blockSize, new Vector2( -220,b+240), new Vector2( 100,b+240)).SetTravSpeed(1.6f);
            //              AddBlock(blockSize, new Vector2( -220,b+300), new Vector2( 180,b+300)).SetTravSpeed(2f);
            //              AddBlock(blockSize, new Vector2( -220,b+360), new Vector2( 260,b+360)).SetTravSpeed(2.4f);
            //          }
            else if (li == i++) { // Large 3x3 drifters
                float o = 0.6f;
                AddBlock(bs, new Vector2(-200,b    ), new Vector2(-100,b    )).SetSpeed(0.6f, o*0);
                AddBlock(bs, new Vector2( -50,b    ), new Vector2(  50,b    )).SetSpeed(0.6f, o*1);
                AddBlock(bs, new Vector2( 100,b    ), new Vector2( 200,b    )).SetSpeed(0.6f, o*2);
                AddBlock(bs, new Vector2(-200,b+150), new Vector2(-100,b+150)).SetSpeed(0.6f, o*1);
                AddBlock(bs, new Vector2( -50,b+150), new Vector2(  50,b+150)).SetSpeed(0.6f, o*2);
                AddBlock(bs, new Vector2( 100,b+150), new Vector2( 200,b+150)).SetSpeed(0.6f, o*3);
                AddBlock(bs, new Vector2(-200,b+300), new Vector2(-100,b+300)).SetSpeed(0.6f, o*2);
                AddBlock(bs, new Vector2( -50,b+300), new Vector2(  50,b+300)).SetSpeed(0.6f, o*3);
                AddBlock(bs, new Vector2( 100,b+300), new Vector2( 200,b+300)).SetSpeed(0.6f, o*4);

                //AddBlock(blockSize, new Vector2(-125,b+ 75), new Vector2( -25,b+ 75)).SetTravSpeed(0.6f, o*0.7f).SetDontTap();
                //AddBlock(blockSize, new Vector2(  25,b+ 75), new Vector2( 175,b+ 75)).SetTravSpeed(0.6f, o*0).SetDontTap();
                //AddBlock(blockSize, new Vector2(-125,b+225), new Vector2( -25,b+225)).SetTravSpeed(0.6f, o*0).SetDontTap();
                //AddBlock(blockSize, new Vector2(  25,b+225), new Vector2( 175,b+225)).SetTravSpeed(0.6f, o*0).SetDontTap();
            }



            // Don't-Tap Blocks
            else if (li == i++) {
                AddBlock(bs,    0,b).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, -120,b);
                AddBlock(bs,    0,b);
                AddBlock(bs,  120,b).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, -120,b);
                AddBlock(bs,  -60,b).SetDontTap();
                AddBlock(bs,    0,b);
                AddBlock(bs,   60,b);
                AddBlock(bs,  120,b).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, -150,b).SetDontTap();
                AddBlock(bs,  -90,b).SetDontTap();
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b);
                AddBlock(bs,   90,b).SetDontTap();
                AddBlock(bs,  150,b);
            }
            else if (li == i++) {
                AddBlock(bs, -220,b);
                AddBlock(bs,  -90,b);
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b).SetDontTap();
                AddBlock(bs,  160,b);
                AddBlock(bs,  220,b).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, -210,b+10).SetDontTap();
                AddBlock(bs, -150,b+80);
                AddBlock(bs,  -90,b+15).SetDontTap();
                AddBlock(bs,  -30,b+40);
                AddBlock(bs,   30,b+20);
                AddBlock(bs,   90,b).SetDontTap();
                AddBlock(bs,  150,b+50);
                AddBlock(bs,  210,b+45);
            }
            else if (li == i++) {
                AddBlock(bs, -210,b+180);
                AddBlock(bs, -150,b+120);
                AddBlock(bs,  -90,b+100).SetDontTap();
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b+300);
                AddBlock(bs,   90,b+40).SetDontTap();
                AddBlock(bs,  150,b+160);
                AddBlock(bs,  210,b+80);
            }
            else if (li == i++) {
                //              AddBlock(blockSize, -240,b+160);
                AddBlock(bs, -180,b+140);
                AddBlock(bs, -120,b+80);
                AddBlock(bs,  -60,b+40);
                AddBlock(bs,    0,b+360).SetDontTap();
                AddBlock(bs,   60,b+40);
                AddBlock(bs,  120,b+80);
                AddBlock(bs,  180,b+140);
                //              AddBlock(blockSize,  240,b+160);
            }
            else if (li == i++) {
                AddBlock(bs, -200,b+120).SetDontTap();
                AddBlock(bs, -200,b+180).SetDontTap();
                AddBlock(bs, -140,b+120).SetDontTap();
                AddBlock(bs, -140,b+180).SetDontTap();
                AddBlock(bs,   0,b);
                AddBlock(bs,  60,b+60);
                AddBlock(bs,  120,b+120);
                AddBlock(bs,  180,b+180);
                AddBlock(bs,  240,b+240);
            }
            else if (li == i++) {
                AddBlock(bs, -120,b);
                AddBlock(bs,  120,b);
                AddBlock(bs, -120,b+280).SetDontTap();
                AddBlock(bs,  120,b+280).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, -120,b).SetDontTap();
                AddBlock(bs,  -60,b);
                AddBlock(bs,   60,b).SetDontTap();
                AddBlock(bs,  120,b);
                AddBlock(bs, -120,b+250);
                AddBlock(bs,  -60,b+250).SetDontTap();
                AddBlock(bs,   60,b+250);
                AddBlock(bs,  120,b+250).SetDontTap();
            }
            //          else if (levelIndex == i++) {
            //              AddBlock(blockSize, -100,b).SetDontTap();
            //              AddBlock(blockSize, -100,b+60).SetDontTap();
            //              AddBlock(blockSize, -100,b+120).SetDontTap();
            //              AddBlock(blockSize, -100,b+180).SetDontTap();
            //              AddBlock(blockSize,  100,b);
            //              AddBlock(blockSize,  100,b+60);
            //              AddBlock(blockSize,  100,b+120);
            //              AddBlock(blockSize,  100,b+180);
            //          }
            else if (li == i++) {
                AddBlock(bs,  160,b+60).SetDontTap();
                AddBlock(bs,  160,b+120).SetDontTap();
                AddBlock(bs,  160,b+180).SetDontTap();
                AddBlock(bs,  160,b+240).SetDontTap();
                AddBlock(bs,  160,b+300).SetDontTap();
                AddBlock(bs,  100,b);
                AddBlock(bs,   40,b);
                AddBlock(bs,  -20,b);
                AddBlock(bs,  -80,b);
                AddBlock(bs, -140,b);
            }
            else if (li == i++) {
                AddBlock(bs, -180,b);
                AddBlock(bs, -120,b);
                AddBlock(bs,  -60,b);
                AddBlock(bs,    0,b);
                AddBlock(bs,   60,b);
                AddBlock(bs,  120,b);
                AddBlock(bs,  180,b);
                AddBlock(bs, -180,b+200).SetDontTap();
                AddBlock(bs, -120,b+200).SetDontTap();
                AddBlock(bs,  -60,b+200).SetDontTap();
                AddBlock(bs,    0,b+200).SetDontTap();
                AddBlock(bs,   60,b+200).SetDontTap();
                AddBlock(bs,  120,b+200).SetDontTap();
                AddBlock(bs,  180,b+200).SetDontTap();
            }
            else if (li == i++) { // Large 3x3 with freckles
                float g = 200;
                AddBlock(bs, -g,b    );
                AddBlock(bs,  0,b    );
                AddBlock(bs,  g,b    );
                AddBlock(bs, -g,b+g*1);
                AddBlock(bs,  0,b+g*1);
                AddBlock(bs,  g,b+g*1);
                AddBlock(bs, -g,b+g*2);
                AddBlock(bs,  0,b+g*2);
                AddBlock(bs,  g,b+g*2);

                AddBlock(bs, -g*0.5f,b+g*0.5f).SetDontTap();
                AddBlock(bs,  g*0.5f,b+g*0.5f).SetDontTap();
                AddBlock(bs, -g*0.5f,b+g*1.5f).SetDontTap();
                AddBlock(bs,  g*0.5f,b+g*1.5f).SetDontTap();
            }





            // Traveling, No-Tap Blocks
            else if (li == i++) {
                AddBlock(bs, new Vector2(-160,b    ), new Vector2( 160,b   )).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, new Vector2( 160,b+50 ), new Vector2(-160,b+50)).SetSpeed(1f, 0.2f).SetDontTap();
                AddBlock(bs, new Vector2(-160,b+100), new Vector2( 160,b+100)).SetSpeed(1f, 0.4f);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-160,b    ), new Vector2( 160,b    )).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, new Vector2( 160,b+50 ), new Vector2(-160,b+50 )).SetSpeed(1f, 0.2f).SetDontTap();
                AddBlock(bs, new Vector2(-160,b+100), new Vector2( 160,b+100)).SetSpeed(1f, 0.4f);
                AddBlock(bs, new Vector2( 160,b+150), new Vector2(-160,b+150)).SetSpeed(1f, 0.6f);
                AddBlock(bs, new Vector2(-160,b+200), new Vector2( 160,b+200)).SetSpeed(1f, 0.8f);
                AddBlock(bs, new Vector2( 160,b+250), new Vector2(-160,b+250)).SetSpeed(1f, 1f);
                AddBlock(bs, new Vector2(-160,b+300), new Vector2( 160,b+300)).SetSpeed(1f, 1.2f).SetDontTap();
                AddBlock(bs, new Vector2( 160,b+350), new Vector2(-160,b+350)).SetSpeed(1f, 1.4f).SetDontTap();
            }
            //else if (levelIndex == i++) {
            //    AddBlock(blockSize, new Vector2(-240,b    ), new Vector2(-180,b    )).SetTravSpeed(8f, 0f).SetDontTap();
            //    AddBlock(blockSize, new Vector2( 240,b    ), new Vector2( 180,b    )).SetTravSpeed(8f, 3.142f).SetDontTap();
            //    AddBlock(blockSize, new Vector2( 240,b+300), new Vector2( 180,b+300)).SetTravSpeed(8f, 3.142f).SetDontTap();
            //    AddBlock(blockSize, new Vector2(-240,b+300), new Vector2(-180,b+300)).SetTravSpeed(8f, 0f).SetDontTap();
            //    AddBlock(blockSize, -40,b+100);
            //    AddBlock(blockSize,  40,b+100);
            //    AddBlock(blockSize, -40,b+180);
            //    AddBlock(blockSize,  40,b+180);
            //    //AddBlock(blockSize, new Vector2(-250,b+100), new Vector2(-160,b+100));
            //    //AddBlock(blockSize, new Vector2( 250,b+100), new Vector2( 160,b+100));
            //}
            //else if (levelIndex == i++) {
            //    AddBlock(blockSize, new Vector2(-250,b    ), new Vector2(-100,b    )).SetTravSpeed(6f, 0f).SetDontTap();
            //    AddBlock(blockSize, new Vector2( 250,b    ), new Vector2( 100,b    )).SetTravSpeed(6f, 0f).SetDontTap();
            //    AddBlock(blockSize, new Vector2( 250,b+300), new Vector2( 100,b+300)).SetTravSpeed(6f, 0f).SetDontTap();
            //    AddBlock(blockSize, new Vector2(-250,b+300), new Vector2(-100,b+300)).SetTravSpeed(6f, 0f).SetDontTap();
            //    AddBlock(blockSize, -50,b+100);
            //    AddBlock(blockSize,  50,b+100);
            //    AddBlock(blockSize, -50,b+200);
            //    AddBlock(blockSize,  50,b+200);
            //}
            else if (li == i++) {
                AddBlock(bs, new Vector2(-240,b    ), new Vector2( 240,b    )).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, new Vector2( 240,b    ), new Vector2(-240,b    )).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, new Vector2( 240,b+300), new Vector2(-240,b+300)).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, new Vector2(-240,b+300), new Vector2( 240,b+300)).SetSpeed(1f, 0f).SetDontTap();
                AddBlock(bs, -40,b+100);
                AddBlock(bs,  40,b+100);
                AddBlock(bs, -40,b+180);
                AddBlock(bs,  40,b+180);
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-240,b    ), new Vector2( 240,b    ));
                AddBlock(bs, new Vector2( 240,b    ), new Vector2(-240,b    ));
                AddBlock(bs, new Vector2( 240,b+300), new Vector2(-240,b+300));
                AddBlock(bs, new Vector2(-240,b+300), new Vector2( 240,b+300));
                AddBlock(bs, -40,b+100).SetDontTap();
                AddBlock(bs,  40,b+100).SetDontTap();
                AddBlock(bs, -40,b+180).SetDontTap();
                AddBlock(bs,  40,b+180).SetDontTap();
            }
            else if (li == i++) {
                AddBlock(bs, new Vector2(-200,b    ), new Vector2(-100,b    )).SetSpeed(1f, 0.0f);
                AddBlock(bs, new Vector2( -50,b    ), new Vector2(  50,b    )).SetSpeed(1f, 0.2f).SetDontTap();
                AddBlock(bs, new Vector2( 100,b    ), new Vector2( 200,b    )).SetSpeed(1f, 0.4f);
                AddBlock(bs, new Vector2(-200,b+150), new Vector2(-100,b+150)).SetSpeed(1f, 0.6f).SetDontTap();
                AddBlock(bs, new Vector2( -50,b+150), new Vector2(  50,b+150)).SetSpeed(1f, 0.8f);
                AddBlock(bs, new Vector2( 100,b+150), new Vector2( 200,b+150)).SetSpeed(1f, 1.0f).SetDontTap();
                AddBlock(bs, new Vector2(-200,b+300), new Vector2(-100,b+300)).SetSpeed(1f, 1.2f);
                AddBlock(bs, new Vector2( -50,b+300), new Vector2(  50,b+300)).SetSpeed(1f, 1.4f).SetDontTap();
                AddBlock(bs, new Vector2( 100,b+300), new Vector2( 200,b+300)).SetSpeed(1f, 1.6f);
            }

            else if (li == i++) { // Large 3x3 drifter with freckles
                float o = 0.8f;
                AddBlock(bs, new Vector2(-200,b    ), new Vector2(-100,b    )).SetSpeed(0.8f, o*0).SetDontTap();
                AddBlock(bs, new Vector2( -50,b    ), new Vector2(  50,b    )).SetSpeed(0.8f, o*1).SetDontTap();
                AddBlock(bs, new Vector2( 100,b    ), new Vector2( 200,b    )).SetSpeed(0.8f, o*2).SetDontTap();
                AddBlock(bs, new Vector2(-200,b+150), new Vector2(-100,b+150)).SetSpeed(0.8f, o*1).SetDontTap();
                AddBlock(bs, new Vector2( -50,b+150), new Vector2(  50,b+150)).SetSpeed(0.8f, o*2).SetDontTap();
                AddBlock(bs, new Vector2( 100,b+150), new Vector2( 200,b+150)).SetSpeed(0.8f, o*3).SetDontTap();
                AddBlock(bs, new Vector2(-200,b+300), new Vector2(-100,b+300)).SetSpeed(0.8f, o*2).SetDontTap();
                AddBlock(bs, new Vector2( -50,b+300), new Vector2(  50,b+300)).SetSpeed(0.8f, o*3).SetDontTap();
                AddBlock(bs, new Vector2( 100,b+300), new Vector2( 200,b+300)).SetSpeed(0.8f, o*4).SetDontTap();
                AddBlock(bs, new Vector2(-125,b+ 75), new Vector2( -25,b+ 75)).SetSpeed(0.8f, o*0.5f);
                AddBlock(bs, new Vector2(  25,b+ 75), new Vector2( 125,b+ 75)).SetSpeed(0.8f, o*1.5f);
                AddBlock(bs, new Vector2(-125,b+225), new Vector2( -25,b+225)).SetSpeed(0.8f, o*1.5f);
                AddBlock(bs, new Vector2(  25,b+225), new Vector2( 125,b+225)).SetSpeed(0.8f, o*3.5f);
            }
            else if (li == i++) { // Large 3x3 drifter chaotic, inverted freckles, no center
                float o = -1.8f;
                AddBlock(bs, new Vector2(-200,b    ), new Vector2(-100,b    )).SetSpeed(0.6f, o*0);
                AddBlock(bs, new Vector2( -50,b    ), new Vector2(  50,b    )).SetSpeed(0.6f, o*1).SetDontTap();
                AddBlock(bs, new Vector2( 100,b    ), new Vector2( 200,b    )).SetSpeed(0.6f, o*2);
                AddBlock(bs, new Vector2(-200,b+150), new Vector2(-100,b+150)).SetSpeed(0.6f, o*1).SetDontTap();
                //AddBlock(blockSize, new Vector2( -50,b+150), new Vector2(  50,b+150), 0.6f).SetTravSpeed(o*2);
                AddBlock(bs, new Vector2( 100,b+150), new Vector2( 200,b+150)).SetSpeed(0.6f, o*3).SetDontTap();
                AddBlock(bs, new Vector2(-200,b+300), new Vector2(-100,b+300)).SetSpeed(0.6f, o*2);
                AddBlock(bs, new Vector2( -50,b+300), new Vector2(  50,b+300)).SetSpeed(0.6f, o*3).SetDontTap();
                AddBlock(bs, new Vector2( 100,b+300), new Vector2( 200,b+300)).SetSpeed(0.6f, o*4);
                AddBlock(bs, new Vector2(-125,b+ 75), new Vector2( -25,b+ 75)).SetSpeed(0.6f, o*0.5f);
                AddBlock(bs, new Vector2(  25,b+ 75), new Vector2( 125,b+ 75)).SetSpeed(0.6f, o*1.5f);
                AddBlock(bs, new Vector2(-125,b+225), new Vector2( -25,b+225)).SetSpeed(0.6f, o*1.5f);
                AddBlock(bs, new Vector2(  25,b+225), new Vector2( 125,b+225)).SetSpeed(0.6f, o*3.5f);
            }



            // Differently Sized Blocks
            else if (li == i++) { // Big ice cube
                AddBlock(bs*3,    0,b);
            }
            else if (li == i++) { // Four bigger blocks
                AddBlock(bs*2, -200,b);
                AddBlock(bs*2,  -80,b);
                AddBlock(bs*2,   80,b);
                AddBlock(bs*2,  200,b);
            }
            else if (li == i++) { // Tiny row
                AddBlock(bs*0.5f, -150,b);
                AddBlock(bs*0.5f,  -90,b);
                AddBlock(bs*0.5f,  -30,b);
                AddBlock(bs*0.5f,   30,b);
                AddBlock(bs*0.5f,   90,b);
                AddBlock(bs*0.5f,  150,b);
            }
            else if (li == i++) { // Foreshortened posse
                AddBlock(bs*0.4f, -260,b);
                AddBlock(bs*0.8f, -200,b);
                AddBlock(bs*1.3f, -130,b);
                AddBlock(bs*3   ,    0,b);
                AddBlock(bs*1.3f,  130,b);
                AddBlock(bs*0.8f,  200,b);
                AddBlock(bs*0.4f,  260,b);
            }
            else if (li == i++) { // Big base, sprinkles above
                AddBlock(bs*5f  ,    0,b-100);
                AddBlock(bs*0.4f, -100,b+120);
                AddBlock(bs*0.5f,  -55,b+200);
                AddBlock(bs*0.4f, -225,b+250);
                AddBlock(bs*0.5f,  110,b+220);
                AddBlock(bs*0.6f,  170,b+210);
                AddBlock(bs*0.45f,  220,b+320);
                AddBlock(bs*0.6f, -200,b+400);
            }
            // Rectangles
            else if (li == i++) { // Sound waves
                float w = 40f;
                AddBlock(new Vector2(w, 320), -240, b+100);
                AddBlock(new Vector2(w, 240), -180, b+100);
                AddBlock(new Vector2(w, 160), -120, b+100);
                AddBlock(new Vector2(w,  80),  -60, b+100);
                AddBlock(new Vector2(w,  40),    0, b+100);
                AddBlock(new Vector2(w,  80),   60, b+100);
                AddBlock(new Vector2(w, 160),  120, b+100);
                AddBlock(new Vector2(w, 240),  180, b+100);
                AddBlock(new Vector2(w, 320),  240, b+100);
            }
            else if (li == i++) { // Skinny pillars
                float w = 12f;
                AddBlock(new Vector2(w, 300), -240, b+100);
                AddBlock(new Vector2(w, 300), -180, b+100);
                AddBlock(new Vector2(w, 300), -120, b+100);
                AddBlock(new Vector2(w, 300),  -60, b+100);
                AddBlock(new Vector2(w, 300),    0, b+100);
                AddBlock(new Vector2(w, 300),   60, b+100);
                AddBlock(new Vector2(w, 300),  120, b+100);
                AddBlock(new Vector2(w, 300),  180, b+100);
                AddBlock(new Vector2(w, 300),  240, b+100);
            }
            else if (li == i++) { // Flat shelves
                AddBlock(new Vector2(160,20), -160, b+300);
                AddBlock(new Vector2(160,20), -160, b+200);
                AddBlock(new Vector2(160,20), -160, b+100);
                AddBlock(new Vector2(160,20), -160, b);
                AddBlock(new Vector2( 90,20),    0, b+150);
                AddBlock(new Vector2(160,20),  160, b+300);
                AddBlock(new Vector2(160,20),  160, b+200);
                AddBlock(new Vector2(160,20),  160, b+100);
                AddBlock(new Vector2(160,20),  160, b);
            }
            //else if (levelIndex == i++) { // Pluses TODO: These lvls
            //}
            //else if (levelIndex == i++) { // Zig-zags
            //}







            // TWO Balls!
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -180,b);
                AddBlock(bs,  -60,b);
                AddBlock(bs,   60,b);
                AddBlock(bs,  180,b);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -160,b);
                AddBlock(bs,  -80,b);
                AddBlock(bs,    0,b);
                AddBlock(bs,   80,b);
                AddBlock(bs,  160,b);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -150,b);
                AddBlock(bs,  -90,b);
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b);
                AddBlock(bs,   90,b);
                AddBlock(bs,  150,b);
            }
            // Offset Y positions
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -180,b+160);
                AddBlock(bs, -100,b);
                AddBlock(bs,  100,b);
                AddBlock(bs,  180,b+160);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -180,b+300);
                AddBlock(bs,  -60,b+200);
                AddBlock(bs,    0,b);
                AddBlock(bs,   60,b+200);
                AddBlock(bs,  180,b+300);
            }
            //else if (li == i++) {
            //    AddPlayer();
            //    AddBlock(bs, -220,b+100);
            //    AddBlock(bs,  -90,b);
            //    AddBlock(bs,  -30,b+160);
            //    AddBlock(bs,   30,b+20);
            //    AddBlock(bs,  130,b);
            //    AddBlock(bs,  180,b+80);
            //}
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -160,b);
                AddBlock(bs,    0,b);
                AddBlock(bs,  160,b);
                AddBlock(bs, -160,b+180);
                AddBlock(bs,    0,b+180);
                AddBlock(bs,  160,b+180);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -240,b+100);
                AddBlock(bs, -120,b+200);
                AddBlock(bs,  -60,b+10);
                AddBlock(bs,    0,b+150);
                AddBlock(bs,   60,b+90);
                AddBlock(bs,  180,b+60);
                AddBlock(bs,  240,b+300);
            }


            // Two-Balls; Don't-Tap Blocks
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -200,b);
                AddBlock(bs, -125,b).SetDontTap();
                AddBlock(bs,  -40,b);
                AddBlock(bs,   40,b);
                AddBlock(bs,  125,b).SetDontTap();
                AddBlock(bs,  200,b);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -210,b);
                AddBlock(bs, -150,b).SetDontTap();
                AddBlock(bs,  -90,b);
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b).SetDontTap();
                AddBlock(bs,   90,b);
                AddBlock(bs,  150,b);
                AddBlock(bs,  210,b).SetDontTap();
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(new Vector2(46,46), -210,b+250);
                AddBlock(new Vector2(46,46), -150,b+200);
                AddBlock(new Vector2(46,46),  -90,b+150);
                AddBlock(new Vector2(46,46),  -30,b+100);
                AddBlock(new Vector2(46,46),   30,b+ 50);
                AddBlock(new Vector2(46,46),   90,b+  0);
                AddBlock(new Vector2(46,46),  220,b+250).SetDontTap();
                AddBlock(new Vector2(46,46),  220,b+200).SetDontTap();
                AddBlock(new Vector2(46,46),  220,b+150).SetDontTap();
                AddBlock(new Vector2(46,46),  220,b+100).SetDontTap();
                AddBlock(new Vector2(46,46),  220,b+ 50).SetDontTap();
                AddBlock(new Vector2(46,46),  220,b+  0).SetDontTap();
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -210,b+240);
                AddBlock(bs, -150,b+160).SetDontTap();
                AddBlock(bs,  -90,b+ 80);
                AddBlock(bs,  -30,b+  0).SetDontTap();
                AddBlock(bs,   30,b+240).SetDontTap();
                AddBlock(bs,   90,b+160);
                AddBlock(bs,  150,b+ 80).SetDontTap();
                AddBlock(bs,  210,b+  0);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -240,b+160).SetDontTap();
                AddBlock(bs, -180,b+120);
                AddBlock(bs, -120,b+ 80);
                AddBlock(bs,  -60,b+ 40);
                AddBlock(bs,    0,b+  0).SetDontTap();
                AddBlock(bs,   60,b+ 40);
                AddBlock(bs,  120,b+ 80);
                AddBlock(bs,  180,b+120);
                AddBlock(bs,  240,b+160).SetDontTap();
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -210,b+20).SetDontTap();
                AddBlock(bs, -210,b+200);
                AddBlock(bs, -150,b+120);
                AddBlock(bs,  -90,b+100).SetDontTap();
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b+300);
                AddBlock(bs,   90,b+50).SetDontTap();
                AddBlock(bs,  150,b+160);
                AddBlock(bs,  210,b+40);
                AddBlock(bs,  210,b+240).SetDontTap();
            }
            else if (li == i++) { // Stiff flipped T
                AddPlayer();
                AddBlock(bs, -220,b);
                AddBlock(bs, -160,b);
                AddBlock(bs, -100,b);
                AddBlock(bs,    0,b+  0).SetDontTap();
                AddBlock(bs,    0,b+ 60).SetDontTap();
                AddBlock(bs,    0,b+120).SetDontTap();
                AddBlock(bs,    0,b+180).SetDontTap();
                AddBlock(bs,    0,b+240).SetDontTap();
                AddBlock(bs,    0,b+300).SetDontTap();
                AddBlock(bs,  100,b);
                AddBlock(bs,  160,b);
                AddBlock(bs,  220,b);
            }
            else if (li == i++) { // Two rows; don't-tap bottom
                AddPlayer();
                AddBlock(bs, -220,b).SetDontTap();
                AddBlock(bs, -120,b).SetDontTap();
                AddBlock(bs,  -60,b).SetDontTap();
                AddBlock(bs,    0,b).SetDontTap();
                AddBlock(bs,   60,b).SetDontTap();
                AddBlock(bs,  120,b).SetDontTap();
                AddBlock(bs,  220,b).SetDontTap();
                AddBlock(bs, -220,b+200);
                AddBlock(bs, -120,b+200);
                AddBlock(bs,  -60,b+200);
                AddBlock(bs,    0,b+200);
                AddBlock(bs,   60,b+200);
                AddBlock(bs,  120,b+200);
                AddBlock(bs,  220,b+200);
            }
            else if (li == i++) { // Headless 3x3 with freckles
                AddPlayer();
                float g = 200;
                AddBlock(bs, -g,b    );
                AddBlock(bs,  0,b    );
                AddBlock(bs,  g,b    );
                AddBlock(bs, -g,b+g*1);
                AddBlock(bs,  0,b+g*1);
                AddBlock(bs,  g,b+g*1);

                AddBlock(bs, -g*0.5f,b+g*0.5f).SetDontTap();
                AddBlock(bs,  g*0.5f,b+g*0.5f).SetDontTap();
                AddBlock(bs, -g*0.5f,b+g*1.5f).SetDontTap();
                AddBlock(bs,  g*0.5f,b+g*1.5f).SetDontTap();
            }
            else if (li == i++) { // Two rows, scattered don't-taps
                AddPlayer();
                AddBlock(bs, -220,b).SetDontTap();
                AddBlock(bs, -120,b);
                AddBlock(bs,  -60,b).SetDontTap();
                //AddBlock(bs,    0,b);
                AddBlock(bs,   60,b).SetDontTap();
                AddBlock(bs,  120,b);
                AddBlock(bs,  220,b).SetDontTap();
                AddBlock(bs, -220,b+200);
                AddBlock(bs, -120,b+200).SetDontTap();
                AddBlock(bs,  -60,b+200);
                //AddBlock(bs,    0,b+200).SetDontTap();
                AddBlock(bs,   60,b+200);
                AddBlock(bs,  120,b+200).SetDontTap();
                AddBlock(bs,  220,b+200);
            }




            // Multi-Hit Blocks
            else if (li == i++) {
                AddBlock(bs,    0,b).SetHitsReq(3);
            }
            else if (li == i++) {
                AddBlock(bs, -80,b+120).SetHitsReq(3);
                AddBlock(bs,  80,b).SetHitsReq(3);
            }
            else if (li == i++) {
                AddBlock(bs, -140,b    ).SetHitsReq(2);
                AddBlock(bs,    0,b+100).SetHitsReq(2);
                AddBlock(bs,  140,b+200).SetHitsReq(2);
            }
            else if (li == i++) {
                AddBlock(bs, -140,b).SetHitsReq(1);
                AddBlock(bs,    0,b).SetHitsReq(2);
                AddBlock(bs,  140,b).SetHitsReq(3);
            }
            // TODO: These levels
            // Multi-Hit +
            //      varying y pos
            //      irregular layouts
            //      sizes
            //      traveling


            // Random Interlude
            else if (li == i++) { // Two 4x4 squares
                AddBlock(bs, -120,b+280);
                AddBlock(bs, -120,b+200);
                AddBlock(bs, -200,b+280);
                AddBlock(bs, -200,b+200);
                AddBlock(bs,    0,b+155).SetDontTap();
                AddBlock(bs,  120,b+ 80);
                AddBlock(bs,  120,b+  0);
                AddBlock(bs,  200,b+ 80);
                AddBlock(bs,  200,b+  0);
            }



            // Two-Ball Multi-Hit Don't-Taps
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -100,b+80).SetHitsReq(3);
                AddBlock(bs,  100,b+80).SetHitsReq(3);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, -120,b+100).SetHitsReq(3);
                AddBlock(bs,    0,b+100).SetHitsReq(3);
                AddBlock(bs,  120,b+100).SetHitsReq(3);
            }
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.9f;
                AddPlayer();
                AddBlock(bs, -240,b).SetDontTap().SetHitsReq(3);
                AddBlock(bs, -180,b).SetDontTap().SetHitsReq(3);
                //AddBlock(bs, -120,b).SetDontTap().SetHitsReq(2);
                AddBlock(new Vector2(70,70), -50,b).SetHitsReq(2);
                AddBlock(new Vector2(70,70),  50,b).SetHitsReq(2);
                //AddBlock(bs,  120,b).SetDontTap().SetHitsReq(2);
                AddBlock(bs,  180,b).SetDontTap().SetHitsReq(3);
                AddBlock(bs,  240,b).SetDontTap().SetHitsReq(3);
            }
            //else if (li == i++) {
            //    gameController.PlayerGravityScale = 0.9f;
            //    AddPlayer();
            //    AddPlayer();
            //    AddPlayer();
            //    AddPlayer();
            //    AddPlayer();
            //    AddBlock(bs, -240,b+160).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs, -180,b+160).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs, -120,b+160).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs, -240,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs, -180,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs, -120,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(new Vector2(140,140),    0,b+100).SetHitsReq(5);
            //    AddBlock(bs,  120,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs,  180,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs,  240,b+100).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs,  120,b+160).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs,  180,b+160).SetDontTap().SetHitsReq(4);
            //    AddBlock(bs,  240,b+160).SetDontTap().SetHitsReq(4);
            //}
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.8f;
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddBlock(bs, -240,b    ).SetDontTap().SetHitsReq(6);
                AddBlock(bs, -180,b    ).SetDontTap().SetHitsReq(6);
                AddBlock(bs, -120,b    ).SetDontTap().SetHitsReq(6);
                AddBlock(new Vector2(140,140),    0,b+100).SetHitsReq(8);
                AddBlock(bs,  120,b    ).SetDontTap().SetHitsReq(6);
                AddBlock(bs,  180,b    ).SetDontTap().SetHitsReq(6);
                AddBlock(bs,  240,b    ).SetDontTap().SetHitsReq(6);
            }
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.8f;
                AddPlayer();
                AddPlayer();
                AddPlayer();
                //AddBlock(bs, -240,b+120).SetDontTap().SetHitsReq(3);
                //AddBlock(bs, -180,b+120).SetDontTap().SetHitsReq(3);
                //AddBlock(bs, -120,b+120).SetDontTap().SetHitsReq(3);
                AddBlock(bs, -240,b+160).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -180,b+160).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -120,b+160).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -240,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -180,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -120,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(new Vector2(140,140),    0,b+100).SetHitsReq(4);
                AddBlock(bs,  120,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  180,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  240,b+100).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  120,b+160).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  180,b+160).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  240,b+160).SetDontTap().SetHitsReq(4);
                //AddBlock(bs,  120,b+120).SetDontTap().SetHitsReq(3);
                //AddBlock(bs,  180,b+120).SetDontTap().SetHitsReq(3);
                //AddBlock(bs,  240,b+120).SetDontTap().SetHitsReq(3);
            }
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.8f;
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddBlock(bs, -240,b+120).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -180,b+120).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -120,b+120).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -240,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -180,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -120,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -240,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -180,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(bs, -120,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(new Vector2(140,140),    0,b+80).SetHitsReq(6);
                AddBlock(bs,  120,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  180,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  240,b+  0).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  120,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  180,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  240,b+ 60).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  120,b+120).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  180,b+120).SetDontTap().SetHitsReq(4);
                AddBlock(bs,  240,b+120).SetDontTap().SetHitsReq(4);
            }




            // Two-Balls; Traveling Blocks
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2( 100,b), new Vector2( 160,b));
                AddBlock(bs, new Vector2(-140,b), new Vector2(-200,b));
                AddBlock(bs, new Vector2( 100,b+100), new Vector2(-100,b+100));
                AddBlock(bs,  200,b+140);
            }
            //else if (li == i++) {
            //    AddPlayer();
            //    AddBlock(bs, new Vector2(-120,b), new Vector2(-240,b));
            //    AddBlock(bs, new Vector2(-60,b), new Vector2(-120,b));
            //    AddBlock(bs, 0,b);
            //    AddBlock(bs, new Vector2( 120,b), new Vector2(240,b));
            //    AddBlock(bs, new Vector2( 60,b), new Vector2(120,b));
            //}
            //else if (li == i++) {
            //    AddPlayer();
            //    AddBlock(bs, new Vector2(-120,b), new Vector2(-240,b));
            //    AddBlock(bs, new Vector2(-60,b), new Vector2(-120,b));
            //    AddBlock(bs, 0,b);
            //    AddBlock(bs, new Vector2(-200,b+250), new Vector2(200,b+250));
            //    AddBlock(bs, new Vector2( 240,b), new Vector2(120,b));
            //    AddBlock(bs, new Vector2( 120,b), new Vector2(60,b));
            //}
            // Faster Traveling Blocks
            else if (li == i++) {
                AddPlayer();
                float w = 120;
                AddBlock(bs, new Vector2(-240,b), new Vector2(-240+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(-180,b), new Vector2(-180+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(-120,b), new Vector2(-120+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2( -60,b), new Vector2( -60+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(   0,b), new Vector2(   0+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2(  60,b), new Vector2(  60+w,b)).SetSpeed(1.4f);
                AddBlock(bs, new Vector2( 120,b), new Vector2( 120+w,b)).SetSpeed(1.4f);
            }
            //else if (li == i++) {
            //    AddPlayer();
            //    float w = 160;
            //    AddBlock(bs, -260,b+240);
            //    AddBlock(bs, new Vector2(-200,b+160), new Vector2(-200+w,b+160)).SetSpeed(2f);
            //    AddBlock(bs, new Vector2(-140,b+ 80), new Vector2(-140+w,b+ 80)).SetSpeed(2f);
            //    AddBlock(bs, new Vector2( -80,b    ), new Vector2( -80+w,b    )).SetSpeed(2f);
            //    AddBlock(bs, new Vector2( -20,b+ 80), new Vector2( -20+w,b+ 80)).SetSpeed(2f);
            //    AddBlock(bs, new Vector2(  40,b+160), new Vector2(  40+w,b+160)).SetSpeed(2f);
            //    AddBlock(bs,  260,b+240);
            //}

            // Varying-Speed Traveling Blocks
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2(-150,b), new Vector2(-240,b)).SetSpeed(2f);
                AddBlock(bs, new Vector2(-100,b+50), new Vector2(100,b+50)).SetSpeed(1f);
                AddBlock(bs, new Vector2( 150,b), new Vector2(240,b)).SetSpeed(2f);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2(-240,b+100), new Vector2(-150,b+100)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2(-120,b), new Vector2(40,b)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2( -40,b+200), new Vector2(120,b+200)).SetSpeed(2.5f);
                AddBlock(bs, new Vector2( 150,b+100), new Vector2(240,b+100)).SetSpeed(2.5f);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2( 160,b), new Vector2(-160,b)).SetSpeed(3f);
                AddBlock(bs, new Vector2(-150,b+200), new Vector2(-240,b+200)).SetSpeed(4f);
                AddBlock(bs, new Vector2(-160,b), new Vector2( 160,b)).SetSpeed(3f);
                AddBlock(bs, new Vector2( 150,b+100), new Vector2(240,b+100)).SetSpeed(4f);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2(-240,b    ), new Vector2( -80,b    )).SetSpeed(2f);
                AddBlock(bs, new Vector2( 240,b    ), new Vector2(  80,b    )).SetSpeed(2f);
                AddBlock(bs, new Vector2( -80,b+160), new Vector2(-240,b+160)).SetSpeed(2f);
                AddBlock(bs, new Vector2(  80,b+160), new Vector2( 240,b+160)).SetSpeed(2f);
                AddBlock(bs, new Vector2(-240,b+320), new Vector2( -80,b+320)).SetSpeed(2f);
                AddBlock(bs, new Vector2( 240,b+320), new Vector2(  80,b+320)).SetSpeed(2f);
            }
            else if (li == i++) {
                AddPlayer();
                AddBlock(bs, new Vector2(-200,b    ), new Vector2( 200,b    )).SetSpeed(1.7f);
                AddBlock(bs, new Vector2(-200,b+100), new Vector2( 200,b+100)).SetSpeed(1.7f, 3.142f);
                AddBlock(bs, new Vector2(-200,b+200), new Vector2( 200,b+200)).SetSpeed(1.7f);
                AddBlock(bs, new Vector2(-200,b+300), new Vector2( 200,b+300)).SetSpeed(1.7f, 3.142f);
            }


            // TONS of Balls!
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.5f;
                AddPlayer();
                AddPlayer();
                AddBlock(bs, -120,b+100).SetHitsReq(2);
                AddBlock(bs,    0,b+100).SetHitsReq(2);
                AddBlock(bs,  120,b+100).SetHitsReq(2);
            }
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.5f;
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddPlayer();
                AddBlock(bs, -150,b);
                AddBlock(bs,  -90,b);
                AddBlock(bs,  -30,b);
                AddBlock(bs,   30,b);
                AddBlock(bs,   90,b);
                AddBlock(bs,  150,b);
            }
            else if (li == i++) {
                gameController.PlayerGravityScale = 0.5f;
                AddPlayer();
                AddPlayer();
                AddPlayer();
                //AddPlayer();
                //AddPlayer();
                AddBlock(bs, new Vector2(-240,b    ), new Vector2( -80,b    )).SetSpeed(2f);
                AddBlock(bs, new Vector2( 240,b    ), new Vector2(  80,b    )).SetSpeed(2f);
                AddBlock(bs, new Vector2( -80,b+160), new Vector2(-240,b+160)).SetSpeed(2f);
                AddBlock(bs, new Vector2(  80,b+160), new Vector2( 240,b+160)).SetSpeed(2f);
                //AddBlock(bs, new Vector2(-240,b+320), new Vector2( -80,b+320)).SetSpeed(2f);
                //AddBlock(bs, new Vector2( 240,b+320), new Vector2(  80,b+320)).SetSpeed(2f);
            }




            // Gravity-Flip Blocks
            //else if (levelIndex == i++) {
            //    AddBlock(blockSize,    0,b).SetGravityFlip(1);
            //}




            /*
            // Even grids for level-making reference
                else if (levelIndex == i++) {
                    AddBlock(bs, -120,b);
                    AddBlock(bs,  -60,b);
                    AddBlock(bs,    0,b);
                    AddBlock(bs,   60,b);
                    AddBlock(bs,  120,b);
                }
                else if (levelIndex == i++) {
                    AddBlock(bs, -150,b);
                    AddBlock(bs,  -90,b);
                    AddBlock(bs,  -30,b);
                    AddBlock(bs,   30,b);
                    AddBlock(bs,   90,b);
                    AddBlock(bs,  150,b);
                }

            // TEST LEVELS
                else if (levelIndex == i++) {
                    AddBlock(bs, new Vector2(0,b), new Vector2(100,b));
                    AddBlock(bs, new Vector2(100,b), new Vector2(0,b));
                    AddBlock(bs, new Vector2(-100,b), new Vector2(100,b));
                    AddBlock(bs,  -200,b);
                }
                else if (levelIndex == i++) {
                    AddBlock(bs, 0,b).SetDontTap();
                }
                else if (levelIndex == i++) {
                    AddBlock(bs, -30,b).SetDontTap();
                    AddBlock(bs,  30,b);
                }
                else if (levelIndex == i++) {
                    AddBlock(bs, 0,b).SetHitsReq(2);
                }
            */
                else {
                    AddBlock(new Vector2(200,200), 0,b);
                    Debug.LogWarning("No level data available for level: " + li);
                }
        }

    }
}