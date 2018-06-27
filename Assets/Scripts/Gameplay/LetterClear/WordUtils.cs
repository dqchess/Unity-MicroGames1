using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class WordUtils {

    static private readonly char[] vowelChars = new char[]{'e'};//TEMP TEST 'a', 'e', 'i', 'o', 'u', 'y'};


    static public bool IsVowel(char c) {
        foreach (char vowel in vowelChars) {
            if (c == vowel) { return true; }
        }
        return false;
    }

}
