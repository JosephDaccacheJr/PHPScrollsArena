using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseStats : MonoBehaviour
{
    public UIStatButton statSTR;
    public UIStatButton statAGL, statEND, statSPD, statLCK;

    public Text textPointsToSpend; 

    private void OnEnable()
    {
        SetStats();
    }

    public void SetStats()
    {
        textPointsToSpend.text = Main.instance.spendablePoints.ToString();
        statSTR.setStat(UIStatButton.statType.STR, Main.instance.currentCharacter.STR);
        statAGL.setStat(UIStatButton.statType.AGL, Main.instance.currentCharacter.AGL);
        statEND.setStat(UIStatButton.statType.END, Main.instance.currentCharacter.END);
        statSPD.setStat(UIStatButton.statType.SPD, Main.instance.currentCharacter.SPD);
        statLCK.setStat(UIStatButton.statType.LCK, Main.instance.currentCharacter.LCK);
    }
}
