using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatTypes
{
    Strength = 1,
    Agility = 2,
    Stamina = 3,
    Intelligence = 4
}

public class CharacterCreation : MonoBehaviour
{
    public int Strength = 8;
    public Text StrengthTextUI;
    public void ClickStrengthUp() { UpdateStat(StatTypes.Strength, 1); }
    public void ClickStrengthDown() { UpdateStat(StatTypes.Strength, -1); }

    public int Agility = 8;
    public Text AgilityTextUI;
    public void ClickAgilityUp() { UpdateStat(StatTypes.Agility, 1); }
    public void ClickAgilityDown() { UpdateStat(StatTypes.Agility, -1); }

    public int Stamina = 8;
    public Text StaminaTextUI;
    public void ClickStaminaUp() { UpdateStat(StatTypes.Stamina, 1); }
    public void ClickStaminaDown() { UpdateStat(StatTypes.Stamina, -1); }

    public int Intelligence = 8;
    public Text IntelligenceTextUI;
    public void ClickIntelligenceUp() { UpdateStat(StatTypes.Intelligence, 1); }
    public void ClickIntelligenceDown() { UpdateStat(StatTypes.Intelligence, -1); }

    public int StatPointsLeft = 10;
    public Text StatsLeftTextUI;

    private void UpdateStat(StatTypes Type, int Change)
    {
        //Do not allow positive stat changes when there are no points left to assign
        if (Change == 1 && StatPointsLeft == 0)
            return;

        switch(Type)
        {
            case (StatTypes.Strength):
                //Do not allow stats to be decreased below 8
                if (Change == -1 && Strength == 8)
                    return;
                //Update the stats and the UI display
                StatPointsLeft -= Change;
                Strength += Change;
                StrengthTextUI.text = "Strength: " + Strength;
                StatsLeftTextUI.text = StatPointsLeft + " points remaining";
                return;
            case (StatTypes.Agility):
                if (Change == -1 && Agility == 8)
                    return;
                StatPointsLeft -= Change;
                Agility += Change;
                AgilityTextUI.text = "Agility: " + Agility;
                StatsLeftTextUI.text = StatPointsLeft + " points remaining";
                return;
            case (StatTypes.Stamina):
                if (Change == -1 && Stamina == 8)
                    return;
                StatPointsLeft -= Change;
                Stamina += Change;
                StaminaTextUI.text = "Stamina: " + Stamina;
                StatsLeftTextUI.text = StatPointsLeft + " points remaining";
                return;
            case (StatTypes.Intelligence):
                if (Change == -1 && Intelligence == 8)
                    return;
                StatPointsLeft -= Change;
                Intelligence += Change;
                IntelligenceTextUI.text = "Intelligence: " + Intelligence;
                StatsLeftTextUI.text = StatPointsLeft + " points remaining";
                return;
        }
    }
}
