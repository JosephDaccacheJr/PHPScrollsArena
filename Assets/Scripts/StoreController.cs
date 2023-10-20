using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using static UnityEditor.Progress;


public class itemData
{
    public string category;
    public int ID;
    public string name;
    public string stat;
    public string governingStat;
    public string cost;
}

public class StoreController : MonoBehaviour
{
    public List<itemData> items = new List<itemData>();
    public GameObject storeItemHolder;
    public GameObject storeItemPrefab;
    List<GameObject> _storeItemList = new List<GameObject>();
    void OnEnable()
    {
        fillItemData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fillItemData()
    {
        foreach (GameObject item in _storeItemList)
        {
            Destroy(item);
        }
        items.Clear();
        JSONNode readItemData;
        Action<JSONNode> getItemCallback = (itemInfo) =>
        {
            readItemData = itemInfo;
            foreach(var cat in readItemData)
            {
                Debug.Log("ITEM: " + cat.Key + " " + cat.Value);
                JSONNode itemCat = (JSONNode)cat.Value;
                foreach (var itemCatItem in itemCat)
                {
                    itemData newItem = new itemData();
                    newItem.category = cat.Key;
                    newItem.ID = int.Parse(itemCatItem.Value["id"]);
                    newItem.name = itemCatItem.Value["name"];
                    newItem.governingStat = itemCatItem.Value["govstat"];
                    newItem.cost = itemCatItem.Value["cost"];

                    if(cat.Key == "ARMORS")
                    {
                        newItem.stat = "AC: " + itemCatItem.Value["AC"];
                    }
                    else
                    {
                        newItem.stat = "DMG: " + itemCatItem.Value["dmgnum"] + "D" + itemCatItem.Value["dmgsides"];
                    }
                    items.Add(newItem);
                }
            }
            showStoreItems();
            updateItemsBoughtStatus();
            Debug.Log("Item List complete");
        };

        StartCoroutine(Main.instance.webRequest.GetItems(getItemCallback));
    }

    public void showStoreItems()
    {
        foreach (itemData item in items)
        {
            GameObject newStoreItem = Instantiate(storeItemPrefab, storeItemHolder.transform);
            UIStoreItem si = newStoreItem.GetComponent<UIStoreItem>();
            si.SetItemInfo(item.ID,item.name,item.stat,item.governingStat,item.cost,item.category);
            _storeItemList.Add(newStoreItem);
        }
    }

    public void updateItemsBoughtStatus()
    {
        foreach (GameObject item in _storeItemList)
        {
            item.GetComponent<UIStoreItem>().checkIfBought();
        }
    }
}
