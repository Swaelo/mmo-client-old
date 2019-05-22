using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonFunctions : MonoBehaviour
{
    public void ConnectServer()
    {
        ConnectionManager.Instance.TryConnect();
    }
}
