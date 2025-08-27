using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : Assign
{
    public GameObject[] commonItems;
    public GameObject[] uncommonItems;
    public GameObject[] rareItems;
    public float waitTime;
    private GameObject storedItemPrefab;
    private string objectName;
    private static List<int> chosenIndices = new List<int>();

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        StartCoroutine(RandomizeItem());
    }

    public IEnumerator RandomizeItem()
    {
        yield return new WaitForSeconds(waitTime);

        SeedManager.Instance.UseSubSeed("ItemSystem"); // generate random item

        // Pick rarity pool first
        GameObject[] pool = ChooseRarityPool();

        // Filter out blocked items
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            ItemBans itemBans = item.GetComponent<ItemBans>();
            if (itemBans == null || !itemBans.CannotBeUsedBy(es.fighterPrefab))
            {
                validItems.Add(item);
            }
        }

        // Remove already chosen ones
        for (int i = validItems.Count - 1; i >= 0; i--)
        {
            if (chosenIndices.Contains(System.Array.IndexOf(pool, validItems[i])))
            {
                validItems.RemoveAt(i);
            }
        }

        if (validItems.Count == 0)
        {
            Debug.LogWarning("No valid items left for this fighter!");
            Start();
            yield break;
        }

        // Pick random from valid list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        int itemTypeIndex = System.Array.IndexOf(pool, chosen);
        chosenIndices.Add(itemTypeIndex);

        storedItemPrefab = chosen;

        SeedManager.Instance.RestoreMasterSeed();

        GameObject itemCard = Instantiate(storedItemPrefab, this.transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        itemCard.GetComponent<Button>().onClick.AddListener(HoldOntoName);
        itemCard.transform.SetParent(GameObject.Find("Items").transform);

        transform.localScale += new Vector3(0.25f, 0.25f, 0);
        //transform.position = new Vector3(5000, 0, 0);
    }

    private GameObject[] ChooseRarityPool()
    {
        int roll = Random.Range(0, 100);

        if (roll < 70)
            return commonItems;
        else if (roll < 90)
            return uncommonItems;
        else
            return rareItems;
    }

    public void HoldOntoName()
    {
        ItemChoose lII = GameObject.Find("Lock In Item").GetComponent<ItemChoose>();
        lII.itemString = objectName;
    }
}
