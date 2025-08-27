using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoose : Assign
{
    public string itemString;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        Button proceedButton = GetComponent<Button>();

        proceedButton.onClick.AddListener(AssignName);
    }

    public void AssignName()
    {
        if(itemString == "")
        {
            GameObject nI = GameObject.Find("No Item");
            nI.transform.SetParent(GameObject.Find("Set Position").transform);
        } else
        {
            es.AddItem(itemString);
            es.LoadSpecificScene("Chapter0");
        }
        
    }
}
