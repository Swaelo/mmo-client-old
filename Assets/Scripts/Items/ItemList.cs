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

    public ItemData[] GameItems;
    
    public ItemData GetItem(int ItemNumber)
    {
        return GameItems[ItemNumber - 1];
    }
}
