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
                case "MISS":
                    updateBattleInfo();
                    Invoke("EnemyMove", 1f);
                    break;
                case "WIN":
                    textOpponentHP.text = "DEAD";
                    popupVictory.SetActive(true);
                    break;

            }
        };
        Main.instance.diceRoll.Play();
        StartCoroutine(Main.instance.webRequest.battleCommand("playerAttack", _opponentID, battleCallback, moveType));
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
                case "MISS":
                case "HEAL":

                    updateBattleInfo();
                    break;
                case "WIN":
                    popupDefeat.SetActive(true);
                    textPlayerHP.text = "DEAD";
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
