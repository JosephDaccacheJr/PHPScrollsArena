using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class UIChallengeButton : MonoBehaviour
{

    public void challengeOpponent()
    {
        Main.instance.closeAllPanels();
        Main.instance.panelBattle.SetActive(true);
        BattleController.instance.StartBattle(1);
    }


}
