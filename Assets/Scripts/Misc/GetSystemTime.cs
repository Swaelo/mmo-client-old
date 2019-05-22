using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSystemTime
{
    //Returns a string representing the current system time in a nicely formatted way
    public static string GetSystemTimeString()
    {
        DateTime Time = System.DateTime.Now;
        bool PM = Time.Hour > 12;
        int Hour = PM ? Time.Hour - 12 : Time.Hour;
        Hour = Hour == 0 ? 12 : Hour;
        string Minute = Time.Minute < 10 ? "0" + Time.Minute : Time.Minute.ToString();
        string Second = Time.Second < 10 ? "0" + Time.Second : Time.Second.ToString();
        return Hour + ":" + Minute + ":" + Second + " " + (PM ? "PM" : "AM");
    }
}
