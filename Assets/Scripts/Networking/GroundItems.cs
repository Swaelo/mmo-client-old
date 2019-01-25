using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItems : MonoBehaviour
{
    public static List<GameItem> Items = new List<GameItem>();
    public void AddItem(GameItem NewItem) { Items.Add(NewItem); }
}
