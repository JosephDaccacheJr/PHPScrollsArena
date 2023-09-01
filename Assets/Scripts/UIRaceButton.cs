using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRaceButton : MonoBehaviour
{
    public Text textRaceName;
    public Text textRaceStats;

    int _raceID;
    public int raceID
    {
        get { return _raceID; }

        set { if (_raceID == 0) { _raceID = value; } }
    }

    public void OnSelectRace()
    {
        Main.instance.charCreator.SetRace(_raceID);
    }

}
