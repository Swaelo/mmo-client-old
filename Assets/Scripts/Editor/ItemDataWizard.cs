// ================================================================================================================================
// File:        ItemDataWizard.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class ItemDataWizard : EditorWindow
{
    //These are the values which will be set through the inspector for the new ItemData
    private string NewItemName = "";    //This will be the new items ingame display name
    private string NewItemDisplayName = ""; 
    private string NewItemDescription = "";
    private ItemType NewItemType = ItemType.NULL;   //Define what type of item this is
    private EquipmentSlot NewItemSlot = EquipmentSlot.NULL; //If this is an equipment item define which slot it can be equipped to
    private GameObject NewItemPrefab = null;    //Used to display the item in the game world
    private Sprite NewItemIcon = null;  //Used to display the item on the user interface

    //Make a new menu button which brings the editor window into focus incase it gets lost
    [MenuItem ("Window/ItemData Wizard %#e")]
    static void Init() { EditorWindow.GetWindow(typeof(ItemDataWizard)); }

    void OnGUI()
    {
        //Display the window title at the top
        GUILayout.BeginHorizontal();
        GUILayout.Label("ItemData Wizard", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        //With the main body of the window, give to option to set all of the ItemData values
        NewItemName = EditorGUILayout.TextField("Item Name", NewItemName);
        NewItemDisplayName = EditorGUILayout.TextField("Item Display Name", NewItemDisplayName);
        NewItemDescription = EditorGUILayout.TextField("Item Description", NewItemDescription);
        NewItemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", NewItemType);
        NewItemSlot = (EquipmentSlot)EditorGUILayout.EnumPopup("Item Slot", NewItemSlot);
        NewItemPrefab = EditorGUILayout.ObjectField("Item Prefab", NewItemPrefab, typeof(GameObject), false) as GameObject;
        NewItemIcon = EditorGUILayout.ObjectField("Item Icon", NewItemIcon, typeof(Sprite), false) as Sprite;

        //Then give a button to finish and have the wizard do the rest
        if(GUILayout.Button("Complete"))
        {
            //Make sure every field in the editor window has been filled out correctly
            bool ItemNameValid = NewItemName != "";
            bool ItemDisplayNameValid = NewItemDisplayName != "";
            bool ItemDescriptionValid = NewItemDescription != "";
            bool ItemTypeValid = NewItemType != ItemType.NULL;
            bool ItemPrefabValid = NewItemPrefab != null;
            bool ItemIconValid = NewItemIcon != null;

            //If everything is filled out correctly then create the new asset file to store all of the information that was entered into the table
            if(ItemNameValid && ItemDisplayNameValid && ItemDescriptionValid && ItemTypeValid && ItemPrefabValid && ItemIconValid)
            {
                //Create the new ItemData object and assign all of the entered values over to it
                ItemData NewItemData = ScriptableObject.CreateInstance<ItemData>();
                //Trim any white space from the item names before creating them as the asset filename cant have spaces in it to work right
                NewItemData.Name = StringEditor.RemoveSpaces(NewItemName);
                NewItemData.DisplayName = NewItemDisplayName;
                NewItemData.Description = NewItemDescription;
                NewItemData.Type = NewItemType;
                NewItemData.Slot = NewItemSlot;
                NewItemData.Prefab = NewItemPrefab;
                NewItemData.Icon = NewItemIcon;

                //Save this as a new asset in the project directory
                string AssetName = "Assets/Items/" + NewItemData.Name + ".asset";
                AssetDatabase.CreateAsset(NewItemData, AssetName);
                AssetDatabase.SaveAssets();

                //Reset all the editor window fields
                NewItemName = "";
                NewItemDisplayName = "";
                NewItemDescription = "";
                NewItemType = ItemType.NULL;
                NewItemSlot = EquipmentSlot.NULL;
                NewItemPrefab = null;
                NewItemIcon = null;
            }
        }
    }
}