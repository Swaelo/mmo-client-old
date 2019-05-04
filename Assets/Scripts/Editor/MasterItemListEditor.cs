// ================================================================================================================================
// File:        MasterItemListEditor.cs
// Description: Manages the lists of items available in the game so far
// ================================================================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MasterItemListEditor : EditorWindow
{
    //Current list of consumable items, and fields for entering data to create brand new consumable items
    public ItemData ExistingConsumableItem;
    public ItemDataList ConsumableItemList;
    private int ConsumableDataIndex = 1;
    private ItemData CurrentConsumableData;
    private string NewConsumableItemName = "";
    private string NewConsumableDisplayName = "";
    private string NewConsumableDescription = "";
    private GameObject NewConsumablePickupPrefab = null;
    private Sprite NewConsumableIcon = null;
    private ConsumableData NewConsumableEffects = null;

    //List of equipment items and fields to make new ones
    public ItemData ExistingEquipmentItem;
    public ItemDataList EquipmentItemList;
    private int EquipmentDataIndex = 1;
    private ItemData CurrentEquipmentData;
    private string NewEquipmentItemName = "";
    private string NewEquipmentDisplayName = "";
    private string NewEquipmentDescription = "";
    private ItemType NewEquipmentType = ItemType.NULL;
    private EquipmentSlot NewEquipmentSlot = EquipmentSlot.NULL;
    private GameObject NewEquipmentPickupPrefab = null;
    private Sprite NewEquipmentIcon = null;

    //List of ability items and fields to make new ones
    public ItemData ExistingAbilityItem;
    public ItemDataList AbilityItemList;
    private int AbilityDataIndex = 1;
    private ItemData CurrentAbilityData;
    private string NewAbilityItemName = "";
    private string NewAbilityDisplayName = "";
    private string NewAbilityDescription = "";
    private GameObject NewAbilityPickupPrefab = null;
    private Sprite NewAbilityIcon = null;

    [MenuItem ("Window/Master Item List Editor %#e")]
    static void Init() { EditorWindow.GetWindow(typeof(MasterItemListEditor)); }

    void OnEnable()
    {
        //Try to load in any already existing item lists
        if (EditorPrefs.HasKey("ConsumableItemListPath"))
        {
            string ObjectPath = EditorPrefs.GetString("ConsumableItemListPath");
            ConsumableItemList = AssetDatabase.LoadAssetAtPath(ObjectPath, typeof(ItemDataList)) as ItemDataList;
        }
        if (EditorPrefs.HasKey("EquipmentItemListPath"))
        {
            string ObjectPath = EditorPrefs.GetString("EquipmentItemListPath");
            EquipmentItemList = AssetDatabase.LoadAssetAtPath(ObjectPath, typeof(ItemDataList)) as ItemDataList;
        }
        if (EditorPrefs.HasKey("AbilityItemListPath"))
        {
            string ObjectPath = EditorPrefs.GetString("AbilityItemListPath");
            AbilityItemList = AssetDatabase.LoadAssetAtPath(ObjectPath, typeof(ItemDataList)) as ItemDataList;
        }
    }

    void OnGUI()
    {
        //Show the window title at the top
        GUILayout.BeginHorizontal();
        GUILayout.Label("Master Item List Editor", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        //Combines all the lists together into 1 giant master item list with every single thing detailed within, exports it out into a text file storing all that data
        if(GUILayout.Button("Export Master List", GUILayout.ExpandWidth(false)))
        {
            //Create a brand new master list which will contain every item from every list we have
            List<ItemData> MasterItemList = new List<ItemData>();
            
            //Fill the new MasterItemList so it contains all the other lists combined
            foreach (ItemData Item in ConsumableItemList.ItemList)
                MasterItemList.Add(Item);
            foreach (ItemData Item in EquipmentItemList.ItemList)
                MasterItemList.Add(Item);
            foreach (ItemData Item in AbilityItemList.ItemList)
                MasterItemList.Add(Item);

            //Loop through the entire list, assigning a new ItemNumber value to each item as we go through
            for (int i = 0; i < MasterItemList.Count; i++)
                MasterItemList[i].ItemNumber = (i + 1);

            //Create a list of strings, each string being 1 line in the text file we are going to export
            List<string> FileLines = new List<string>();

            //Each line in the texture file details an item that exists inside the game
            foreach (ItemData Item in MasterItemList)
                FileLines.Add(Item.Name + ":" + Item.DisplayName + ":" + Item.Type + ":" + Item.Slot + ":" + Item.ItemNumber);

            //Cast the List of strings into a regular array, then write it all into a new text file
            string[] Lines = FileLines.ToArray();
            string FileName = "C:/mmo-client/Assets/Exports/MasterItemList.txt";
            System.IO.File.WriteAllLines(FileName, Lines);
        }

        //Loads in a previous master item list from a local text file
        

        ////Loads in a previous master item list from a local text file
        //if(GUILayout.Button("Import Master List", GUILayout.ExpandWidth(false)))
        //{
        //    //string[] FileLines = System.IO.File.ReadAllLines("C:/mmo-client/Assets/Exports/MasterItemList.txt");

        //    //foreach(string Line in FileLines)
        //    //{
        //    //    string[] LineSplit = Line.Split(':');
        //    //}
        //}

        //Consumable Item List Management
        GUILayout.Label("Consumable Items");
        if(ConsumableItemList == null)
        {
            if (GUILayout.Button("Create New Consumable ItemDataList", GUILayout.ExpandWidth(false)))
                ConsumableItemList = CreateNewList("Assets/Items/ConsumableItemDataList.asset", "ConsumableItemListPath");
        }
        else
        {
            if(GUILayout.Button("Open Consumable List", GUILayout.ExpandWidth(false)))
            {
                string AbsolutePath = EditorUtility.OpenFilePanel("Select Consumable Item List", "", "");
                if(AbsolutePath.StartsWith(Application.dataPath))
                {
                    string RelativePath = AbsolutePath.Substring(Application.dataPath.Length - "Assets".Length);
                    ConsumableItemList = AssetDatabase.LoadAssetAtPath(RelativePath, typeof(ItemDataList)) as ItemDataList;
                    if (ConsumableItemList == null)
                        ConsumableItemList.ItemList = new List<ItemData>();
                    if (ConsumableItemList)
                        EditorPrefs.SetString("ConsumableItemListPath", RelativePath);
                }
            }

            ExistingConsumableItem = EditorGUILayout.ObjectField("Existing Consumable Item Insert", ExistingConsumableItem, typeof(ItemData), false) as ItemData;
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                ConsumableItemList.ItemList.Add(ExistingConsumableItem);
                ExistingConsumableItem = null;
            }

            //Place buttons to move back and forth while navigating the list of consumable items
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (ConsumableDataIndex > 1)
                    ConsumableDataIndex--;
            }
            GUILayout.Space(5);
            if(GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (ConsumableDataIndex < ConsumableItemList.ItemList.Count)
                    ConsumableDataIndex++;
            }
            GUILayout.EndHorizontal();

            //Navigate through the items in the consumables list if it has any in it
            if(ConsumableItemList.ItemList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                ConsumableDataIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Consumable ItemData", ConsumableDataIndex, GUILayout.ExpandWidth(false)), 1, ConsumableItemList.ItemList.Count);
                EditorGUILayout.LabelField("of " + ConsumableItemList.ItemList.Count.ToString() + " items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                ItemData CurrentConsumableItemData = ConsumableItemList.ItemList[ConsumableDataIndex - 1];
                GUILayout.BeginHorizontal();
                CurrentConsumableItemData.Name = EditorGUILayout.TextField("Current Consumable Name", CurrentConsumableItemData.Name as string);
                CurrentConsumableItemData.DisplayName = EditorGUILayout.TextField("Current Consumable Display Name", CurrentConsumableItemData.DisplayName as string);
                CurrentConsumableItemData.Description = EditorGUILayout.TextField("Current Consumable Description", CurrentConsumableItemData.Description as string);
                CurrentConsumableItemData.Type = (ItemType)EditorGUILayout.EnumPopup("Current Consumable Item Type", CurrentConsumableItemData.Type);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                CurrentConsumableItemData.Prefab = EditorGUILayout.ObjectField("Current Consumable Pickup Prefab", CurrentConsumableItemData.Prefab, typeof(GameObject), false) as GameObject;
                CurrentConsumableItemData.Icon = EditorGUILayout.ObjectField("Current Consumable Icon", CurrentConsumableItemData.Icon, typeof(Sprite), false) as Sprite;
                CurrentConsumableItemData.ConsumableEffect = EditorGUILayout.ObjectField("Current Consumable Effects", CurrentConsumableItemData.ConsumableEffect, typeof(ConsumableData), false) as ConsumableData;
                GUILayout.EndHorizontal();
            }

            //Allow the creation of brand new consumable items that get stored straight into the consumable item data list with the others
            GUILayout.BeginHorizontal();
            NewConsumableItemName = EditorGUILayout.TextField("New Consumable Name", NewConsumableItemName as string);
            NewConsumableDisplayName = EditorGUILayout.TextField("New Consumable Display Name", NewConsumableDisplayName as string);
            NewConsumableDescription = EditorGUILayout.TextField("New Consumable Description", NewConsumableDescription as string);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            NewConsumablePickupPrefab = EditorGUILayout.ObjectField("New Consumable Pickup Prefab", NewConsumablePickupPrefab, typeof(GameObject), false) as GameObject;
            NewConsumableIcon = EditorGUILayout.ObjectField("New Consumable Icon", NewConsumableIcon, typeof(Sprite), false) as Sprite;
            NewConsumableEffects = EditorGUILayout.ObjectField("New Consumable Effects", NewConsumableEffects, typeof(ConsumableData), false) as ConsumableData;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create New Consumable", GUILayout.ExpandWidth(false)))
            {
                //Create the new consumable itemdata object and store all the relevant information inside it
                ItemData NewConsumableItem = ScriptableObject.CreateInstance<ItemData>();
                NewConsumableItem.Name = NewConsumableItemName;
                NewConsumableItem.DisplayName = NewConsumableDisplayName;
                NewConsumableItem.Description = NewConsumableDescription;
                NewConsumableItem.Prefab = NewConsumablePickupPrefab;
                NewConsumableItem.Icon = NewConsumableIcon;
                NewConsumableItem.ConsumableEffect = NewConsumableEffects;
                NewConsumableItem.Type = ItemType.Consumable;

                //Save this as a new asset in the project directory
                string AssetName = "Assets/Items/Consumables/" + NewConsumableItem.Name + ".asset";
                AssetDatabase.CreateAsset(NewConsumableItem, AssetName);
                AssetDatabase.SaveAssets();

                //Place this in the list with the rest of the consumable items
                ConsumableItemList.ItemList.Add(NewConsumableItem);

                //Empty all the input fields
                NewConsumableItemName = "";
                NewConsumableDisplayName = "";
                NewConsumableDescription = "";
                NewConsumablePickupPrefab = null;
                NewConsumableIcon = null;
                NewConsumableEffects = null;
            }
        }

        //Equipment Item List Management
        GUILayout.Space(15);
        GUILayout.Label("Equipment Items");
        if(EquipmentItemList == null)
        {
            if (GUILayout.Button("Create New Equipment ItemDataList", GUILayout.ExpandWidth(false)))
                EquipmentItemList = CreateNewList("Assets/Items/EquipmentItemDataList.asset", "EquipmentItemListPath");
        }
        else
        {
            if (GUILayout.Button("Open Equipment List", GUILayout.ExpandWidth(false)))
            {
                string AbsolutePath = EditorUtility.OpenFilePanel("Select Equipment Item List", "", "");
                if (AbsolutePath.StartsWith(Application.dataPath))
                {
                    string RelativePath = AbsolutePath.Substring(Application.dataPath.Length - "Assets".Length);
                    EquipmentItemList = AssetDatabase.LoadAssetAtPath(RelativePath, typeof(ItemDataList)) as ItemDataList;
                    if (EquipmentItemList == null)
                        EquipmentItemList.ItemList = new List<ItemData>();
                    if (EquipmentItemList)
                        EditorPrefs.SetString("EquipmentItemListPath", RelativePath);
                }
            }

            ExistingEquipmentItem = EditorGUILayout.ObjectField("Existing Equipment Item Insert", ExistingEquipmentItem, typeof(ItemData), false) as ItemData;
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                EquipmentItemList.ItemList.Add(ExistingEquipmentItem);
                ExistingEquipmentItem = null;
            }

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (EquipmentDataIndex > 1)
                    EquipmentDataIndex--;
            }
            GUILayout.Space(5);
            if(GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (EquipmentDataIndex < EquipmentItemList.ItemList.Count)
                    EquipmentDataIndex++;
            }
            GUILayout.EndHorizontal();

            //Navigate through the items in the equipments list if it has any in it
            if(EquipmentItemList.ItemList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                EquipmentDataIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Equipment ItemData", EquipmentDataIndex, GUILayout.ExpandWidth(false)), 1, EquipmentItemList.ItemList.Count);
                EditorGUILayout.LabelField("of " + EquipmentItemList.ItemList.Count.ToString() + " items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                ItemData CurrentEquipmentItemData = EquipmentItemList.ItemList[EquipmentDataIndex - 1];
                GUILayout.BeginHorizontal();
                CurrentEquipmentItemData.Name = EditorGUILayout.TextField("Current Equipment Name", CurrentEquipmentItemData.Name as string);
                CurrentEquipmentItemData.DisplayName = EditorGUILayout.TextField("Current Equipment Display Name", CurrentEquipmentItemData.DisplayName as string);
                CurrentEquipmentItemData.Description = EditorGUILayout.TextField("Current Equipment Description", CurrentEquipmentItemData.Description as string);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                CurrentEquipmentItemData.Type = (ItemType)EditorGUILayout.EnumPopup("Current Equipment Item Type", CurrentEquipmentItemData.Type);
                CurrentEquipmentItemData.Prefab = EditorGUILayout.ObjectField("Current Consumable Pickup Prefab", CurrentEquipmentItemData.Prefab, typeof(GameObject), false) as GameObject;
                CurrentEquipmentItemData.Icon = EditorGUILayout.ObjectField("Current Consumable Icon", CurrentEquipmentItemData.Icon, typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();
            }

            //Allow the creation of brand new equipment items that get stored straight into the equipments item data list with the others
            GUILayout.BeginHorizontal();
            NewEquipmentItemName = EditorGUILayout.TextField("New Equipment Name", NewEquipmentItemName as string);
            NewEquipmentDisplayName = EditorGUILayout.TextField("New Equipment Display Name", NewEquipmentDisplayName as string);
            NewEquipmentDescription = EditorGUILayout.TextField("New Equipment Description", NewEquipmentDescription as string);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            NewEquipmentType = (ItemType)EditorGUILayout.EnumPopup("New Equipment Item Type", NewEquipmentType);
            NewEquipmentSlot = (EquipmentSlot)EditorGUILayout.EnumPopup("New Equipment Item Slot", NewEquipmentSlot);
            NewEquipmentPickupPrefab = EditorGUILayout.ObjectField("New Equipment Pickup Prefab", NewEquipmentPickupPrefab, typeof(GameObject), false) as GameObject;
            NewEquipmentIcon = EditorGUILayout.ObjectField("New Equipment Icon", NewEquipmentIcon, typeof(Sprite), false) as Sprite;
            GUILayout.EndHorizontal();

            if(GUILayout.Button("Create New Equipment", GUILayout.ExpandWidth(false)))
            {
                //Create the new equipment ItemData object and store all the relevant information inside it
                ItemData NewEquipmentItem = ScriptableObject.CreateInstance<ItemData>();
                NewEquipmentItem.Name = NewEquipmentItemName;
                NewEquipmentItem.DisplayName = NewEquipmentDisplayName;
                NewEquipmentItem.Description = NewEquipmentDescription;
                NewEquipmentItem.Type = NewEquipmentType;
                NewEquipmentItem.Prefab = NewEquipmentPickupPrefab;
                NewEquipmentItem.Icon = NewEquipmentIcon;
                NewEquipmentItem.Slot = NewEquipmentSlot;

                //Save this as a new asset in the project
                string AssetName = "Assets/Items/Equipments/" + NewEquipmentItem.Name + ".asset";
                AssetDatabase.CreateAsset(NewEquipmentItem, AssetName);
                AssetDatabase.SaveAssets();

                EquipmentItemList.ItemList.Add(NewEquipmentItem);

                //Empty all the input fields
                NewEquipmentItemName = "";
                NewEquipmentDisplayName = "";
                NewEquipmentDescription = "";
                NewEquipmentSlot = EquipmentSlot.NULL;
                NewEquipmentType = ItemType.NULL;
                NewEquipmentPickupPrefab = null;
                NewEquipmentIcon = null;
            }
        }

        //Ability Item List Management
        GUILayout.Space(15);
        GUILayout.Label("Ability Items");
        if(AbilityItemList == null)
        {
            if (GUILayout.Button("Create New Ability ItemDataList", GUILayout.ExpandWidth(false)))
                AbilityItemList = CreateNewList("Assets/Items/AbilityItemDataList.asset", "AbilityItemListPath");
        }
        else
        {
            if (GUILayout.Button("Open Ability List", GUILayout.ExpandWidth(false)))
            {
                string AbsolutePath = EditorUtility.OpenFilePanel("Select Ability Item List", "", "");
                if (AbsolutePath.StartsWith(Application.dataPath))
                {
                    string RelativePath = AbsolutePath.Substring(Application.dataPath.Length - "Assets".Length);
                    AbilityItemList = AssetDatabase.LoadAssetAtPath(RelativePath, typeof(ItemDataList)) as ItemDataList;
                    if (AbilityItemList == null)
                        AbilityItemList.ItemList = new List<ItemData>();
                    if (AbilityItemList)
                        EditorPrefs.SetString("AbilityItemListPath", RelativePath);
                }
            }

            ExistingAbilityItem = EditorGUILayout.ObjectField("Existing Ability Item Insert", ExistingAbilityItem, typeof(ItemData), false) as ItemData;
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                AbilityItemList.ItemList.Add(ExistingAbilityItem);
                ExistingAbilityItem = null;
            }

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (AbilityDataIndex > 1)
                    AbilityDataIndex--;
            }
            GUILayout.Space(5);
            if(GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (AbilityDataIndex < AbilityItemList.ItemList.Count)
                    AbilityDataIndex++;
            }
            GUILayout.EndHorizontal();

            //Navigate through the items in the abilitys list if it has any in it
            if(AbilityItemList.ItemList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                AbilityDataIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Ability ItemData", AbilityDataIndex, GUILayout.ExpandWidth(false)), 1, AbilityItemList.ItemList.Count);
                EditorGUILayout.LabelField("of " + AbilityItemList.ItemList.Count.ToString() + " items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                ItemData CurrentAbilityItemData = AbilityItemList.ItemList[AbilityDataIndex - 1];
                GUILayout.BeginHorizontal();
                CurrentAbilityItemData.Name = EditorGUILayout.TextField("Current Ability Name", CurrentAbilityItemData.Name as string);
                CurrentAbilityItemData.DisplayName = EditorGUILayout.TextField("Current Ability Display Name", CurrentAbilityItemData.DisplayName as string);
                CurrentAbilityItemData.Description = EditorGUILayout.TextField("Current Ability Description", CurrentAbilityItemData.Description as string);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                CurrentAbilityItemData.Type = (ItemType)EditorGUILayout.EnumPopup("Current Ability Item Type", CurrentAbilityItemData.Type);
                CurrentAbilityItemData.Prefab = EditorGUILayout.ObjectField("Current Ability Pickup Prefab", CurrentAbilityItemData.Prefab, typeof(GameObject), false) as GameObject;
                CurrentAbilityItemData.Icon = EditorGUILayout.ObjectField("Current Ability Icon", CurrentAbilityItemData.Icon, typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();
            }

            //Allow the creation of brand new equipment items that get stored straight into the equipments item data list with the others
            GUILayout.BeginHorizontal();
            NewAbilityItemName = EditorGUILayout.TextField("New Ability Name", NewAbilityItemName as string);
            NewAbilityDisplayName = EditorGUILayout.TextField("New Ability Display Name", NewAbilityDisplayName as string);
            NewAbilityDescription = EditorGUILayout.TextField("New Ability Description", NewAbilityDescription as string);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            NewAbilityPickupPrefab = EditorGUILayout.ObjectField("New Ability Pickup Prefab", NewAbilityPickupPrefab, typeof(GameObject), false) as GameObject;
            NewAbilityIcon = EditorGUILayout.ObjectField("New Ability Icon", NewAbilityIcon, typeof(Sprite), false) as Sprite;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create New Ability", GUILayout.ExpandWidth(false)))
            {
                //Create the new equipment ItemData object and store all the relevant information inside it
                ItemData NewAbilityItem = ScriptableObject.CreateInstance<ItemData>();
                NewAbilityItem.Name = NewAbilityItemName;
                NewAbilityItem.DisplayName = NewAbilityDisplayName;
                NewAbilityItem.Description = NewAbilityDescription;
                NewAbilityItem.Prefab = NewAbilityPickupPrefab;
                NewAbilityItem.Icon = NewAbilityIcon;
                NewAbilityItem.Type = ItemType.AbilityGem;

                //Save this as a new asset in the project
                string AssetName = "Assets/Items/Abilities/" + NewAbilityItem.Name + ".asset";
                AssetDatabase.CreateAsset(NewAbilityItem, AssetName);
                AssetDatabase.SaveAssets();

                AbilityItemList.ItemList.Add(NewAbilityItem);

                //Empty all the input fields
                NewAbilityItemName = "";
                NewAbilityDisplayName = "";
                NewAbilityDescription = "";
                NewAbilityPickupPrefab = null;
                NewAbilityIcon = null;
            }
        }
    }

    //Creates a brand new ItemDataList asset file in the project directory
    private ItemDataList CreateNewList(string AssetName, string PathKey)
    {
        ItemDataList NewList = ScriptableObject.CreateInstance<ItemDataList>();
        AssetDatabase.CreateAsset(NewList, AssetName);
        AssetDatabase.SaveAssets();
        NewList.ItemList = new List<ItemData>();
        string RelativePath = AssetDatabase.GetAssetPath(NewList);
        EditorPrefs.SetString(PathKey, RelativePath);
        return NewList;
    }
}
