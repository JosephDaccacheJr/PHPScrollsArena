using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Net;

public class WebRequests : MonoBehaviour
{
    public static string hostURL = "http://localhost//phpscrollsarena/";
    public IEnumerator Login(string username, Action<int> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterName", username);

        using (UnityWebRequest www = UnityWebRequest.Post(hostURL + "login.php", form))
        {

            yield return www.SendWebRequest();


            int responseCode;
            int.TryParse(www.downloadHandler.text, out responseCode);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("SERVER ERROR: " + responseCode);
            }
            else
            {
                Debug.Log("SERVER: Login Web request success");
                if (responseCode == -2)
                {
                    Debug.Log("SERVER ERROR: Invalid credentials");
                   // StartCoroutine(Register(username));
                }
                else if (responseCode == -1)
                {
                    Debug.Log("SERVER ERROR: Character does not exist");
                }
                else
                {
                    
                    Debug.Log("SERVER: Character found");
                }
            }
            callback(responseCode);
        }
    }

    public IEnumerator Register(System.Action<int> callback, string username)
    {

        WWWForm form = new WWWForm();
        form.AddField("characterName", username);

        using (UnityWebRequest www = UnityWebRequest.Post(hostURL + "registercharacter.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("SERVER ERROR: " + www.responseCode);
            }
            else
            {

                JSONNode tempArray = JSON.Parse(www.downloadHandler.text) as JSONNode;

                int newID = 0;
                int.TryParse(tempArray[0].Value, out newID);
                callback(newID);

                Debug.Log("SERVER: Character created. " + www.downloadHandler.text);
            }
        }

    }

    public IEnumerator GetRule(string ruleName, System.Action<int> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("ruleName", ruleName);

        using (UnityWebRequest www = UnityWebRequest.Post(hostURL + "getrule.php", form))
        {
            yield return www.SendWebRequest();
            Debug.Log("RULE: " + www.downloadHandler.text);
            int responseCode;
            int.TryParse(www.downloadHandler.text, out responseCode);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("SERVER ERROR: " + www.responseCode);
            }
            else
            {
                Debug.Log("SERVER: GetRule Web request success");
                if (responseCode == -1)
                {
                    Debug.Log("SERVER ERROR: Rule not found");
                }
                else
                {
                    Debug.Log("SERVER: Rule found " + responseCode);
                }
            }

            callback(responseCode);
        }
    }

    public IEnumerator GetRaces(System.Action<JSONArray> callback)
    {

        using (UnityWebRequest WWW = UnityWebRequest.Get(hostURL + "getraces.php"))
        {
            {
                {
                    yield return WWW.SendWebRequest();
                    if (WWW.isNetworkError || WWW.isHttpError)
                    {

                        Debug.Log("ERROR: " + WWW.error);

                    }
                    else
                    {
                        string jsonArray = WWW.downloadHandler.text;
                        Debug.Log("SERVER: Got race data: " + WWW.downloadHandler.text);
                        JSONArray tempArray = JSON.Parse(jsonArray) as JSONArray;
                        callback(JSON.Parse(jsonArray) as JSONArray);

                    }
                }
            }
        }
    }

    public IEnumerator GetOpponents(System.Action<JSONArray> callback)
    {

        using (UnityWebRequest WWW = UnityWebRequest.Get(hostURL + "getopponents.php"))
        {
            {
                {
                    yield return WWW.SendWebRequest();
                    if (WWW.isNetworkError || WWW.isHttpError)
                    {

                        Debug.Log("ERROR: " + WWW.error);

                    }
                    else
                    {
                        string jsonArray = WWW.downloadHandler.text;
                        JSONArray tempArray = JSON.Parse(jsonArray) as JSONArray;
                        Debug.Log("SERVER: Got opponent data: " + WWW.downloadHandler.text + " " + tempArray.Count);
                        callback(JSON.Parse(jsonArray) as JSONArray);

                    }
                }
            }
        }
    }

    public IEnumerator battleCommand(string command,int oppID, Action<JSONNode> callback, movTyp moveType = movTyp.regularAttack)
    {
        Debug.Log("currentID " + Main.instance.currentID);
        WWWForm form = new WWWForm();
        form.AddField("command", command);
        form.AddField("opponentID", oppID);
        form.AddField("playerID", Main.instance.currentID.ToString());
        form.AddField("moveType", moveType.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(hostURL + "battle.php", form))
        {
            yield return www.SendWebRequest();
        //    int responseCode;
        //    int.TryParse(www.downloadHandler.text, out responseCode);
            if (command == "startBattle")
            {
                Debug.Log("Battle Command: " + www.downloadHandler.text);
                callback(null);
                
            }
            else if (command == "updateBattle")
            {
                string jsonArray = www.downloadHandler.text;
                JSONArray tempArray = JSONArray.Parse(jsonArray) as JSONArray;

                Debug.Log("updateBattle: " + www.downloadHandler.text + " " + tempArray[0]);
                callback(tempArray[0]);

            }
            else if (command == "playerAttack")
            {
                string jsonArray = www.downloadHandler.text;
                callback((JSON.Parse(jsonArray) as JSONArray)[0]);
            }
        }

    }

    public IEnumerator SetStats(System.Action<int> callback) 
    {
        WWWForm form = new WWWForm();
        form.AddField("STR", Main.instance.currentCharacter.STR);
        form.AddField("AGL", Main.instance.currentCharacter.AGL);
        form.AddField("END", Main.instance.currentCharacter.END);
        form.AddField("SPD", Main.instance.currentCharacter.SPD);
        form.AddField("LCK", Main.instance.currentCharacter.LCK);
        form.AddField("RACE", Main.instance.currentCharacter.raceID);
        form.AddField("ID", Main.instance.currentCharacter.id);
        Debug.Log("Setting stats for ID " + Main.instance.currentCharacter.id);
        using (UnityWebRequest www = UnityWebRequest.Post(hostURL + "setstats.php", form))
        {
            Debug.Log("SET STATS TEXT: " + www.downloadHandler.text);
            yield return www.SendWebRequest();
            int responseCode;
            int.TryParse(www.downloadHandler.text, out responseCode);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("SERVER ERROR: " + www.responseCode);
            }
            else
            {
                Debug.Log("SERVER: GetRule Web request success");
                if (responseCode == -1)
                {
                    Debug.Log("SERVER ERROR: Stats failed to set");
                }
                else
                {
                    Debug.Log("SERVER: stat found " + responseCode);
                }
            }

            callback(responseCode);
        }
    }
}
