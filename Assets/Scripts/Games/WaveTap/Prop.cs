using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveTap {
	public class Prop : MonoBehaviour {
		// Components
		[SerializeField] protected RectTransform myRectTransform=null;
		// References
		protected Level level;

		// Getters (Public)
		public float PosY { get { return posY; } }
		// Getters (Protected)
		protected int LevelIndex { get { return level==null ? 1 : level.LevelIndex; } }
		protected Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		protected float posY {
			get { return Pos.y; }
			set { Pos = new Vector2(Pos.x, value); }
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		protected void InitializeAsProp(Level _level, Transform tf_parent, PropData _data) {
			level = _level;
			GameUtils.ParentAndReset(this.gameObject, tf_parent);
			Pos = _data.pos;
		}
		
	}
}