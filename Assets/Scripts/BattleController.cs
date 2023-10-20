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
    public GameObject popupVictory;
    public GameObject popupDefeat;
    public Text textPlayerHP;
    public Text textOpponentHP;
    public Text textPlayerName;
    public Text textOpponentName;
    public Text textPlayerFlourish;
    public Text textBattleDialog;
    public List<Button> attackButtons = new List<Button>();

    int _opponentID = 1;
    int _playerFlourish;
    int _playerHP;
    int _opponentHP;

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
        popupVictory.SetActive(false);
        popupDefeat.SetActive(false);

        textBattleDialog.text = "FIGHT!\n";
        setMoveButton(true);
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
        Debug.Log("Calling updateBattleInfo...");
        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            foreach (var bi in battleInfo)
            {
                Debug.Log("BI: " + bi.Key + ":" + bi.Value);
            }
            textPlayerHP.text = "HP: " + battleInfo["playerHP"].Value;
            textOpponentHP.text = "HP: " + battleInfo["opponentHP"].Value;
            textPlayerName.text = battleInfo["playerName"].Value;
            textOpponentName.text = battleInfo["opponentName"].Value;
            int.TryParse(battleInfo["playerFlourish"].Value, out _playerFlourish);
            textPlayerFlourish.text = "Flourishes: " + _playerFlourish;
        }; 
        StartCoroutine(Main.instance.webRequest.battleCommand("updateBattle", _opponentID, battleCallback));
    }
    
    public void makeAttack(int moveTypeNum)
    {
        movTyp moveType = (movTyp)moveTypeNum;

        if (moveType == movTyp.buildFlourish && _playerFlourish >= 3)
        {
            return;
        }

        setMoveButton(false);

        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            foreach (var bi in battleInfo)
            {

                Debug.Log("Attack result: " + bi.Key + ": " + bi.Value);
            }

            switch (battleInfo["RESULT"].Value)
            {
                case "HIT":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    textBattleDialog.text = "You rolled a [" + battleInfo["HITROLL"].Value +
                    "] against an AC of " + battleInfo["ENEMYAC"].Value +
                    " and hit for [" + battleInfo["DAMAGE"].Value + "] damage!\n"
                    + textBattleDialog.text;
                    break;
                case "MISS":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    textBattleDialog.text = "You rolled a [" + battleInfo["HITROLL"].Value +
                    "] against an AC of [" + battleInfo["ENEMYAC"].Value +
                    "] and missed!\n"
                    + textBattleDialog.text;
                    break;
                case "HEAL":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    textBattleDialog.text = "You defend and heals [" + battleInfo["HEALAMOUNT"].Value + "] HP.\n"
                    + textBattleDialog.text;
                    break;
                case "WIN":
                    textBattleDialog.text = "You defeat your opponent!\n"
                    + textBattleDialog.text;
                    textOpponentHP.text = "DEAD";
                    popupVictory.SetActive(true);
                    break;
                default:
                    setMoveButton(false);
                    Invoke("EnemyMove", 1f);
                    updateBattleInfo();
                    break;

            }
        };
        Main.instance.diceRoll.Play();
        StartCoroutine(Main.instance.webRequest.battleCommand("playerMove", _opponentID, battleCallback, moveType));
    }

    void EnemyMove()
    {
        movTyp moveType = movTyp.regularAttack;


        JSONArray battleInfo = new JSONArray();
        Action<JSONNode> battleCallback = (battleInfo) =>
        {
            foreach (var bi in battleInfo)
            {

                Debug.Log("Enemy move result: " + bi.Key + ": " + bi.Value);
            }
            switch (battleInfo["RESULT"].Value)
            {
                case "HIT":
                    updateBattleInfo();
                    textBattleDialog.text = "Opponent rolled a [" + battleInfo["HITROLL"].Value +
                    "] against an AC of [" + battleInfo["ENEMYAC"].Value +
                    "] and hit for [" + battleInfo["DAMAGE"].Value + "] damage!\n"
                    + textBattleDialog.text;
                    break;
                case "MISS":
                    updateBattleInfo();
                    textBattleDialog.text = "Opponent rolled a [" + battleInfo["HITROLL"].Value +
                    "] against an AC of [" + battleInfo["ENEMYAC"].Value +
                    "] and missed!\n"
                    + textBattleDialog.text;
                    break;
                case "HEAL":
                    textBattleDialog.text = "Opponent defends and heals [" + battleInfo["HEALAMOUNT"].Value + "] HP.\n"
                    + textBattleDialog.text;
                    updateBattleInfo();
                    break;
                case "WIN":
                    popupDefeat.SetActive(true);
                    textBattleDialog.text = "Opponent defeats you!\n"
                    + textBattleDialog.text;
                    textPlayerHP.text = "DEAD";
                    break;
                default:
                    updateBattleInfo();
                    break;
            }
        };

        StartCoroutine(Main.instance.webRequest.battleCommand("opponentMove", _opponentID, battleCallback, moveType));

        foreach (Button b in attackButtons)
        {
            setMoveButton(true);
        }
    }

    void setMoveButton(bool set)
    {
        foreach (Button b in attackButtons)
        {
            b.interactable = set;
        }
    }
}
