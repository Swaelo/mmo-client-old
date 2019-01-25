using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance;

    public Text Line1;
    public Text Line2;
    public Text Line3;
    public Text Line4;
    public Text Line5;

    private void Awake()
    {
        Instance = this;
    }

    public void Print(string line)
    {
        Debug.Log("Console: " + line);
        Line5.text = Line4.text;
        Line4.text = Line3.text;
        Line3.text = Line2.text;
        Line2.text = Line1.text;
        Line1.text = line;
    }
}