using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	public class Level : BaseLevel {
		// Constants
		public static int FirstLevelIndex = 1;
//		public static int LastLevelIndex = 101;
		// Components
        [SerializeField] private LevelBounds bounds=null;
        [SerializeField] private LevelUI levelUI=null;
        [SerializeField] private RectTransform rt_gameComponents=null; // Growers and Walls go on this!
		private List<Grower> growers;
		private List<Wall> walls;
		private List<Image> i_collisionIcons;
		// Properties
//		private float screenShakeVolume;
        private int scoreRequired;
		private int currentGrowerIndex;
		// References
		private GameController gameController;


        // Getters (Public)
		public bool IsGameStatePlaying { get { return gameController.IsGameStatePlaying; } }
        public int ScoreRequired { get { return scoreRequired; } }
        public List<Grower> Growers { get { return growers; } }
		public RectTransform rt_GameComponents { get { return rt_gameComponents; } }
		// Getters (Private)
		private Grower currentGrower {
			get {
				if (currentGrowerIndex<0 || currentGrowerIndex>=growers.Count) { return null; } // Index outta bounds? Return null.
				return growers[currentGrowerIndex];
			}
		}



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(GameController _gameController, Transform tf_parent, int _levelIndex) {
			gameController = _gameController;

            bounds.Initialize();
			BaseInitialize(tf_parent, _levelIndex);
            levelUI.Initialize();
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScoreUI(int scorePossible, int scoreSolidified) {
            levelUI.UpdateScoreUI(scorePossible, scoreSolidified);
		}
		private void AddIllegalOverlapIcon(Vector2 pos) {
			Image iconImage = Instantiate(resourcesHandler.circleGrow_collisionIcon).GetComponent<Image>();
			GameUtils.ParentAndReset(iconImage.gameObject, rt_gameComponents);
			iconImage.rectTransform.anchoredPosition = pos;
			i_collisionIcons.Add(iconImage);
		}


        // ----------------------------------------------------------------
        //  Game Doers
		// ----------------------------------------------------------------
		private void SetCurrentGrowerIndex(int _index) {
            if (growers.Count == 0) { return; } // Safety check.
            currentGrowerIndex = _index;

			gameController.UpdateScore();

			// There IS another Grower!
			if (currentGrower != null) {
				currentGrower.SetAsPreGrowing();
			}
            // There is NOT another Grower! End the level.
			else {
                gameController.OnAllGrowersSolidified();
			}
		}
		private void StartGrowingCurrentGrower() {
			if (currentGrower != null) {
				currentGrower.StartGrowing();
			}
		}
		private void SolidifyCurrentGrower() {
			if (currentGrower == null) { return; } // Safety check.
			if (currentGrower.CurrentState != GrowerStates.Growing) { return; } // Oh, if it's only pre-growing, DON'T do anything.

			// Solidify current, and move onto the next one!
			currentGrower.Solidify();
			SetCurrentGrowerIndex(currentGrowerIndex + 1);
		}



		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnLoseLevel(LoseReasons reason) {
			levelUI.OnLoseLevel(reason);
		}
		public void OnWinLevel() {
			levelUI.OnWinLevel();
		}

		public void OnTapScreen() {
			if (currentGrower == null) { return; } // No grower? Do nothin'.
			// PreGrowing -> Growing!
			if (currentGrower.CurrentState == GrowerStates.PreGrowing) {
				StartGrowingCurrentGrower();
			}
			// Growing -> Solidified!
			else if (currentGrower.CurrentState == GrowerStates.Growing) {
				SolidifyCurrentGrower();
			}
		}
		public void OnIllegalOverlap(Vector2 pos) {
			AddIllegalOverlapIcon(pos); // Add the no-no icon!
			gameController.OnIllegalOverlap(); // Tattle to GameController!!
		}
		/** Growers are responsible for telling me when they've changed their size, so we can update the score. */
		public void OnGrowerGrowStep() {
			gameController.UpdateScore();
		}




		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
//		private void Update () {
//			if (Time.timeScale == 0) { return; } // No time? Do nothin'.
//
//			if (IsGameStatePlaying) {
//				GrowCurrentGrower();
//			}
//		}
//        private void GrowCurrentGrower() {
//			if (currentGrower == null) { return; } // Safety check.
//
//            // Grow it, and update our score!
//            if (currentGrower.CurrentState == GrowerStates.Growing) {
//                currentGrower.GrowStep();
//                gameController.UpdateScore();
//            }
//
//   //         if (IsIllegalOverlap(currentGrower)) {
//			//	OnIllegalOverlap(currentGrower);
//			//}
//		}


		// ----------------------------------------------------------------
		//  Destroying Elements
		// ----------------------------------------------------------------
        private void DestroyLevelComponents() {
            if (growers != null) {
                for (int i=growers.Count-1; i>=0; --i) {
                    Destroy(growers[i].gameObject);
                }
            }
			growers = new List<Grower>();
			if (walls != null) {
				for (int i=walls.Count-1; i>=0; --i) {
					Destroy(walls[i].gameObject);
				}
			}
			walls = new List<Wall>();
			if (i_collisionIcons != null) {
				for (int i=i_collisionIcons.Count-1; i>=0; --i) {
					Destroy(i_collisionIcons[i].gameObject);
				}
			}
			i_collisionIcons = new List<Image>();
		}
		// ----------------------------------------------------------------
		//  Adding Elements
        // ----------------------------------------------------------------
        //private Grower AddGrower(PropShapes shape, float x,float y) {
        private Grower AddGrower(GrowerData data) {
			GameObject prefabGO = null;
            switch (data.shape) {
			case PropShapes.Circle: prefabGO = resourcesHandler.circleGrow_growerCircle; break;
			case PropShapes.Rect: prefabGO = resourcesHandler.circleGrow_growerRect; break;
            default: Debug.LogError("Grower shape not yet supported. Time to write some more code! Shape: " + data.shape); break;
			}
			Grower newObj = Instantiate(prefabGO).GetComponent<Grower>();
            newObj.Initialize(this, rt_gameComponents, data);
            growers.Add(newObj);
            return newObj;
		}
        /*
		private Wall AddWall(float x,float y, float diameter) {
			return AddWall(PropShapes.Circle, x,y, diameter,diameter);
		}
		private Wall AddWall(float x,float y, float w,float h) {
			return AddWall(PropShapes.Rect, x,y, w,h);
		}
		private Wall AddWall(PropShapes shape, float x,float y, float w,float h) {
		*/
        private Wall AddWall(WallData data) {
			GameObject prefabGO = null;
            switch (data.shape) {
			case PropShapes.Circle: prefabGO = resourcesHandler.circleGrow_wallCircle; break;
			case PropShapes.Rect: prefabGO = resourcesHandler.circleGrow_wallRect; break;
			default: Debug.LogError("Wall shape not yet supported. Time to write some more code! Shape: " + data.shape); break;
			}
			Wall newObj = Instantiate(prefabGO).GetComponent<Wall>();
            newObj.Initialize(this, rt_gameComponents, data);//new Vector2(x,y), new Vector2(w,h)
            walls.Add(newObj);
            return newObj;
        }



		// ----------------------------------------------------------------
		//  Making Level!
		// ----------------------------------------------------------------
		override protected void AddLevelComponents() {
			DestroyLevelComponents(); // Just in case.
			if (resourcesHandler == null) { return; } // Safety check for runtime compile.

            // Specify default values
			//PropShapes gs = PropShapes.Circle; // default growerShape!
            scoreRequired = 1000;
			bounds.SetSize(550,750); // Default to 600x800 with 25px padding on all sides.

            string levelString = gameController.LevelLoader.GetLevelString(LevelIndex);
            if (!string.IsNullOrEmpty(levelString)) {
                MakeLevelFromString(levelString);
            }
            else {
                DestroyLevelComponents();
                levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
                Debug.LogWarning("No level data available for level: " + LevelIndex);
            }

            // Start growing the first dude!
            SetCurrentGrowerIndex(0);
        }
        private void MakeLevelFromString(string _str) {
            try {
                string[] lines = TextUtils.GetStringArrayFromStringWithLineBreaks(_str);
                foreach (string s in lines) {
                    if (s.StartsWith("scoreReq")) {
                        SetScoreRequiredFromString(s);
                    }
                    else if (s.StartsWith("grower")) {
                        AddGrowerFromString(s);
                    }
                    else if (s.StartsWith("wall")) {
                        AddWallFromString(s);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError("Error reading level string! LevelIndex: " + LevelIndex + ". Error: " + e);
            }
        }
        private void SetScoreRequiredFromString(string s) {
            scoreRequired = TextUtils.ParseInt(s.Substring(s.IndexOf('=')+1));
        }
        private void AddGrowerFromString(string fullLine) {
            GrowerData data = new GrowerData();
            data.shape = GetGrowerShapeFromLine(fullLine);
            fullLine = fullLine.Substring(fullLine.IndexOf(" ")+1); // remove "growerX ".
            string[] properties = fullLine.Split(';');
            // Prop Properties!
            data.pos = TextUtils.GetVector2FromStringNoParens(properties[0]);
            SetPropDataProperties(data, properties, 1); // Note: Skip the first property, which must always be pos.
            // Grower-specific Properties
            for (int i=1; i<properties.Length; i++) {
                string s = properties[i].TrimStart();
                if (s.StartsWith("growSpeed=")) {
                    data.growSpeed = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                }
            }
            // Add the dude!
            Grower newObj = AddGrower(data);
        }
        private void AddWallFromString(string fullLine) {
            WallData data = new WallData();
            data.shape = GetWallShapeFromLine(fullLine);
            fullLine = fullLine.Substring(fullLine.IndexOf(" ")+1); // remove "wallX ".
            string[] properties = fullLine.Split(';');
            // Prop Properties!
            data.pos = TextUtils.GetVector2FromStringNoParens(properties[0]);
            data.size = GetWallSizeFromString(properties[1]);
            SetPropDataProperties(data, properties, 2); // Note: Skip the first TWO properties, which are pos and size.
            // Add the dude!
            Wall newObj = AddWall(data);//.shape, data.pos.x,data.pos.y, data.size.x,data.size.y);
        }
        /// The startIndex makes this confusing, so ignore it (it's for optimization). We use this function to convert string properties from Levels.txt to PropData properties. :)
        private void SetPropDataProperties(PropData data, string[] properties, int startIndex) {
            for (int i=startIndex; i<properties.Length; i++) {
                string s = properties[i].TrimStart();
                if (s.StartsWith("posB=")) {
                    data.posB = TextUtils.GetVector2FromStringNoParens(s.Substring(s.IndexOf('=')+1));
                }
                else if (s.StartsWith("moveSpeed=")) {
                    data.moveSpeed = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                }
                else if (s.StartsWith("moveLocOffset=")) {
                    data.moveLocOffset = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                }
                else if (s.StartsWith("size=")) {
                    data.size = TextUtils.GetVector2FromStringNoParens(s.Substring(s.IndexOf('=')+1));
                }
                else if (s.StartsWith("rotation=")) {
                    data.rotation = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                }
                else if (s.StartsWith("rotateSpeed=")) {
                    data.rotateSpeed = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                }
            }
        }
        /*
        private void AddWallFromString(string fullLine) {
            fullLine = fullLine.Substring(fullLine.IndexOf(" ")+1); // remove "wall ".
            string[] properties = fullLine.Split(';');
            // Pos
            Vector2 pos = TextUtils.GetVector2FromStringNoParens(properties[0]);
            WallData newData = WallData(pos);
            // Properties!
            for (int i=1; i<properties.Length; i++) { // Note: Skip the first property (which must always be pos).
                string s = properties[i].TrimStart();
                if (s.StartsWith("growSpeed=")) {
                    float val = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                    newObj.SetGrowSpeed(val);
                }
                else if (s.StartsWith("posB=")) {
                    Vector2 val = TextUtils.GetVector2FromStringNoParens(s.Substring(s.IndexOf('=')+1));
                    newObj.SetPosB(val);
                }
                else if (s.StartsWith("moveSpeed=")) {
                    float val = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                    newObj.SetMoveSpeed(val);
                }
                else if (s.StartsWith("size=")) {
                    float val = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                    newObj.SetSize(val);
                }
                else if (s.StartsWith("rotation=")) {
                    float val = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                    newObj.SetRotation(val);
                }
                else if (s.StartsWith("rotateSpeed=")) {
                    float val = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
                    newObj.SetRotateSpeed(val);
                }
            }
        }
        */

        private PropShapes GetGrowerShapeFromLine(string s) {
            if (s.StartsWith("growerC")) { return PropShapes.Circle; }
            else if (s.StartsWith("growerR")) { return PropShapes.Rect; }
            Debug.LogError("Grower shape not recognized or specified! Line: \"" + s + "\".");
            return PropShapes.Circle;
        }
        private PropShapes GetWallShapeFromLine(string s) {
            if (s.StartsWith("wallC")) { return PropShapes.Circle; }
            else if (s.StartsWith("wallR")) { return PropShapes.Rect; }
            Debug.LogError("Wall shape not recognized or specified! Line: \"" + s + "\".");
            return PropShapes.Circle;
        }
        /// Complicated here, for easier text-file editing. We can specify JUST the diameter ("50"), or give both w/h dimensions ("50,50"). This will accept/interpret either. :)
        private Vector2 GetWallSizeFromString(string s) {
            string[] split = s.Split(',');
            if (split.Length == 1) { // We provided ONE value, the radius. Interpret it from "50" to "50,50"!
                return new Vector2(TextUtils.ParseFloat(split[0]), TextUtils.ParseFloat(split[0]));//*2
            }
            // We provided TWO values, width and height. Interpret it as is!
            return new Vector2(TextUtils.ParseFloat(split[0]), TextUtils.ParseFloat(split[1]));
        }


        /*
			float l=-550*0.5f;
			float r= 550*0.5f;
			float b=-750*0.5f;
			float t= 750*0.5f;

			// NOTE: All coordinates are based off of LevelBounds's playing space (specified here! 550x750 or less)!

			int li = LevelIndex;
			int i=FirstLevelIndex;
			if (false) {}


            // Balls to the Walls
            else if (li == i++) { // One.
                scoreRequired = 2000;
                AddGrower(gs, 0,0);
			}
//			else if (li == i++) { // One, with high fence
//				scoreRequired = 300;
//				AddWall(0,-250, 600,600);
//				AddGrower(gs, 0, 160).SetGrowSpeed(0.7f);
			//			}
//			else if (li == i++) { // One, with single WallCircle (vertical)
//				scoreRequired = 900;
//				AddGrower(gs, 0, 130);
//				AddWall(0,-100, 100);
//			}
			else if (li == i++) { // One, with single WallCircle (horizontal)
				scoreRequired = 700;
				AddGrower(gs, -30,30).SetGrowSpeed(0.8f);
				AddWall(172,30, 92);
			}
//			else if (li == i++) { // Two, opposite sides MAYBE use
//				scoreRequired = 800;
//				AddGrower(gs,   0,t-100);
//				AddGrower(gs, 120, -120);
//			}
//			else if (li == i++) { // Two, with center diamond wall
//				scoreRequired = 1200;
//				AddGrower(gs,  -167,282);
//				AddGrower(gs,  -42,-181);
//				AddWall(-45,145, 200,200).SetRotation(45);
	//			}
			else if (li == i++) { // One, with compressing walls!
				scoreRequired = 1200;
				AddGrower(gs,  0,0).SetGrowSpeed(1.2f);
				AddWall(-230,0, 400,800).SetPosB(-450,0).SetMoveSpeed(0.8f,-Mathf.PI*0.5f);
				AddWall( 230,0, 400,800).SetPosB( 450,0).SetMoveSpeed(0.8f,-Mathf.PI*0.5f);
			}
			else if (li == i++) { // Two, with center WallCircle
				scoreRequired = 900;
				AddGrower(gs,  0, -184).SetGrowSpeed(0.9f);
				AddGrower(gs,  0,  150).SetGrowSpeed(0.9f);
				AddWall(0,13, 92);
			}
			else if (li == i++) { // Two, in a field of landmines!
				scoreRequired = 900;
				AddGrower(gs, -100,-160);
				AddGrower(gs,   56, 132);
				AddWall(-185, 265, 150);
				AddWall(-175,  21,  190);
				AddWall(  54, 333,  82);
				AddWall( 169,-108, 206);
				AddWall(-225,-250, 100);
				AddWall( 216, 316, 166);
				AddWall(  59,-277, 192);
			}
			else if (li == i++) { // One, with rotating center plank
				scoreRequired = 800;
				AddGrower(gs,  0, 100);
				AddWall(0,-100, 30,300).SetPosB(0,-220).SetMoveSpeed(2.5f).SetRotateSpeed(1f);
			}
			else if (li == i++) { // Three, with center diamond wall
				scoreRequired = 900;
				AddGrower(gs, -130, -130);
				AddGrower(gs,  150, -250);
				AddGrower(gs,    0,  258);
				AddWall(0,0, 200,200).SetRotation(45);
			}

			else if (li == i++) { // One, concave narrow squish
				scoreRequired = 650;
				AddWall(-500,0, 700);
				AddWall( 500,0, 700);
				AddGrower(gs, 0, 0);
			}
//            else if (li == i++) { // Two in chambers
//                scoreRequired = 1600;
//                AddWall(0,100, 600,100);
//                AddGrower(gs, 0, 214+50);
//                AddGrower(gs, 0,-214+50);
//            }
            else if (li == i++) { // Biig rotating squares you gotta wait for
                scoreRequired = 850;
                AddGrower(gs, -220, 0);
                AddGrower(gs,  220, 0);
                AddGrower(gs, 0, 0);
                AddWall(0,-520, 700,700).SetRotateSpeed( 0.2f);
                AddWall(0, 520, 700,700).SetRotateSpeed( 0.2f);
            }
            //else if (li == i++) { // 2 alone.
            //    scoreRequired = 800;
            //    AddGrower(gs, -120,0);
            //    AddGrower(gs,  120,0);
            //}
			else if (li == i++) { // One moving Grower
				scoreRequired = 1400;
				AddGrower(gs, 0,-100).SetPosB(0,330).SetMoveSpeed(0.8f);
			}
//            else if (li == i++) { // 4 in corners. Squidward-Robot.
//				scoreRequired = 1500;
////				AddWall(0,0, 600,50);
////				AddWall(0,0, 50,800);
//				AddWall(0,t, 50,t*2);
////				AddWall(l*0.5, -l,50);
//                AddGrower(gs,  150,  200);
//                AddGrower(gs, -150, -200);
//                AddGrower(gs,  150, -200);
//                AddGrower(gs, -150,  200);
//            }


			else if (li == i++) { // Triplex moving
				scoreRequired = 1000;
				AddWall(0,-140, 600,25);
				AddWall(0, 140, 600,25);
				AddGrower(gs, -150,  262).SetPosB(150, 262).SetMoveSpeed(1, 0);
				AddGrower(gs, -150, -262).SetPosB(150,-262).SetMoveSpeed(1, Mathf.PI);
				AddGrower(gs, -150,    0).SetPosB(150,   0).SetMoveSpeed(1, Mathf.PI*0.5f);
			}
            else if (li == i++) { // 2 diagonal.
                scoreRequired = 1850;
                AddGrower(gs, -60,-160);
                AddGrower(gs,  60, 160);
			}
            else if (li == i++) { // Easy V
                scoreRequired = 2700;
                AddWall(-275,-375, 250).SetPosB(-350,-450).SetMoveSpeed(1.2f);
                AddWall( 275,-375, 250).SetPosB( 350,-450).SetMoveSpeed(1.2f);
                AddWall(-275, 375,  80).SetPosB(-350, 450).SetMoveSpeed(1.2f, Mathf.PI);
                AddWall( 275, 375,  80).SetPosB( 350, 450).SetMoveSpeed(1.2f, Mathf.PI);
                AddGrower(gs, -140,  240).SetGrowSpeed(1.2f);
                AddGrower(gs,  140,  240).SetGrowSpeed(1.2f);
                AddGrower(gs,    0, -140).SetGrowSpeed(1.2f);
			}
            else if (li == i++) { // Snowman
                scoreRequired = 1500;
                AddGrower(gs, 0, -200);
                AddGrower(gs, 0,  100);
                AddGrower(gs, 0,  300);
			}



            else if (li == i++) { // One patrolling Wall
                scoreRequired = 1300;
                AddGrower(gs, -120,  220).SetGrowSpeed(1.2f);
                AddGrower(gs,  120, -220).SetGrowSpeed(1.2f);
                AddWall(0,250, 60).SetPosB(0,-250).SetMoveSpeed(0.2f);
            }

			//else if (li == i++) { // Moving Grower Limbo-esque activation (start early)
			//	scoreRequired = 2100;
			//	AddGrower(gs, 0,-100).SetPosB(0,300).SetMoveSpeed(0.8f);
			//}


			else if (li == i++) { // 3 diagonal
				scoreRequired = 1800;
				AddGrower(gs, -100, 200).SetGrowSpeed(1.2f);
				AddGrower(gs,  100,-200).SetGrowSpeed(1.2f);
				AddGrower(gs,    0,   0).SetGrowSpeed(1.2f);
			}
			else if (li == i++) { // 3 diagonal OOO
				scoreRequired = 1800;
				AddGrower(gs,    0,   0).SetGrowSpeed(1.2f);
				AddGrower(gs, -100, 200).SetGrowSpeed(1.2f);
				AddGrower(gs,  100,-200).SetGrowSpeed(1.2f);
			}
			else if (li == i++) { // 3 V OOO
				scoreRequired = 2000;
				AddGrower(gs, -100, 200);
				AddGrower(gs,  100, 200);
				AddGrower(gs,    0,-100);
			}

            else if (li == i++) { // 3 square Wall gears! Flashy.
                scoreRequired = 600;
                AddGrower(gs, -150, 250).SetGrowSpeed(0.7f);
                AddGrower(gs,  150,-250).SetGrowSpeed(0.7f);
                AddWall(   0,   0, 210,210).SetRotateSpeed(-0.9f);
                AddWall(-110,-210, 210,210).SetRotateSpeed( 0.9f);
                AddWall( 110, 210, 210,210).SetRotateSpeed( 0.9f);
            }


            //else if (li == i++) { // One moving, one static
            //    scoreRequired = 1600;
            //    AddGrower(gs, 0, 50).SetPosB(0,-200).SetMoveSpeed(0.9f);
            //    AddGrower(gs, 0, 200);
            //}
            else if (li == i++) { // One moving, one static reversed
                scoreRequired = 1350;
                AddGrower(gs, 0, 200);
                AddGrower(gs, 0, 50).SetPosB(0,-200).SetMoveSpeed(0.8f);
            }


            //else if (li == i++) { // TODO: This Lots of moving stuff! Gotta wait for the right moment. Relatively easy.
            //    scoreRequired = 1600;
            //    AddGrower(gs, 0,-200);
            //    AddGrower(gs, 0, 200);
            //    //AddWall(
            //}

            else if (li == i++) { // Surprise wall squish!
                scoreRequired = 2000;
                AddGrower(gs, 0, 0).SetGrowSpeed(2f);
                //AddWall(-140,0, 80,800).SetPosB(-2000,0).SetMoveSpeed(0.8f, Mathf.PI);
                //AddWall( 140,0, 80,800).SetPosB( 2000,0).SetMoveSpeed(0.8f, Mathf.PI);
                AddWall(0,-300, 600,500).SetPosB(0,-4000).SetMoveSpeed(0.7f, 0.15f);
                AddWall(0, 300, 600,500).SetPosB(0, 4000).SetMoveSpeed(0.7f, 0.15f);
            }
            else if (li == i++) { // Three-wall squish
                scoreRequired = 1400;
                AddGrower(gs, 0,-250).SetGrowSpeed(1.5f);
                AddGrower(gs, 0,   0).SetGrowSpeed(1.5f);
                AddGrower(gs, 0, 250).SetGrowSpeed(1.5f);
                AddWall(-230,-280, 400,280).SetPosB(-450,-280).SetMoveSpeed(0.28f,    0f);
                AddWall( 230,-280, 400,280).SetPosB( 450,-280).SetMoveSpeed(0.28f,    0f);
                AddWall(-230,   0, 400,280).SetPosB(-450,   0).SetMoveSpeed(0.28f, -1.2f);
                AddWall( 230,   0, 400,280).SetPosB( 450,   0).SetMoveSpeed(0.28f, -1.2f);
                AddWall(-230, 280, 400,280).SetPosB(-450, 280).SetMoveSpeed(0.28f, -2.4f);
                AddWall( 230, 280, 400,280).SetPosB( 450, 280).SetMoveSpeed(0.28f, -2.4f);
            }



//			else if (li == i++) { // 3 moving in sync. TODO: This
//				scoreRequired = 100;
//				AddGrower(gs, -60,-160);
//			}



            else if (li == i++) { // You're squares!
                scoreRequired = 1600;
                AddGrower(PropShapes.Rect,    0, 200).SetSize(20,10);
                AddGrower(PropShapes.Rect, -130,-230);
                AddGrower(PropShapes.Rect,  130,-230);
            }
            else if (li == i++) { // Random, rotated squares
                scoreRequired = 1600;
                AddGrower(gs, 0,-200);
                AddGrower(gs, 0, 200);
            }
            else if (li == i++) { // Three-wall wave offset!
                scoreRequired = 1200;
                AddGrower(PropShapes.Rect, 0,-250).SetGrowSpeed(1.5f).SetRotateSpeed(1f);
                AddGrower(PropShapes.Rect, 0,   0).SetGrowSpeed(1.5f).SetRotateSpeed(1f);
                AddGrower(PropShapes.Rect, 0, 250).SetGrowSpeed(1.5f).SetRotateSpeed(1f);
                AddWall(-320,-280, 400,280).SetPosB(-400,-280).SetMoveSpeed(1f,    0f);
                AddWall( 320,-280, 400,280).SetPosB( 400,-280).SetMoveSpeed(1f,    0f+Mathf.PI);
                AddWall(-320,   0, 400,280).SetPosB(-400,   0).SetMoveSpeed(1f, -1.2f);
                AddWall( 320,   0, 400,280).SetPosB( 400,   0).SetMoveSpeed(1f, -1.2f+Mathf.PI);
                AddWall(-320, 280, 400,280).SetPosB(-400, 280).SetMoveSpeed(1f, -2.4f);
                AddWall( 320, 280, 400,280).SetPosB( 400, 280).SetMoveSpeed(1f, -2.4f+Mathf.PI);
            }
            // TODO: You're squares AND circles

            else if (li == i++) { // One slowly rotating GrowerSquare. Gotta time it right not to hit sides.
                scoreRequired = 2400;
                AddGrower(PropShapes.Rect, 0,0).SetSize(40,40).SetRotateSpeed(0.2f);
            }
            else if (li == i++) { // GrowerSquare fitting in with WallSquares
                scoreRequired = 600;
                AddGrower(PropShapes.Rect, 0,0).SetRotateSpeed(0.3f);
                AddWall(    0,-200, 120,120).SetRotateSpeed(-0.3f);
                AddWall(    0, 200, 120,120).SetRotateSpeed(-0.3f);
            }
			else if (li == i++) { // 2 close call
				scoreRequired = 1500;
				AddGrower(gs, 0,  200);
                AddGrower(gs, 0, -112);
                AddWall(-138, 22, 100);//.SetPosB(-200,22).SetMoveSpeed(0.9f);
                AddWall( 138, 22, 100);//.SetPosB( 200,22).SetMoveSpeed(0.9f);
            }
            else if (li == i++) { // 2 in-n-out.
                scoreRequired = 1400;
                AddGrower(gs, 0, 100).SetGrowSpeed(1.2f).SetPosB(0, 250).SetMoveSpeed(1.2f);
                AddGrower(gs, 0,-100).SetGrowSpeed(1.2f).SetPosB(0,-250).SetMoveSpeed(1.2f);
            }
            else if (li == i++) { // Perfect square fit
                scoreRequired = 1800;
                bounds.SetSize(500,500);
                AddGrower(PropShapes.Rect, 0,0).SetGrowSpeed(2f).SetRotateSpeed(0.6f);
            }
            else if (li == i++) { // Fast 4 square
                scoreRequired = 2080;
                float rd = 140;
                float m = 70f;
                AddGrower(gs, -130, -130).SetGrowSpeed(3f);
                AddGrower(gs,  130,  130).SetGrowSpeed(3f);
                AddGrower(gs,  130, -130).SetGrowSpeed(3f);
                AddGrower(gs, -130,  130).SetGrowSpeed(3f);
                AddWall(-275,-375, rd*2);
                AddWall(-275, 375, rd*2);
                AddWall( 275,-375, rd*2);
                AddWall( 275, 375, rd*2);
            }

            else if (li == i++) { // Four-square Foursquare
                scoreRequired = 2000;
                AddGrower(PropShapes.Rect, -134,-134).SetGrowSpeed(1.8f).SetRotateSpeed(0.5f);
                AddGrower(PropShapes.Rect, -134, 134).SetGrowSpeed(1.8f).SetRotateSpeed(0.5f);
                AddGrower(PropShapes.Rect,  134,-134).SetGrowSpeed(1.8f).SetRotateSpeed(0.5f);
                AddGrower(PropShapes.Rect,  134, 134).SetGrowSpeed(1.8f).SetRotateSpeed(0.5f);
            }


            //else if (li == i++) { // 4 square
            //    scoreRequired = 2100;
            //    float rd = 140;
            //    float m = 70f;
            //    AddGrower(gs, -130, -130);//.SetPosB(-130,-130+m).SetMoveSpeed(1f);
            //    AddGrower(gs,  130,  130);//.SetPosB( 130, 130+m).SetMoveSpeed(1f, Mathf.PI);
            //    AddGrower(gs,  130, -130);//.SetPosB( 130,-130+m).SetMoveSpeed(1f, Mathf.PI);
            //    AddGrower(gs, -130,  130);//.SetPosB(-130, 130+m).SetMoveSpeed(1f);
            //    AddWall(-275,-375, rd*2);//.SetPosB(-275,-375+m).SetMoveSpeed(1f);
            //    AddWall(-275, 375, rd*2);//.SetPosB(-275, 375+m).SetMoveSpeed(1f);
            //    AddWall( 275,-375, rd*2);//.SetPosB( 275,-375+m).SetMoveSpeed(1f, Mathf.PI);
            //    AddWall( 275, 375, rd*2);//.SetPosB( 275, 375+m).SetMoveSpeed(1f, Mathf.PI);
            //}

            else if (li == i++) { // Two frozen guys
                scoreRequired = 1350;
                float m = 120;
                AddGrower(gs, -100, -130).SetGrowSpeed(2f).SetPosB(-100,-130+m).SetMoveSpeed(1f);
                AddGrower(gs,  100,  130).SetGrowSpeed(2f).SetPosB( 100, 130+m).SetMoveSpeed(1f);
                AddWall(-100,-440, 1000,200).SetPosB(-100,-440+m).SetMoveSpeed(1f);
                AddWall( 100, 440, 1000,200).SetPosB( 100, 440+m).SetMoveSpeed(1f);
            }


            else if (li == i++) { // Double slash time
                scoreRequired = 1800;
                float s = 190;
                AddGrower(gs,    0, 0).SetGrowSpeed(1.3f);
                AddWall(-s,-s, 30).SetPosB( s,-s).SetMoveSpeed(0.3f);
                AddWall(-s, s, 30).SetPosB( s, s).SetMoveSpeed(0.3f, Mathf.PI);
                AddWall(-s, s, 30).SetPosB(-s,-s).SetMoveSpeed(0.3f);
                AddWall( s, s, 30).SetPosB( s,-s).SetMoveSpeed(0.3f, Mathf.PI);
            }


            //else if (li == i++) { // Fast and slow
            //    scoreRequired = 2500;
            //    AddGrower(gs, 0, -190).SetGrowSpeed(4);
            //    AddGrower(gs, 0,  190).SetGrowSpeed(0.8f);
            //}
            else if (li == i++) { // + perfect fit
                scoreRequired = 2500;
                //AddGrower(gs,     0, -190).SetGrowSpeed(;
                AddGrower(gs,    0,  190);
                AddGrower(gs, -190,   0);
                AddGrower(gs,  190,   0);
            }


            // Moving Growers
            else if (li == i++) { // Cutoff monitor
                scoreRequired = 1300;
                AddGrower(gs,    0, 200);
                AddGrower(gs,    0,-220).SetGrowSpeed(1f);
                AddWall(-180,60, 30).SetPosB(180, 60).SetMoveSpeed(0.12f, 0);
            }

            //else if (li == i++) { // First one is super slow, gotta just tap it right away to save time
            //    scoreRequired = 1000;
            //    AddGrower(gs, -230, 330).SetGrowSpeed(0.1f);
            //    AddGrower(gs,    0, 170);
            //    AddWall(-180,60, 30).SetPosB(180, 60).SetMoveSpeed(0.2f, -1.5f);
            //}
            else if (li == i++) { //TEST
                scoreRequired = 1400;
                float o = 1.5f;
                AddGrower(gs, -140, 140).SetPosB(140, 140);
                AddGrower(gs, -140,   0).SetPosB(140,   0).SetMoveSpeed(1, o*1);
                AddGrower(gs, -140,-140).SetPosB(140,-140).SetMoveSpeed(1, o*2);
            }
            else if (li == i++) { //TEST
                scoreRequired = 1400;
                float o = 1.5f;
                AddGrower(gs, -140, 250).SetPosB(140, 250);
                AddGrower(gs, -140, 125).SetPosB(140, 125).SetMoveSpeed(1, o*1);
                AddGrower(gs, -140,   0).SetPosB(140,   0).SetMoveSpeed(1, o*2);
                AddGrower(gs, -140,-125).SetPosB(140,-125).SetMoveSpeed(1, o*3);
                AddGrower(gs, -140,-250).SetPosB(140,-250).SetMoveSpeed(1, o*4);
            }

            else if (li == i++) { // 2 out-of-order diagonal
                scoreRequired = 2200;
                AddGrower(gs, 140, 160);
                AddGrower(gs,   0,-100);
            }



            else if (li == i++) { // 3 haphazard V
                scoreRequired = 2000;
                AddGrower(gs, -100, -200);
                AddGrower(gs,  100, -200);
                AddGrower(gs,  -60,  100);
            }
            else if (li == i++) { // + perfect fit
                scoreRequired = 2500;
                AddGrower(gs, -190,   0);
                AddGrower(gs,  190,   0);
                AddGrower(gs,    0, -190);
                AddGrower(gs,    0,  190);
            }
            else if (li == i++) { // 3 diagonal with round walls
                scoreRequired = 1700;
                AddWall(-275,-375, 550);
                AddWall( 275, 375, 550);
                AddGrower(gs,  122, -220);
                AddGrower(gs, -122,  220);
                AddGrower(gs,    0,    0);
            }
            else if (li == i++) { // TEST
                scoreRequired = 2000;
				AddWall(-275,0, 300);
				AddWall( 275,0, 300);
                AddGrower(gs,  -120, -190);
                AddGrower(gs,   120,  190);
                AddGrower(gs,     0,    0);
            }
            else if (li == i++) { // TEST
                scoreRequired = 2000;
				AddWall(-500,0, 700);
				AddWall( 500,0, 700);
                AddGrower(gs,    0,    0);
                AddGrower(gs, -120, -190);
                AddGrower(gs,  120,  190);
            }

            else if (li == i++) { // TEST
                scoreRequired = 3000;
                AddGrower(gs, 0,0).SetGrowSpeed(3f);
            }
            /* Level ideas
             * Two right next to each other
             * Two pairs next to each other
             * One hidden-tucked in a corner
             * A few lvls with different speed growing
             * A few lvls with random scattered dots
             * Have to do 3 small-ish ones, then one bigger hard one when the pressure's on!
             * /

            // UNTESTED
            else if (li == i++) { // 5-die max-fit
                scoreRequired = 3000;
                AddGrower(gs,     0,    0);
                AddGrower(gs,  -200, -300);
                AddGrower(gs,   200, -300);
                AddGrower(gs,  -200,  300);
                AddGrower(gs,   200,  300);
            }


			// TESTSSS
			else if (li == i++) {
				AddGrower(gs, -140, 270).SetPosB(140, 270);
			}
			else if (li == i++) {
				AddGrower(gs, 260, 0);
			}
			else if (li == i++) {
				AddGrower(gs,-50, 0);
				AddGrower(gs, 10, 0);
			}
			else if (li == i++) {
				AddGrower(gs,-50, 200);
				AddGrower(gs, 10, 200);
			}
			else if (li == i++) {
				AddGrower(gs, 0, -50);
				AddGrower(gs, 0,  10);
			}
			else if (li == i++) {
				AddGrower(gs, 200, -50);
				AddGrower(gs, 200,  10);
			}
			else if (li == i++) {
				AddGrower(PropShapes.Rect, 0,0).SetRotateSpeed(1f);
				AddWall(0,200, 50,50).SetPosB(200,200).SetMoveSpeed(2f);
				AddWall(0,-200, 100,50).SetRotateSpeed(1f).SetPosB(200,200).SetMoveSpeed(2f);
			}



            /*
            else if (li == i++) { // Simple 2 moving contra
                scoreRequired = 1600;
                AddGrower(gs, -100, 160).SetPosB(100, 160);
                AddGrower(gs, -100,-160).SetPosB(100,-160).SetMoveSpeed(1, Mathf.PI);
            }
            else if (li == i++) { // 5-die (Cut because it requires thinking)
                scoreRequired = 1800;
                AddGrower(gs,    0,    0);
                AddGrower(gs, -120, -150);
                AddGrower(gs,  120, -150);
                AddGrower(gs, -120,  150);
                AddGrower(gs,  120,  150);
            }
			else if (li == i++) { // Triplex
				scoreRequired = 1000;
				AddWall(0,-140, 600,25);
				AddWall(0, 140, 600,25);
				AddGrower(gs,  150,  262);
				AddGrower(gs, -150, -262);
				AddGrower(gs,    0,    0);
			}
            else if (li == i++) { // N tetronimo TO DO: Add some more dots here and there
                scoreRequired = 1600;
                AddGrower(gs,  -100, -180);
                AddGrower(gs,   100,    0);
                AddGrower(gs,  -100,    0);
                AddGrower(gs,   100,  180);
            }
            else if (li == i++) { // 4 top-lined
                scoreRequired = 570;
                AddGrower(gs,  -195, 295);
                AddGrower(gs,   -65, 295);
                AddGrower(gs,    65, 295);
                AddGrower(gs,   210, 310);
            }
            else if (li == i++) { // 4 Side-huggers
            }
            else if (li == i++) { // 4 Random
            }
            else if (li == i++) { // 4 Random
            }
            else if (li == i++) { // Random 4
            }
            else if (li == i++) { // Random 5
            }
            else if (li == i++) { // Rhombus
                scoreRequired = 1700;
                AddGrower(gs,   -74, -100);
                AddGrower(gs,   100,    0);
                AddGrower(gs,  -100,  100);
                AddGrower(gs,    74,  200);
            }
            else if (li == i++) { // 2 in-order diagonal
                scoreRequired = 2200;
                AddGrower(gs,     0,-100);
                AddGrower(gs,   140, 160);
            }
            * /


			else {
				DestroyLevelComponents();
                levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
				Debug.LogWarning("No level data available for level: " + li);
			}

			// Start growing the first dude!
            SetCurrentGrowerIndex(0);
		}
    */

	}
}

/*

		//		private void CheckOscillatingCirclesOverlaps() {
//			for (int i=circles.Count-1; i>=0; --i) {
//				if (circles[i].IsOscillating) {
//					if (IsCircleIllegalOverlap(circles[i])) {
//						OnCircleIllegalOverlap(circles[i]);
//					}
//				}
//			}
//		}
//		// ----------------------------------------------------------------
//		//  Events
//		// ----------------------------------------------------------------
//		public void OnWinLevel(Player winningPlayer) {
//			// Shaken, not stirred!
//			screenShakeVolume = 2f;
//		}
//		// ----------------------------------------------------------------
//		//  Update
//		// ----------------------------------------------------------------
//		private void Update() {
//			UpdateScreenShake();
//		}
//		private void UpdateScreenShake() {
//			if (screenShakeVolume != 0) {
//				// Apply!
//				//float rotation = Mathf.Sin(screenShakeVolume*5f) * screenShakeVolume*4f;
//				//this.transform.localEulerAngles = new Vector3(0,0,rotation);
//				float yOffset = Mathf.Sin(screenShakeVolume*20f) * screenShakeVolume*7f;
//				myRectTransform.anchoredPosition = new Vector3(0, yOffset, 0); // TEST
//				// Update!
//				screenShakeVolume += (0-screenShakeVolume) / 24f * TimeController.FrameTimeScale;
//				if (Mathf.Abs(screenShakeVolume) < 0.5f) { screenShakeVolume = 0; }
//			}
//		}

		private Vector2 GetBestPosForNewCircle() {
			Vector2 randPos;
			// Kinda sloppy! Brute-force-y.
			randPos = GetOpenPosForNewCircle(200);
			if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
			randPos = GetOpenPosForNewCircle(100); // TO DO: CLean up this negative infinity awkwardness checksings.
			if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
			randPos = GetOpenPosForNewCircle(50);
			if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
			randPos = GetOpenPosForNewCircle(10);
			if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
			Debug.Log("Couldn't find open pos for new circle!");
			return Vector2.negativeInfinity;
		}
		private Vector2 GetOpenPosForNewCircle(float newRadius) {
			Vector2 pos;
			int safetyCount=0;
			do {
				pos = new Vector2(Random.Range(r_levelBounds.xMin,r_levelBounds.xMax), Random.Range(r_levelBounds.yMin,r_levelBounds.yMax));
				if (CanAddCircleAtPos(pos, newRadius)) { break; }
				if (safetyCount++>499) {
					//Debug.Log("Couldn't find open pos for new circle!");
					return Vector2.negativeInfinity;
				}
			}
			while(true);
			return pos;
		}
		private bool CanAddCircleAtPos(Vector2 pos, float radius) {
			return !IsCircleAtPos(pos, radius) && IsCircleInBounds(pos, radius);
		}
        private void AddNewCircle() {
            float startingRadius = 5f;
            Vector2 randPos = GetBestPosForNewCircle();
            // Can't find a suitable position to put this circle? We're outta room!
            if (randPos.x == Mathf.NegativeInfinity) {
                SetGameOver();
            }
            // We DID find a suitable pos for this new circle! Add it!
            else {
                Circle newCircle = Instantiate(resourcesHandler.circleGrow_circle).GetComponent<Circle>();
                newCircle.Initialize(this, canvas.transform, randPos, startingRadius);
                circles.Add(newCircle);
                newCircle.SetIsOscillating(true); // Start it out oscillating up up!
            }
            // Update the score now! :)
            UpdateScore();
        }
*/
/*
private bool IsIllegalOverlap(Grower circle) {
	return IsCircleAtPos(circle.Pos, circle.Radius) || !IsCircleInBounds(circle.Pos, circle.Radius);
}
private bool IsCircleInBounds(Vector2 pos, float radius) {
	if (pos.x-radius < r_levelBounds.xMin) { return false; }
	if (pos.x+radius > r_levelBounds.xMax) { return false; }
	if (pos.y-radius < r_levelBounds.yMin) { return false; }
	if (pos.y+radius > r_levelBounds.yMax) { return false; }
	return true;
}
private bool IsCircleAtPos(Vector2 pos, float radius) {
	foreach (Grower c in growers) {
		if (c.Pos==pos && c.Radius==radius) { continue; } // Skip itself.
		if (DoCirclesOverlap(c.Pos,c.Radius, pos,radius)) { return true; }
	}
	return false; // We're good!
}
private bool DoCirclesOverlap(Grower circleA, Grower circleB) {
	return DoCirclesOverlap(circleA.Pos,circleA.Radius, circleB.Pos,circleB.Radius);
}
private bool DoCirclesOverlap(Vector2 posA,float radiusA, Vector2 posB,float radiusB) {
	float dist = Vector2.Distance(posA,posB);
	return dist < radiusA+radiusB;
}
*/