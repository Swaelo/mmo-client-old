using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItems : MonoBehaviour
{
    public List<GameItem> Items = new List<GameItem>();
    public void AddItem(GameItem NewItem) { Items.Add(NewItem); }
    public void RemoveItemByItemNumber(int ItemNumber)
    {
        for(int i = 0; i < Items.Count; i++)
        {
            GameItem Item = Items[i];
            if(Item.ItemNumber == ItemNumber)
            {
                GameObject.Destroy(Item.gameObject);
                return;
            }
        }
    }
}
