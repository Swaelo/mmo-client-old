using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour
{
    public InputField ServerIPInputField;

    public void ConnectServer()
    {
        ConnectionManager.Instance.TryConnect(ServerIPInputField.text);
    }
}
