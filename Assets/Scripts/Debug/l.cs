// ================================================================================================================================
// File:        l.cs
// Description: Quickly and easily print messages to the chat window from anywhere in the code by typing l.og("hello world");
// ================================================================================================================================

using UnityEngine;

public class l
{
    public static void og(string msg)
    {
        ChatWindowManager.Instance.DisplayMessage(msg);
    }

    public static void og(int value)
    {
        og("int: " + value.ToString());
    }

    public static void og(float value)
    {
        og("float: " + value.ToString());
    }

    public static void og(bool value)
    {
        og("bool: " + value.ToString());
    }

    public static void og(UnityEngine.Vector3 Vector)
    {
        og("Vector3: " + Vector.x + ", " + Vector.y + ", " + Vector.z);
    }
}
