using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    private GameObject storedItemPrefab;
    private string objectName;
    // Start is called before the first frame update
    void Start()
    {
        SeedManager.Instance.UseSubSeed("ItemSystem"); //generate random item

        int itemTypeIndex = Random.Range(0, itemPrefabs.Length);
        storedItemPrefab = itemPrefabs[itemTypeIndex];

        SeedManager.Instance.RestoreMasterSeed();

        GameObject itemCard = Instantiate(storedItemPrefab, this.transform);

        objectName = itemCard.name;

        itemCard.GetComponent<Button>().onClick.AddListener(HoldOntoName);

        itemCard.transform.SetParent(GameObject.Find("Items").transform);

        transform.position = new Vector3(5000, 0, 0);
    }

    public void HoldOntoName()
    {
        ItemChoose lII = GameObject.Find("Lock In Item").GetComponent<ItemChoose>();
        lII.itemString = objectName;
    }
}
