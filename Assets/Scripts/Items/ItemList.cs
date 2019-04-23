// ================================================================================================================================
// File:        ItemList.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class ItemList : MonoBehaviour
{
    public static ItemList Instance = null;
    private void Awake() { Instance = this; }

    public Item[] GameItems;

    public Item GetItem(int ItemID)
    {
        return GameItems[ItemID - 1];
    }
}
