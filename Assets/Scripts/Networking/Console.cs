using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance;
    private void Awake() { Instance = this; }

    public Text[] Lines;

    public void Print(string line)
    {
        //Move all the previous message up 1 line in the window
        for(int i = Lines.Length - 1; i > 0; i--)
        {
            Lines[i].text = Lines[i - 1].text;
        }
        //Now set the new message into the last line
        Lines[0].text = line;
    }
}