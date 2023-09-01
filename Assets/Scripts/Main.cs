using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public enum movTyp { regularAttack,quickAttack,powerAttack,defendAndHeal,buildFlourish}

public struct race 
{
    public int raceID;
    public string raceName;
    public int STR, AGL, END, SPD, LCK;
}
public struct character
{
    public int id;
    public string name;
    public int raceID, armorID, weaponID, STR, AGL, END, SPD, LCK, gold, LVL, EXP, maxHP;
    public character(int id)
    {
        this.id = id;
        name = string.Empty;
        raceID = 0; armorID = 0; weaponID = 0; 
        STR = 50; AGL = 50; END = 50; SPD = 50; LCK = 50;
        EXP = 0; maxHP = 0; gold = 0; LVL = 1;

    }
}

public struct opponent
{
    public int id;
    public string name;
    public int raceID, armorID, weaponID, STR, AGL, END, SPD, LCK, gold, rank, rewardXP, maxHP;

}

public class Main : MonoBehaviour
{
    public static Main instance;
    public WebRequests webRequest { private set; get; }
    public List<race> raceList { private set; get; }
    public List<opponent> opponentList { private set; get; }

    [Header("Panels")]
    public GameObject panelLogin;
    public GameObject panelCharacterCreation;
    public GameObject panelArenaMain;
    public GameObject panelBattle;

    [Header("Audio")]
    public AudioSource diceRoll;

    public CharacterCreator charCreator { private set; get; }
    public character currentCharacter;
    public int spendablePoints { private set; get; }
    public int maxSpendablePoints { private set; get; }

    public int currentID { private set; get; }

    // Callback reference
    Action<int> getRuleCallback = (itemInfo) => {
        print("getRuleCallback: " + itemInfo);
    };

    private void Awake()
    {
        if (Main.instance == null)
        {
            Main.instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        webRequest = GetComponent<WebRequests>();
        charCreator = panelCharacterCreation.GetComponent<CharacterCreator>();
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        fillRaceData();
        fillOpponentData();
        // StartCoroutine(webRequest.Register("Testchar"));
        // StartCoroutine(webRequest.GetRule("startingpointstospend", getRuleCallback));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeAllPanels()
    {
        panelLogin.SetActive(false);
        panelCharacterCreation.SetActive(false);
        panelArenaMain.SetActive(false);
        panelBattle.SetActive(false);
    }

    public void fillRaceData()
    {
        raceList = new List<race>();
        JSONArray readRaceData = new JSONArray();
        Action<JSONArray> getRaceCallback = (raceInfo) =>
        {
            readRaceData = raceInfo;

            for (int i = 0; i < readRaceData.Count; i++)
            {
                var raceItem = readRaceData[i].AsObject;
                race newRace = new race();
                int.TryParse(raceItem["raceID"], out newRace.raceID);
                int.TryParse(raceItem["STR"], out newRace.STR);
                int.TryParse(raceItem["AGL"], out newRace.AGL);
                int.TryParse(raceItem["END"], out newRace.END);
                int.TryParse(raceItem["SPD"], out newRace.SPD);
                int.TryParse(raceItem["LCK"], out newRace.LCK);
                newRace.raceName = raceItem["name"];
                raceList.Add(newRace);

                Debug.Log(string.Format("Race Data: {0},{1},{2},{3},{4},{5},{6},", 
                    newRace.raceID, newRace.raceName, newRace.STR,
                    newRace.AGL,newRace.END,newRace.SPD,newRace.LCK));
            }
        };

        StartCoroutine(webRequest.GetRaces(getRaceCallback));


    }

    public void fillOpponentData()
    {
        opponentList = new List<opponent>();
        JSONArray readOpponentData = new JSONArray();
        Action<JSONArray> getOpponentCallback = (opponentInfo) =>
        {
            readOpponentData = opponentInfo;

            for (int i = 0; i < readOpponentData.Count; i++)
            {
                var opponentItem = readOpponentData[i].AsObject;
                opponent newOpponent = new opponent();
                int.TryParse(opponentItem["raceID"], out newOpponent.raceID);
                int.TryParse(opponentItem["STR"], out newOpponent.STR);
                int.TryParse(opponentItem["AGL"], out newOpponent.AGL);
                int.TryParse(opponentItem["END"], out newOpponent.END);
                int.TryParse(opponentItem["SPD"], out newOpponent.SPD);
                int.TryParse(opponentItem["LCK"], out newOpponent.LCK);
                int.TryParse(opponentItem["rank"], out newOpponent.rank);
                int.TryParse(opponentItem["maxHP"], out newOpponent.maxHP);
                int.TryParse(opponentItem["rewardEXP"], out newOpponent.rewardXP);
                newOpponent.name = opponentItem["name"];
                opponentList.Add(newOpponent);

                Debug.Log(string.Format("Opponent Data: {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    newOpponent.raceID, newOpponent.name, newOpponent.STR,
                    newOpponent.AGL, newOpponent.END, newOpponent.SPD, newOpponent.LCK,
                    newOpponent.rank,newOpponent.maxHP,newOpponent.rewardXP));
            }
        };

        StartCoroutine(webRequest.GetOpponents(getOpponentCallback));
    }

    public void changeSpendablePoints(int amt)
    {
        spendablePoints += amt;
    }

    public void setStartingSpendingPoints(Action<int> callback)
    {
        StartCoroutine(webRequest.GetRule("startingpointstospend", callback));
    }

    public void setMaxSpendingPoints()
    {
        Action<int> callback = (maxPoints) => {
            maxSpendablePoints = maxPoints;
        };
        StartCoroutine(webRequest.GetRule("maxpointstospend", callback));
    }

    public void setSpendingPoints(int amt)
    {
        spendablePoints = amt;
    }

    public void setCurrentID(int newID)
    {
        currentID = newID;
    }
}
