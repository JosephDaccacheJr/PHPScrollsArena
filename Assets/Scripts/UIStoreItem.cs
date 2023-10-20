using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class UIStoreItem : MonoBehaviour
{
    public Text textItemName, textItemStat, textGoverningStat, textCost;
    public GameObject boughtCheck;

    int _itemID;
    string _itemCategory;
    bool _isBought;
    
    public void SetItemInfo(int itemID, string strName, string strItemStat, string strGoverningStat, string strCost,string strCategory)
    {
        _itemID = itemID;
        textItemName.text = strName;
        textItemStat.text = strItemStat;
        textGoverningStat.text = strGoverningStat;
        textCost.text = strCost;
        _itemCategory = strCategory;
    }

    public void clickBuy()
    {
        if (_isBought)
            return;

        Action<string> buyItemCallback = (buyInfo) =>
        {
            int buyCode = int.Parse(buyInfo);
            switch(buyCode)
            {
                case 0:
                    Main.instance.updatePlayerInfoPanel();
                    break;
                case 1:
                    Debug.Log("ERROR: CAN'T FIND ITEM");
                    break;
                case -1:
                    Debug.Log("NOT ENOUGH GOLD");
                    break;
            }
        };
        StartCoroutine(Main.instance.webRequest.buyItem(Main.instance.currentID, _itemID, _itemCategory, buyItemCallback));
        
    }

    public void checkIfBought()
    {
        int idToCheck = 0;
        if (_itemCategory == "WEAPONS")
        {
            idToCheck = Main.instance.currentCharacter.weaponID;
        }
        else if (_itemCategory == "ARMORS")
        {
            idToCheck = Main.instance.currentCharacter.armorID;
        }
        _isBought = idToCheck == _itemID;
        boughtCheck.SetActive(_isBought);
    }
}
