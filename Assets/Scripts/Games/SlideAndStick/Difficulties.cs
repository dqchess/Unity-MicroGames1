using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Difficulties {
	public const int Undefined = 0;
	public const int VeryEasy = 1;
	public const int Easy = 2;
	public const int Medium = 3;
	public const int Hard = 4;
	public const int VeryHard = 5;
	public const int SupaHard = 6;

	public static string GetName(int difficulty) {
		switch (difficulty) {
			case VeryEasy:	return "very easy";
			case Easy:		return "easy";
			case Medium:	return "medium";
			case Hard:		return "hard";
			case VeryHard:	return "very hard";
			case SupaHard:	return "supa hard";
			default:		return "undefined";
		}
	}

}
