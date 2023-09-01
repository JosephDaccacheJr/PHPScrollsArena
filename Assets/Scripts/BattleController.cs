using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class BattleController : MonoBehaviour
{
    public WebRequests webRequest { private set; get; }
    public static BattleController instance;

    [Header("UI Elements")]
    public Text textPlayerHP;
    public Text textOpponentHP;
    public Text textPlayerName;
    public Text textOpponentName;
    public List<Button> attackButtons = new List<Button>();

    int _opponentID = 1;
    int _playerFlourish;

    private void Awake()
    {
        if(BattleController.instance == null)
        {
            BattleController.instance = this;
        }
    }

    private void Start()
    {
        webRequest = Main.instance.webRequest;
    }
    
    public void StartBattle(int oppID)
    {
        _opponentID = oppID;
        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            updateBattleInfo();
        };
        StartCoroutine(Main.instance.webRequest.battleCommand("startBattle", _opponentID, battleCallback));
    }

    public void updateBattleInfo()
    {
        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            textPlayerHP.text = battleInfo["playerHP"].Value;
            textOpponentHP.text = battleInfo["opponentHP"].Value;
            textPlayerName.text = battleInfo["playerName"].Value;
            textOpponentName.text = battleInfo["opponentName"].Value;
            int.TryParse(battleInfo["playerFlourish"].Value, out _playerFlourish);
        }; 
        StartCoroutine(Main.instance.webRequest.battleCommand("updateBattle", _opponentID, battleCallback));
    }
    
    public void makeAttack(int moveTypeNum)
    {
        movTyp moveType = (movTyp)moveTypeNum;

        foreach (Button b in attackButtons)
        {
            b.interactable = false;
        }
        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            foreach (var bi in battleInfo)
            {

                Debug.Log("Attack result: " + bi.Key + ": " + bi.Value);
            }
            switch(battleInfo["RESULT"].Value)
            {
                case "HIT":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    break;
                case "MISS":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    break;
                case "WIN":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    break;
            }
        };
        Main.instance.diceRoll.Play();
        StartCoroutine(Main.instance.webRequest.battleCommand("playerAttack", _opponentID, battleCallback, moveType));
    }

    void EnemyMove()
    {
        foreach (Button b in attackButtons)
        {
            b.interactable = true;
        }
    }
}
