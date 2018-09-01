using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CirclePop {
	public class Level : BaseLevel {
		// Constants
		public static int FirstLevelIndex = 1;
		public static int LastLevelIndex = 39;
		// Components
        [SerializeField] private LevelBounds bounds=null;
        [SerializeField] private LevelUI levelUI=null;
        [SerializeField] private RectTransform rt_gameComponents=null; // Growers and Walls go on this!
		private List<Grower> growers;
		private List<Wall> walls;
		private List<Image> i_collisionIcons;
		// Properties
//      private float screenShakeVolume;
        private int scoreRequired;
        private int currentGrowerIndex;
        private string description; // only for DEVELOPER. Makes debugging easier. It's what comes after "LEVEL" in the Level.txt string.
		// References
		private GameController gameController;


        // Getters (Public)
        public bool IsFUEGameplayFrozen { get { return gameController.IsFUEGameplayFrozen; } }
		public bool DidGetScoreRequired { get { return gameController.DidGetScoreRequired; } }
		public bool IsCurrentGrowerGrowing { get { return currentGrower!=null && currentGrower.CurrentState==GrowerStates.Growing; } }
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
			BaseInitialize(gameController, tf_parent, _levelIndex);

			bounds.Initialize();
			AddLevelComponents();
			levelUI.Initialize();

			// Scale me DOWN if the screen is too short! A) I don't love that we're scaling the whole lvl, and B) This *could* be a shared property of ALL BaseLevels.
			float gcHeight = rt_gameComponents.rect.height;
			float canvasHeight = Canvas.GetComponent<RectTransform>().rect.height;
			float availableHeight = canvasHeight + rt_gameComponents.anchoredPosition.y; // how much ACTUAL room we've got to work with (from gc's top to the bottom of the screen).
			float gcScale = Mathf.Min(1, availableHeight/gcHeight);
			rt_gameComponents.transform.localScale = Vector3.one*gcScale;
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScoreUI(int scorePossible, int scoreSolidified) {
            levelUI.UpdateScoreUI(scorePossible, scoreSolidified);
		}
		private void AddIllegalOverlapIcon(Vector2 pos) {
			Image iconImage = Instantiate(resourcesHandler.circlePop_collisionIcon).GetComponent<Image>();
			GameUtils.ParentAndReset(iconImage.gameObject, rt_gameComponents);
			iconImage.rectTransform.anchoredPosition = pos;
			i_collisionIcons.Add(iconImage);
		}
        private void SetAllPropCollidersEnabled(bool _isEnabled) {
            foreach (Grower prop in growers) { prop.SetColliderEnabled(_isEnabled); }
            foreach (Wall prop in walls) { prop.SetColliderEnabled(_isEnabled); }
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
				// Little nice edge case: Have we hit the target score?? Auto-solidify the next Grower (this'll keep going recursively until all are solidified)!
				if (DidGetScoreRequired) {
					SolidifyCurrentGrower();
				}
				// Otherwise, prep the next guy!
				else {
					currentGrower.SetAsPreGrowing();
				}
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
//			if (currentGrower.CurrentState != GrowerStates.Growing) { return; } // Oh, if it's only pre-growing, DON'T do anything. NOTE: Disabled! Check is unnecessary, and messes up the auto-solidify-all-remaining-Growers edge-case.

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
            SetAllPropCollidersEnabled(false);
		}

		public void OnTapDown() {
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
//			AddIllegalOverlapIcon(pos);//NOTE: Disabled. // Add the no-no icon!
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
        private Grower AddGrower(GrowerData data) {
			GameObject prefabGO = null;
            switch (data.shape) {
			case PropShapes.Circle: prefabGO = resourcesHandler.circlePop_growerCircle; break;
			case PropShapes.Rect: prefabGO = resourcesHandler.circlePop_growerRect; break;
			case PropShapes.Composite: prefabGO = resourcesHandler.circlePop_growerComposite; break;
            default: Debug.LogError("Grower shape not yet supported. Time to write some more code! Shape: " + data.shape); break;
			}
			Grower newObj = Instantiate(prefabGO).GetComponent<Grower>();
            newObj.Initialize(this, rt_gameComponents, data);
            growers.Add(newObj);
            return newObj;
		}
        private Wall AddWall(WallData data) {
			GameObject prefabGO = null;
            switch (data.shape) {
			case PropShapes.Circle: prefabGO = resourcesHandler.circlePop_wallCircle; break;
			case PropShapes.Rect: prefabGO = resourcesHandler.circlePop_wallRect; break;
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
            if (LevelIndex > LastLevelIndex) {
                levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
            }

            // Start growing the first dude!
            SetCurrentGrowerIndex(0);
        }
        private void MakeLevelFromString(string _str) {
            try {
                string[] lines = TextUtils.GetStringArrayFromStringWithLineBreaks(_str);
                description = lines[0]; // Description will be the first line (what follows "LEVEL ").
                foreach (string s in lines) {
                    if (s.StartsWith("scoreReq")) {
                        SetScoreRequiredFromString(s);
                    }
                    else if (s.StartsWith("bounds")) {
                        SetBoundsFromString(s);
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
                Debug.LogError("Error reading level string! LevelIndex: " + LevelIndex + ", description: \"" + description + "\". Error: " + e);
            }
        }
        private void SetScoreRequiredFromString(string s) {
            s = s.Substring(s.IndexOf('=')+1);
            scoreRequired = TextUtils.ParseInt(s);
        }
        private void SetBoundsFromString(string s) {
            s = s.Substring(s.IndexOf('=')+1);
            bounds.SetSize(TextUtils.GetVector2FromStringNoParens(s));
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
                if (s.StartsWith("doMoveWhenSolid")) {
                    data.doMoveWhenSolid = true;
				}
				else if (s.StartsWith("growSpeed=")) {
					data.growSpeed = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("growSpeedBounce=")) {
					data.growSpeedBounce = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("growSpeedGravity=")) {
					data.growSpeedGravity = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("growSpeedMin=")) {
					data.growSpeedMin = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("growSpeedMax=")) {
					data.growSpeedMax = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("growSpeedOscFreq=")) {
					data.growSpeedOscFreq = TextUtils.ParseFloat(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("part=")) { // Parts can be either Circles or Rects; determined by number of params for the part.
					GrowerCompositePartData partData = new GrowerCompositePartData(s.Substring(s.IndexOf('=')+1));
					data.parts.Add(partData);
				}
            }
            // Add the dude!
            AddGrower(data);
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
            AddWall(data);//.shape, data.pos.x,data.pos.y, data.size.x,data.size.y);
        }
        /// The startIndex makes this confusing, so ignore it (it's for optimization). We use this function to convert string properties from Levels.txt to PropData properties. :)
        private void SetPropDataProperties(PropData data, string[] properties, int startIndex) {
            for (int i=startIndex; i<properties.Length; i++) {
                string s = properties[i].TrimStart();
                if (s.StartsWith("posB=")) {
                    data.posB = TextUtils.GetVector2FromStringNoParens(s.Substring(s.IndexOf('=')+1));
				}
				else if (s.StartsWith("doHideMovePath")) {
					data.doHideMovePath = true;
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

        private PropShapes GetGrowerShapeFromLine(string s) {
			if (s.StartsWith("growerC")) { return PropShapes.Circle; }
			else if (s.StartsWith("growerR")) { return PropShapes.Rect; }
			else if (s.StartsWith("growerP")) { return PropShapes.Composite; }
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



	}
}

/*

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
*/