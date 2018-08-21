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
		public float posYMax =  200f; // TOP of my wave.
		public float posYMin = -200f; // BOTTOM of my wave.
		public PlayerData() { }
        public PlayerData(float _posYMax,float _posYMin) {
			posYMax = _posYMax;
			posYMin = _posYMin;
		}
	}

}