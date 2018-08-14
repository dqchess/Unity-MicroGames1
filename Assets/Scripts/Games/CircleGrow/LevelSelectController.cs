using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CircleGrow {
	public class LevelSelectController : BaseLevelSelectController {
		// Overrideables
		override public    string MyGameName() { return GameNames.CircleGrow; }
		override protected GameObject LevelTilePrefab() { return ResourcesHandler.Instance.circleGrow_levelTile; }
		override protected int FirstLevelIndex() { return Level.FirstLevelIndex; }
		override protected int LastLevelIndex()  { return Level.LastLevelIndex;  }
//		// Components
//		[SerializeField] private TextMeshProUGUI t_headerA=null;
//		[SerializeField] private TextMeshProUGUI t_headerB=null;
		// Properties
//		private float oscLoc;


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		override protected void Start() {
			tileSize = new Vector2(120,120); // to override, set these values in Start() before calling base.Start().
			tileGap = new Vector2(8,8);

			base.Start();

//			oscLoc = Random.Range(0f, 100f);
		}



//		// ----------------------------------------------------------------
//		//  Update
//		// ----------------------------------------------------------------
//		override protected void Update () {
//			base.Update();
//
//			UpdateHeaderColors();
//		}
//		private void UpdateHeaderColors() {
//			float h;
//			float s = 1f;
//			float b = 0.92f;
//
//			oscLoc += Time.deltaTime;
//
//			h = (oscLoc*0.07f) % 1f;
//			t_headerA.color = new ColorHSB(h,s,b).ToColor();
//			h = (oscLoc*0.03f+0.3f) % 1f;
//			t_headerB.color = new ColorHSB(h,s,b).ToColor();
//		}



	}
}