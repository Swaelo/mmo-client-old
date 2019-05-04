// ================================================================================================================================
// File:        StringCharReplacer.cs
// Description: Helper functions for string editing
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class StringEditor : MonoBehaviour
{
    public static string RemoveSpaces(string String)
    {
        string NewString = "";

        for (int i = 0; i < String.Length; i++)
            if (String[i] != ' ')
                NewString += String[i];

        return NewString;
    }

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
