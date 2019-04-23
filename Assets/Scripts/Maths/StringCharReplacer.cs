// ================================================================================================================================
// File:        StringCharReplacer.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class StringCharReplacer : MonoBehaviour
{
    public static string ReplaceChars(string String, char OldChar, char NewChar)
    {
        List<char> CharList = new List<char>();
        foreach (char a in String)
            CharList.Add(a == OldChar ? NewChar : a);

        string NewString = "";
        foreach (char a in CharList)
            NewString.Insert(NewString.Length, a.ToString());

        return NewString;
    }
}
