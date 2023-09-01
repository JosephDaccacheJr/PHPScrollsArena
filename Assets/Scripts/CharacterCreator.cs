using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelRaceSelection;
    public GameObject panelStatSelection;

    [Header("Race Selection References")]
    public GameObject raceButtonPrefab;
    public GameObject raceButtonHolder;

    [Header("Stat Setting References")]
    public ChooseStats chooseStats;
    List<GameObject> raceButtonList = new List<GameObject>();


    private void OnEnable()
    {
     //   StartCharacterCreation();
        
    }

    public void StartCharacterCreation(int newID)
    {
        if (raceButtonList.Count <= 0)
        {
            foreach (race r in Main.instance.raceList)
            {
                GameObject newRace = Instantiate(raceButtonPrefab, raceButtonHolder.transform);
                string raceInfo =
                    (r.STR > 0 ? " STR: +" + r.STR.ToString().PadRight(2) : "") +
                    (r.AGL > 0 ? " AGL: +" + r.AGL.ToString().PadRight(2) : "") +
                    (r.END > 0 ? " END: +" + r.END.ToString().PadRight(2) : "") +
                    (r.SPD > 0 ? " SPD: +" + r.SPD.ToString().PadRight(2) : "") +
                    (r.LCK > 0 ? " LCK: +" + r.LCK.ToString().PadRight(2) : "");

                UIRaceButton urb = newRace.GetComponent<UIRaceButton>();
                urb.textRaceName.text = r.raceName;
                urb.textRaceStats.text = raceInfo;
                urb.raceID = r.raceID;
                raceButtonList.Add(newRace);
            }
        }
        Debug.Log("Creating new character with ID " + newID);
        Main.instance.currentCharacter = new character(newID);
    }

    public void clearAllPanels()
    {
        panelRaceSelection.SetActive(false);
        panelStatSelection.SetActive(false);
    }

    public void SetRace(int id)
    {
        Main.instance.currentCharacter.raceID = id;
        clearAllPanels();
        setBaseStats();

    }

    public void SetName(string name)
    {
        Main.instance.currentCharacter.name = name;
    }

    public void setBaseStats()
    {
        Action<int> getStats = (spendingPoints) => {
            Main.instance.setSpendingPoints(spendingPoints);
            panelStatSelection.SetActive(true);
            panelStatSelection.GetComponent<ChooseStats>().textPointsToSpend.text = Main.instance.spendablePoints.ToString();

        };
        Main.instance.currentCharacter.STR += Main.instance.raceList[Main.instance.currentCharacter.raceID].STR;
        Main.instance.currentCharacter.AGL += Main.instance.raceList[Main.instance.currentCharacter.raceID].AGL;
        Main.instance.currentCharacter.END += Main.instance.raceList[Main.instance.currentCharacter.raceID].END;
        Main.instance.currentCharacter.SPD += Main.instance.raceList[Main.instance.currentCharacter.raceID].SPD;
        Main.instance.currentCharacter.LCK += Main.instance.raceList[Main.instance.currentCharacter.raceID].LCK;
        Main.instance.setStartingSpendingPoints(getStats);
        Main.instance.setMaxSpendingPoints();
    }

    public void setCharacterStats()
    {
        Action<int> setStatsResponse = (responseCode) =>
        {
            if(responseCode == 0) 
            {
                Main.instance.panelCharacterCreation.SetActive(false);
            }
            //  Main.instance.setSpendingPoints(responseCode);

        };
        StartCoroutine(
        Main.instance.webRequest.SetStats(setStatsResponse));
    }

}
