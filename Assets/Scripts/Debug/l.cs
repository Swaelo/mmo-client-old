using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class l
{
    public static void og(string msg)
    {
        ChatWindow.Instance.DisplaySystemMessage(msg);
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
