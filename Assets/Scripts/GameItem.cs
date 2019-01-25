using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    public int ItemID = -1;
    public string ItemName = "not set";
    public Vector3 ItemPosition = new Vector3();

    public GameItem(int ID, string Name, Vector3 Position) { ItemID = ID; ItemName = Name; ItemPosition = Position; }
}
