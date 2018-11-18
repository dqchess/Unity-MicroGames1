using UnityEngine;

public struct Vector2Int {
	static public readonly Vector2Int zero = new Vector2Int(0,0);
	static public readonly Vector2Int none = new Vector2Int(-1,-1);
	static public readonly Vector2Int L = new Vector2Int (-1, 0);
	static public readonly Vector2Int R = new Vector2Int ( 1, 0);
	static public readonly Vector2Int B = new Vector2Int ( 0, 1);
	static public readonly Vector2Int T = new Vector2Int ( 0,-1);
	static public readonly Vector2Int TL = new Vector2Int (-1,-1);
	static public readonly Vector2Int TR = new Vector2Int ( 1,-1);
	static public readonly Vector2Int BL = new Vector2Int (-1, 1);
	static public readonly Vector2Int BR = new Vector2Int ( 1, 1);

	public int x;
	public int y;
	public Vector2Int (int _x,int _y) {
		x = _x;
		y = _y;
	}
	public Vector2Int (float _x,float _y) {
		x = Mathf.RoundToInt(_x);
		y = Mathf.RoundToInt(_y);
	}
	public Vector2 ToVector2 () { return new Vector2 (x,y); }

	public override string ToString() { return x+","+y; }
	public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
	public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).

    public static Vector2Int operator + (Vector2Int a, Vector2Int b) {
        return new Vector2Int(a.x+b.x, a.y+b.y);
    }
    public static Vector2Int operator - (Vector2Int a, Vector2Int b) {
        return new Vector2Int(a.x-b.x, a.y-b.y);
    }
    public static Vector2Int operator * (Vector2Int a, float m) {
        return new Vector2Int(a.x*m, a.y*m);
    }
	public static bool operator == (Vector2Int a, Vector2Int b) {
		return a.Equals(b);
	}
	public static bool operator != (Vector2Int a, Vector2Int b) {
		return !a.Equals(b);
	}
    
    
    /** Returns a Vector2Int that only has ONE value set (e.g. 1,0; 0,-1). */
    public static Vector2Int Sign(int x,int y) {
        Vector2Int v = new Vector2Int(MathUtils.Sign(x), MathUtils.Sign(y));
        if (v.x!=0 && v.y!=0) { v.y = 0; } // Don't-allow-diagonals!
        return v;
    }
    
    public Vector2Int RotatedClockwise() {
        if (this == Vector2Int.T) { return Vector2Int.R; }
        if (this == Vector2Int.R) { return Vector2Int.B; }
        if (this == Vector2Int.B) { return Vector2Int.L; }
        if (this == Vector2Int.L) { return Vector2Int.T; }
        Debug.LogError("Whoa! Rotating a Vector2Int that ISN'T TLBR. TO DO: Use sin/cos to rotate these properly.");
        return this;
    }
    public Vector2Int RotatedCounterClockwise() {
        if (this == Vector2Int.T) { return Vector2Int.L; }
        if (this == Vector2Int.R) { return Vector2Int.T; }
        if (this == Vector2Int.B) { return Vector2Int.R; }
        if (this == Vector2Int.L) { return Vector2Int.B; }
        Debug.LogError("Whoa! Rotating a Vector2Int that ISN'T TLBR. TO DO: Use sin/cos to rotate these properly.");
        return this;
    }
}