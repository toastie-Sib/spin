using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoose : Assign
{
    public bool buying = false;
    [HideInInspector] public string itemString;
    [HideInInspector] public GameObject storedCard;
    [HideInInspector] public int cost;
    [HideInInspector] public bool trading = false;
    private Button button;



    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        button = GetComponent<Button>();

        button.onClick.AddListener(AssignName);
    }

    public void AssignName()
    {
        if (buying == false)
        {
            if (itemString == "")
            {
                GameObject nI = GameObject.Find("No Item");
                nI.transform.SetParent(GameObject.Find("Set Position").transform, false);
            }
            else
            {
                button.interactable = false;
                es.AddItem(itemString);
                es.LoadSpecificScene("Chapter0");
            }
        } else
        {
            if (itemString == "") return;
            if (trading == false)
            {
                if (SceneSwitcher.Instance.playerMoney >= cost)
                {
                    es.AddItem(itemString);
                    SceneSwitcher.Instance.playerMoney -= cost;
                    itemString = "";
                    Destroy(storedCard);
                }
                else
                {
                    GameObject nM = GameObject.Find("No Money");
                    nM.transform.SetParent(GameObject.Find("Set Position").transform, false);
                }
            } else //Trade cards
            {
                GameObject iP = GameObject.Find("InventoryPanel");
                iP.transform.SetParent(GameObject.Find("Set Position").transform, false);
                if(cost == 3)
                {
                    iP.GetComponent<ShopTradeItems>().filterRarity = "Common";
                    iP.GetComponent<ShopTradeItems>().selectionLimit = 3;
                    iP.GetComponent<ShopTradeItems>().RefreshDisplay();
                    iP.GetComponent<ShopTradeItems>().itemString = itemString;
                    itemString = "";
                }
                if (cost == 5)
                {
                    iP.GetComponent<ShopTradeItems>().filterRarity = "Uncommon";
                    iP.GetComponent<ShopTradeItems>().selectionLimit = 5;
                    iP.GetComponent<ShopTradeItems>().RefreshDisplay();
                    iP.GetComponent<ShopTradeItems>().itemString = itemString;
                    itemString = "";
                }
                if (cost == 2)
                {
                    iP.GetComponent<ShopTradeItems>().filterRarity = "Rare";
                    iP.GetComponent<ShopTradeItems>().selectionLimit = 1;
                    iP.GetComponent<ShopTradeItems>().RefreshDisplay();
                    iP.GetComponent<ShopTradeItems>().itemString = itemString;
                    itemString = "";
                }
            }
            
        }
        
        
    }
}
