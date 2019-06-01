// ================================================================================================================================
// File:        ItemList.cs
// Description: Lists every item available in the game, has some helper functions to find out other information about an item with only its ItemNumber
// ================================================================================================================================

using UnityEngine;
using System;
using System.Collections.Generic;

public class ItemList : MonoBehaviour
{
    //Make this class a singleton so it can easily be accessed from anywhere in the code
    public static ItemList Instance = null;

    //Complete list of all ItemData assets created for the game so far
    public List<ItemData> FullItemList = new List<ItemData>();

    //All the ItemData assets sorted into a dictionary, index by their ItemNumber value (taken from importing master item list file)
    public Dictionary<int, ItemData> FullItemDictionary = new Dictionary<int, ItemData>();

    //Loads in all the data from the master item list text file, organize items from the FullItemList into the FullItemDictionary by combining the ItemData with what was read from the text file
    private void Awake()
    {
        //Assign our singleton class instance
        Instance = this;

        //Import the master item list file listening every item and its ItemNumber values
        string[] FileLines = System.IO.File.ReadAllLines(Application.dataPath + "/Exports/MasterItemList.txt");
        Log.PrintChatMessage("Reading " + FileLines.Length + " lines in the MasterItemList file");
        //string[] FileLines = System.IO.File.ReadAllLines("C:/Users/Harley/Desktop/mmo-client/Assets/Exports/MasterItemList.txt");

        //Look through and process each line of the text file
        foreach (string FileLine in FileLines)
        {
            //Split apart each line of the file to seperate the items information apart
            string[] LineSplit = FileLine.Split(':');

            //Each line should split into 5 parts, being the Name, DisplayName, Type, Slot and ItemNumber values
            string ItemName = LineSplit[0];
            string ItemDisplayName = LineSplit[1];
            ItemType ItemType = FindItemType(LineSplit[2]);
            EquipmentSlot ItemSlot = FindItemSlot(LineSplit[3]);
            int ItemNumber = Int32.Parse(LineSplit[4]);

            //Fetch the relevant items ItemData object from the FullItemList array, using the ItemName to find it
            ItemData ItemData = GetItemData(ItemName);
            //Update the items ItemNumber value, then store it into the dictionary with the ItemNumber as its key
            ItemData.ItemNumber = ItemNumber;
            FullItemDictionary.Add(ItemNumber, ItemData);
        }
    }

    //Returns the correct item type value from the partial string read out from the file import
    public ItemType FindItemType(string ItemTypeValue)
    {
        if (ItemTypeValue == "Consumable")
            return ItemType.Consumable;
        else if (ItemTypeValue == "Equipment")
            return ItemType.Equipment;
        else if (ItemTypeValue == "AbilityGem")
            return ItemType.AbilityGem;

        return ItemType.NULL;
    }

    //Returns the correct equipment slot value from the partial string read out from the file importing
    public EquipmentSlot FindItemSlot(string ItemSlotValue)
    {
        if (ItemSlotValue == "Head")
            return EquipmentSlot.Head;
        else if (ItemSlotValue == "Back")
            return EquipmentSlot.Back;
        else if (ItemSlotValue == "Neck")
            return EquipmentSlot.Neck;
        else if (ItemSlotValue == "LeftShoulder")
            return EquipmentSlot.LeftShoulder;
        else if (ItemSlotValue == "RightShoulder")
            return EquipmentSlot.RightShoulder;
        else if (ItemSlotValue == "Chest")
            return EquipmentSlot.Chest;
        else if (ItemSlotValue == "LeftGlove")
            return EquipmentSlot.LeftGlove;
        else if (ItemSlotValue == "RightGlove")
            return EquipmentSlot.RightGlove;
        else if (ItemSlotValue == "Legs")
            return EquipmentSlot.Legs;
        else if (ItemSlotValue == "LeftHand")
            return EquipmentSlot.LeftHand;
        else if (ItemSlotValue == "RightHand")
            return EquipmentSlot.RightHand;
        else if (ItemSlotValue == "LeftFoot")
            return EquipmentSlot.LeftFoot;
        else if (ItemSlotValue == "RightFoot")
            return EquipmentSlot.RightFoot;

        return EquipmentSlot.NULL;
    }

    //Retrieves an ItemData object from the FullItemList array using the ItemName to find it
    public ItemData GetItemData(string ItemName)
    {
        //Loop through the FullItemList, checking each items name as we go along
        foreach(ItemData Item in FullItemList)
        {
            //If this ItemData has the matching ItemName then we have found what we were searching for
            if (Item.Name == ItemName)
                return Item;
        }

        return null;
    }

    //Retrieves an ItemData object from the dictionary using its ItemNumber value to find it
    public ItemData GetItemData(int ItemNumber)
    {
        if (!FullItemDictionary.ContainsKey(ItemNumber))
        {
            Log.PrintChatMessage("ItemList.GetItemData(" + ItemNumber + "), dictionary does not contain key, returning null.");
            return null;
        }

        return FullItemDictionary[ItemNumber];
    }

    //Retrieves an items Name from the dictionary using its ItemNumber value to find it
    public string GetItemName(int ItemNumber)
    {
        if (!FullItemDictionary.ContainsKey(ItemNumber))
            return null;

        return FullItemDictionary[ItemNumber].Name;
    }

    //Retrievers an items ItemType value
    public ItemType GetItemType(int ItemNumber)
    {
        if (!FullItemDictionary.ContainsKey(ItemNumber))
            return ItemType.NULL;

        return FullItemDictionary[ItemNumber].Type;
    }
}