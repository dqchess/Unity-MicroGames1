using UnityEngine;

namespace WaveTap {

	public class PropData {
		public Vector2 pos=Vector2.zero;
	}

	public class BarData : PropData {
		public int numKnocksReq=1;
//		public Vector2 posB=Vector2.positiveInfinity; // default to unused.
//		public float moveSpeed=1;
//		public float moveLocOffset=0;
	}
	public class PlayerData : PropData {
		public float startingLoc=0; // where I start the level at.
		public float posYStart = 200f; // TOP of my wave.
		public float posYEnd = -200f; // BOTTOM of my wave.
		public PlayerData() { }
		public PlayerData(float _startingLoc, float _posYStart,float _posYEnd) {
			startingLoc = _startingLoc;
			posYStart = _posYStart;
			posYEnd = _posYEnd;
		}
	}

}