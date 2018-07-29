using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	public class Level : BaseLevel {
		// Constants
		public static int FirstLevelIndex = 1;
//		public static int LastLevelIndex = 101;
		// Components
		[SerializeField] private Image i_border=null;
		[SerializeField] private Image i_bounds=null;
		[SerializeField] private TextMeshProUGUI t_levelName=null;
		[SerializeField] private TextMeshProUGUI t_moreLevelsComingSoon=null;
		private List<Circle> circles;
		// Properties
//		private float screenShakeVolume;
		private int currentCircleIndex;
		private Rect r_levelBounds; // set to i_bounds.rect in Initialize.
		// References
		private GameController gameController;


		// Getters (Public)
		public List<Circle> Circles { get { return circles; } }
		// Getters (Private)
		private Circle currentCircle {
			get {
				if (currentCircleIndex<0 || currentCircleIndex>=circles.Count) { return null; } // Index outta bounds? Return null.
				return circles[currentCircleIndex];
			}
		}
		private bool IsCircleIllegalOverlap(Circle circle) {
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
			foreach (Circle c in circles) {
				if (c.Pos==pos && c.Radius==radius) { continue; } // Skip itself.
				if (DoCirclesOverlap(c.Pos,c.Radius, pos,radius)) { return true; }
			}
			return false; // We're good!
		}
		//private bool IsCircleOverlappingAnother(Circle circle) {
		//    foreach (Circle c in circles) {
		//        if (c == circle) { continue; } // Skip itself of course.
		//        if (DoCirclesOverlap(c, circle)) { return true; }
		//    }
		//    return false; // We're good!
		//}
		private bool DoCirclesOverlap(Circle circleA, Circle circleB) {
			return DoCirclesOverlap(circleA.Pos,circleA.Radius, circleB.Pos,circleB.Radius);
		}
		private bool DoCirclesOverlap(Vector2 posA,float radiusA, Vector2 posB,float radiusB) {
			float dist = Vector2.Distance(posA,posB);
			return dist < radiusA+radiusB;
		}



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(GameController _gameController, Transform tf_parent, int _levelIndex) {
			gameController = _gameController;
			BaseInitialize(tf_parent, _levelIndex);

			t_levelName.text = "LEVEL " + LevelIndex.ToString();

			r_levelBounds = i_bounds.rectTransform.rect;
			r_levelBounds.center += new Vector2(0, r_levelBounds.height*0.5f);// Hacky center the bounds because Level and Circle anchors are currently different :P

			i_border.color = Circle.color_solid;
		}


		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		public void SolidifyCurrentCircle() {
			if (currentCircle == null) { return; } // Safety check.

			currentCircle.OnSolidify();

			// TODO: Calculate score and stuff too

			// Move onto the next circle!
			SetCurrentCircleIndex(currentCircleIndex + 1);
		}
		private void OnCircleIllegalOverlap(Circle circle) {
			circle.OnIllegalOverlap();
			gameController.LoseLevel();
		}
		private void SetCurrentCircleIndex(int _index) {
			currentCircleIndex = _index;
			//			circles[currentCircleIndex].SetIsOscillating(true);
			// There IS another circle!
			if (currentCircle != null) {
//				currentCircle.
			}
			// There is NOT another circle! End the level.
			else {
				// TODO: This.
			}
		}





		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update () {
			if (Time.timeScale == 0) { return; } // No time? Do nothin'.

			if (gameController.IsGameStatePlaying) {
				GrowCurrentCircle();
			}
		}
		private void GrowCurrentCircle() {
			if (currentCircle == null) { return; } // Safety check.

			currentCircle.GrowStep();
			if (IsCircleIllegalOverlap(currentCircle)) {
				OnCircleIllegalOverlap(currentCircle);
			}
		}


		// ----------------------------------------------------------------
		//  Destroying Elements
		// ----------------------------------------------------------------
		private void DestroyLevelComponents() {
			if (circles != null) {
				for (int i=circles.Count-1; i>=0; --i) {
					Destroy(circles[i].gameObject);
				}
			}
			circles = new List<Circle>();
		}
		// ----------------------------------------------------------------
		//  Adding Elements
		// ----------------------------------------------------------------
		private void AddCircle(float radius, float x,float y) {
			Circle newCircle = Instantiate(resourcesHandler.circleGrow_circle).GetComponent<Circle>();
			newCircle.Initialize(this.transform, new Vector2(x,y), radius);
			circles.Add(newCircle);
		}



		// ----------------------------------------------------------------
		//  Making Level!
		// ----------------------------------------------------------------
		override protected void AddLevelComponents() {
			DestroyLevelComponents(); // Just in case.
			if (resourcesHandler == null) { return; } // Safety check for runtime compile.

			// Reset values
			circles = new List<Circle>();

			// Specify default values
			float sr = 10; // startingRadius

			// NOTE: All coordinates are based off of a 600x800 available playing space! :)

			int li = LevelIndex;
			int i=FirstLevelIndex;
			if (false) {}


			// Simple, together.
			else if (li == i++) {
				AddCircle(sr, 0,0);
			}
			else if (li == i++) {
				AddCircle(sr, 0,-200);
				AddCircle(sr, 0, 200);
			}


			else {
				DestroyLevelComponents();
				t_moreLevelsComingSoon.gameObject.SetActive(true);
				Debug.LogWarning("No level data available for level: " + li);
			}

			// Start with the first circle oscillating!
			SetCurrentCircleIndex(0);
		}

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