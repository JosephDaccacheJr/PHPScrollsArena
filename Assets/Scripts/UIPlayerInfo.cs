using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class UIPlayerInfo : MonoBehaviour
{
    public Text textPlayerName, textPlayerLevel, textPlayerGold;

    
    public void updateInfo(string strName,string strLVL, string strGold)
    {
        textPlayerName.text = Main.instance.currentCharacter.name;
        textPlayerLevel.text = "LVL: " + Main.instance.currentCharacter.LVL;
        textPlayerGold.text = "GP: " + Main.instance.currentCharacter.gold;
    }
}
