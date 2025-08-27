using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public float waitTime;
    private GameObject storedItemPrefab;
    private string objectName;
    private static List<int> chosenIndices = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomizeItem());
    }

    public IEnumerator RandomizeItem()
    {
        yield return new WaitForSeconds(waitTime);

        SeedManager.Instance.UseSubSeed("ItemSystem"); //generate random item

        // keep rolling until we get a unique index
        int itemTypeIndex;
        do
        {
            itemTypeIndex = Random.Range(0, itemPrefabs.Length);
        }
        while (chosenIndices.Contains(itemTypeIndex) && chosenIndices.Count < itemPrefabs.Length);

        chosenIndices.Add(itemTypeIndex);
        storedItemPrefab = itemPrefabs[itemTypeIndex];

        SeedManager.Instance.RestoreMasterSeed();

        GameObject itemCard = Instantiate(storedItemPrefab, this.transform);

        objectName = itemCard.name.Replace("(Clone)", "");

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
