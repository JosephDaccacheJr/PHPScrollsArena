using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIStatButton : MonoBehaviour
{
    public enum statType { STR,AGL,END,SPD,LCK};
    public statType stat;
    public Text textStatName, textStatAmt;

    public void setStat(statType type, int amt)
    {
        textStatAmt.text = amt.ToString();
        textStatName.text = type.ToString();
    }

    public void OnStatChangeClicked(int amt)
    {

        if ((amt > 0 && Main.instance.spendablePoints <= 0) || 
            (amt < 0 && Main.instance.spendablePoints >= Main.instance.maxSpendablePoints))
        {
            return;
        }
        Main.instance.changeSpendablePoints(-amt);
        switch (stat)
        {
            case statType.STR:
                Main.instance.currentCharacter.STR += amt;
                break;
            case statType.AGL:
                Main.instance.currentCharacter.AGL += amt;
                break;
            case statType.END:
                Main.instance.currentCharacter.END += amt;
                break;
            case statType.SPD:
                Main.instance.currentCharacter.SPD += amt;
                break;
            case statType.LCK:
                Main.instance.currentCharacter.LCK += amt;
                break;
        }

        Main.instance.charCreator.chooseStats.SetStats();
    }
}
