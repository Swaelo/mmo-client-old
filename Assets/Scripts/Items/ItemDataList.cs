// ================================================================================================================================
// File:        ItemDataList.cs
// Description: Stores a complete list of all the game items that have currently been defined so far, allows reordering and updating the list
// ================================================================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDataList : ScriptableObject
{
    public List<ItemData> ItemList;
}
