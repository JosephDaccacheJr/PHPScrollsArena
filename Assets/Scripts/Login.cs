using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField inputCharacterName;
    public Button buttonLogin;

    private void Start()
    {
        buttonLogin.onClick.AddListener(OnLoginClick);
    }

    public void OnLoginClick()
    {
        Action<int> loginCallback = (loginCode) => {
            print("loginCallback: " + loginCode);
            if (loginCode == -1)
            {
                Action<int> registerCallback = (newID) => 
                {
                    Main.instance.panelLogin.SetActive(false);
                    Main.instance.panelCharacterCreation.SetActive(true);
                    Main.instance.charCreator.StartCharacterCreation(newID);
                    Main.instance.setCurrentID(newID);
                };
                StartCoroutine(Main.instance.webRequest.Register(registerCallback, inputCharacterName.text));
            }
            else
            {
                Main.instance.closeAllPanels();
                Main.instance.panelArenaMain.SetActive(true);
                Main.instance.setCurrentID(loginCode);
                Main.instance.updatePlayerInfoPanel();
            }

        };
        StartCoroutine(Main.instance.webRequest.Login(inputCharacterName.text, loginCallback));
    }
}
