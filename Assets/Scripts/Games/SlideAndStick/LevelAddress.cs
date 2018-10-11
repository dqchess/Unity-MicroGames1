using UnityEngine;

namespace SlideAndStick {
    public struct LevelAddress {
    	// Statics
    	static public readonly LevelAddress undefined = new LevelAddress(-1, -1, -1, -1);
    	static public readonly LevelAddress zero = new LevelAddress(0,0,0,0);
    	// Properties
    	public int mode;
    	public int collection;
    	public int pack;
    	public int level;
    
    	public LevelAddress (int mode, int collection, int pack, int level) {
    		this.mode = mode;
    		this.collection = collection;
    		this.pack = pack;
    		this.level = level;
    	}
    
    	public bool IsTutorial { get { return mode == GameModes.TutorialIndex; } }
    	public LevelAddress NextLevel { get { return new LevelAddress(mode, collection, pack, level+1); } }
    	public LevelAddress PreviousLevel { get { return new LevelAddress(mode, collection, pack, level-1); } }
    
    	public override string ToString() { return mode + "," + collection + "," + pack + "," + level; }
    	static public LevelAddress FromString(string str) {
    		string[] array = str.Split(',');
    		if (array.Length >= 4) {
    			return new LevelAddress (int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
    		}
    		return LevelAddress.undefined; // Hmm.
    	}
    
    	public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
    	public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
    
    	public static bool operator == (LevelAddress a, LevelAddress b) {
    		return a.Equals(b);
    	}
    	public static bool operator != (LevelAddress a, LevelAddress b) {
    		return !a.Equals(b);
    	}
    }
}