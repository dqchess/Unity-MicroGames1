using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveTap {
	public class Level : BaseLevel {
		// Constants
		public static int FirstLevelIndex = 1;
		public static int LastLevelIndex = 20;
		// Components
		[SerializeField] private LevelUI levelUI=null;
		[SerializeField] private RectTransform rt_gameComponents=null; // Player and Bars go on this!
		private Player player;
		private List<Bar> bars;
		// Properties
//		private int nextBarIndex;
		private string description; // only for DEVELOPER. Makes debugging easier. It's what comes after "LEVEL" in the Level.txt string.
		// References
		private GameController gameController;


		// Getters (Private)
		private bool AreAllBarsDone() {
			foreach (Bar bar in bars) {
				if (!bar.IsDone) { return false; }
			}
			return true;
		}
		// Getters (Public)
		public Player Player { get { return player; } }
		public RectTransform rt_GameComponents { get { return rt_gameComponents; } }
		public bool IsPlayerTouchingABar() {
			return player.IsTouchingABar();
		}



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(GameController _gameController, Transform tf_parent, int _levelIndex) {
			gameController = _gameController;

			BaseInitialize(gameController, tf_parent, _levelIndex);
			levelUI.Initialize();
		}



		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnLoseLevel(LoseReasons reason) {
			levelUI.OnLoseLevel(reason);
			player.OnLoseLevel();
		}
		public void OnWinLevel() {
			levelUI.OnWinLevel();
			player.OnWinLevel();
		}



		// ----------------------------------------------------------------
		//  Destroying Elements
		// ----------------------------------------------------------------
		private void DestroyLevelComponents() {
			DestroyPlayer();
			if (bars != null) {
				for (int i=bars.Count-1; i>=0; --i) {
					Destroy(bars[i].gameObject);
				}
			}
			bars = new List<Bar>();
		}
		private void DestroyPlayer() {
			if (player != null) {
				Destroy(player.gameObject);
			}
			player = null;
		}
		// ----------------------------------------------------------------
		//  Adding Elements
		// ----------------------------------------------------------------
		private Bar AddBar(BarData data) {
			GameObject prefabGO = resourcesHandler.waveTap_bar;
			Bar newObj = Instantiate(prefabGO).GetComponent<Bar>();
			newObj.Initialize(this, rt_gameComponents, data);
			bars.Add(newObj);
			return newObj;
		}
		private Player MakePlayer(PlayerData data) {
			DestroyPlayer(); // Just in case.
			GameObject prefabGO = resourcesHandler.waveTap_player;
			Player newObj = Instantiate(prefabGO).GetComponent<Player>();
			newObj.Initialize(this, rt_gameComponents, data);
			player = newObj;
			return newObj;
		}



		// ----------------------------------------------------------------
		//  Making Level!
		// ----------------------------------------------------------------
		override protected void AddLevelComponents() {
			DestroyLevelComponents(); // Just in case.
			if (resourcesHandler == null) { return; } // Safety check for runtime compile.

			// Specify default values
//			bounds.SetSize(550,750); // Default to 600x800 with 25px padding on all sides.

			string levelString = gameController.LevelLoader.GetLevelString(LevelIndex);
			if (!string.IsNullOrEmpty(levelString)) {
				MakeLevelFromString(levelString);
			}
			else {
				DestroyLevelComponents();
//				levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
				Debug.LogWarning("No level data available for level: " + LevelIndex);
			}

//			// Head towards the first dude!
//			SetNextBarIndex(0);
		}
		private void MakeLevelFromString(string _str) {
			try {
				string[] lines = TextUtils.GetStringArrayFromStringWithLineBreaks(_str);
				description = lines[0]; // Description will be the first line (what follows "LEVEL ").
				// Default Player so we don't HAVE to provide any player info in each Level string.
				PlayerData playerData = new PlayerData();
				foreach (string s in lines) {
					if (s.StartsWith("player")) {
						playerData = GetPlayerDataFromString(s);
					}
					else if (s.StartsWith("bar")) {
						AddBarFromString(s);
					}
				}
				// Finally, make Player from either our default, or what the Level string specified.
				MakePlayer(playerData);
			}
			catch (Exception e) {
				Debug.LogError("Error reading level string! LevelIndex: " + LevelIndex + ", description: \"" + description + "\". Error: " + e);
			}
		}
		private PlayerData GetPlayerDataFromString(string fullLine) {
			PlayerData data = new PlayerData();
			fullLine = fullLine.Substring(fullLine.IndexOf(" ")+1); // remove "player ".
			string[] properties = fullLine.Split(';');
			for (int i=0; i<properties.Length; i++) {
				string s = properties[i].TrimStart();
				//if (s.StartsWith("startingLoc=")) {
					//data.startingLoc = TextUtils.ParseInt(s.Substring(s.IndexOf('=')+1));
                //} else
                if (s.StartsWith("range=")) { // Note: Range is stored as a Vector2 for more readable levels. The vector's converted to the separate start/end pos values.
                    Vector2 range = TextUtils.GetVector2FromStringNoParens(s.Substring(s.IndexOf('=')+1));
                    data.posYMax = range.x;
                    data.posYMin = range.y;
                }
			}
			return data;
		}
		private void AddBarFromString(string fullLine) {
			BarData data = new BarData();
			fullLine = fullLine.Substring(fullLine.IndexOf(" ")+1); // remove "bar ".
			string[] properties = fullLine.Split(';');
			data.pos = TextUtils.GetVector2FromStringNoParens(properties[0]);
			for (int i=1; i<properties.Length; i++) { // Note: Skip the first property, which must always be pos.
				string s = properties[i].TrimStart();
				if (s.StartsWith("numKnocksReq=")) {
					data.numKnocksReq = TextUtils.ParseInt(s.Substring(s.IndexOf('=')+1));
				}
			}
			// Add the dude!
			AddBar(data);
		}


		// ----------------------------------------------------------------
		//	Game Flow Doers
		// ----------------------------------------------------------------
		public void OnMissedBar() {
			gameController.OnMissedNextBar();
		}
		public void PlayerKnockBarsTouching() {
			player.RapBarsTouching();
			// Did we win?!
			if (AreAllBarsDone()) {
				gameController.OnKnockLastBar();
			}
		}
//		private void SetNextBarIndex(int _index) {
//			nextBarIndex = _index;
//			bool isNextBarInSameDir =
//				(player.DirMoving>0&&nextBarIndex== && nextBarIndex!=bars.Count-1; // the next Bar's in the same direction IF it's 
//			player.OnSetNextBar();
//		}



	}
}
