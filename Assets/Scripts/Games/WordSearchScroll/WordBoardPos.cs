using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WordBoardPos {
	// Properties
	public Vector2Int pos;
	public Vector2Int dir;
	public int length;

	public WordBoardPos(Vector2Int pos, Vector2Int dir, int length) {
		this.pos = pos;
		this.dir = dir;
		this.length = length;
	}
}