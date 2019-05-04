// ================================================================================================================================
// File:        ConsumableEffectsWizard.cs
// Description: Provides a new custom editor window used to create new consumable effect assets
// ================================================================================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class ConsumableEffectsWizard : EditorWindow
{
    //Temporarily values filled through the editor window sent into the new ConsumableData object once its created
    private string ConsumableEffectName = "";
    private int HealthValueEffect = 0;
    private int StaminaValueEffect = 0;
    private int ManaValueEffect = 0;

    //Define a new menu button to bring this editor window into focus incase it is somehow lost
    [MenuItem ("Window/Consumable Effects Wizard %#e")]
    static void Init() { EditorWindow.GetWindow(typeof(ConsumableEffectsWizard)); }

    private void OnGUI()
    {
        //Display the window title at the top
        GUILayout.BeginHorizontal();
        GUILayout.Label("Consumable Effects Wizard", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        //In the main body of the window, allow the option to change all of the consumables effect values
        ConsumableEffectName = EditorGUILayout.TextField("Consumable Effect Name", ConsumableEffectName);
        HealthValueEffect = EditorGUILayout.IntField("Health Value Change", HealthValueEffect);
        StaminaValueEffect = EditorGUILayout.IntField("Stamina Value Change", StaminaValueEffect);
        ManaValueEffect = EditorGUILayout.IntField("Mana Value Change", ManaValueEffect);

        //Finally give a button to finalize the process and create the new ConsumableData asset file
        if(GUILayout.Button("Complete"))
        {
            //Make sure that a name has been entered, and check to make sure all the values are no all zero
            bool NameValid = ConsumableEffectName != "";
            bool ValuesValid = HealthValueEffect != 0 || StaminaValueEffect != 0 || ManaValueEffect != 0;

            //Only finalize the asset creation if the values entered seem to be valid
            if(NameValid && ValuesValid)
            {
                //Create the new ConsumableData object and assign all of the entered values to it
                ConsumableData NewConsumableData = ScriptableObject.CreateInstance<ConsumableData>();
                NewConsumableData.EffectName = StringEditor.RemoveSpaces(ConsumableEffectName);
                NewConsumableData.HealthValueAdjustment = HealthValueEffect;
                NewConsumableData.StaminaValueAdjustment = StaminaValueEffect;
                NewConsumableData.ManaValueAdjustment = ManaValueEffect;

                //Save this as a new asset file in the project files
                string AssetName = "Assets/Consumables/" + NewConsumableData.EffectName + ".asset";
                AssetDatabase.CreateAsset(NewConsumableData, AssetName);
                AssetDatabase.SaveAssets();

                //Reset all the editor window values
                ConsumableEffectName = "";
                HealthValueEffect = 0;
                StaminaValueEffect = 0;
                ManaValueEffect = 0;
            }
        }
    }
}
