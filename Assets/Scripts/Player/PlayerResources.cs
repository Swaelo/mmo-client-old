// ================================================================================================================================
// File:        PlayerResources.cs
// Description: Tracks the player characters current Health, Mana and Stamina values
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour
{
    //UI Components for displaying the players various resources levels
    private Image HealthBarDisplay;
    private Image ManaBarDisplay;
    private Image StaminaBarDisplay;
    private float MaxBarLength = 484;

    //Track the current resources levels for the player
    public float CurrentHealth = 100;
    public float MaxHealth = 100;
    public float CurrentMana = 100;
    public float MaxMana = 100;
    
    public float CurrentStamina = 100;
    public float MaxStamina = 100;
    //Define the values for how much easy different activity effects the players stamina value
    public float RunStaminaDrain = 25f;
    public float IdleStaminaReplenish = 10f;
    public float StaminaRegenDelay = 0.0f;

    //Is the player character currently running or not
    public bool Running = false;

    private void Awake()
    {
        HealthBarDisplay = GameObject.Find("Health Bar Display").GetComponent<Image>();
        ManaBarDisplay = GameObject.Find("Mana Bar Display").GetComponent<Image>();
        StaminaBarDisplay = GameObject.Find("Stamina Bar Display").GetComponent<Image>();
    }

    public void Update()
    {
        //Reduce the regen delay until we allowed to regen again
        if (StaminaRegenDelay > 0.0f)
            StaminaRegenDelay -= Time.deltaTime;
        //Drain the stamina if the player is running and they are allowed to run
        if (Running)
        {
            CurrentStamina -= RunStaminaDrain * Time.deltaTime;
            //If the player run out of stamina because of running, they are blocked from running for 1 second
            if(CurrentStamina <= 0.0f)
                StaminaRegenDelay = 1.0f;
        }
        //Otherwise, replenish it if there is any missing (if we are able to)
        else if (StaminaRegenDelay <= 0.0f && CurrentStamina < MaxStamina)
            CurrentStamina += IdleStaminaReplenish * Time.deltaTime;

        //Find the current percentage of stamine relative to the maximum stamina amount
        float StaminaFraction = CurrentStamina / MaxStamina;
        //Find StaminaPercentage of the MaxBarLength, and set that as the new y value of the stamina bar
        float NewBarLength = MaxBarLength * StaminaFraction;
        //Reset the stamina bars rect transform with the new bar length to be displayed
        RectTransform StaminaRect = StaminaBarDisplay.rectTransform;
        StaminaRect.sizeDelta = new Vector2(NewBarLength, StaminaRect.sizeDelta.y);
    }
}
