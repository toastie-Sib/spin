using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawnerBoss : Assign
{
    [System.Serializable]
    public class FighterItemPool
    {
        public string fighterName;
        public GameObject[] items;
    }

    public FighterItemPool[] fighterPools;   // Assign pools per fighter in inspector
    public GameObject foodPrefab;            // Assign a Food prefab in inspector
    public float waitTime;
    public int num;

    private GameObject storedItemPrefab;
    private string objectName;

    // Track unique item spawns across ALL spawners per round
    private static HashSet<string> chosenNamesThisRound = new HashSet<string>();
    private static string lastRoundKey = null;

    public override void Start()
    {
        base.Start();

        // Detect new round and reset uniqueness
        string roundKey = SceneSwitcher.Instance.currentNodeID;
        if (lastRoundKey != roundKey)
        {
            chosenNamesThisRound.Clear();
            lastRoundKey = roundKey;
        }

        StartCoroutine(SpawnFighterItem());
    }

    private IEnumerator SpawnFighterItem()
    {
        yield return new WaitForSeconds(waitTime);

        // Get fighter name
        string fighterName = es.fighterPrefab.name.Replace("(Clone)", "");

        // Get pool for that fighter
        GameObject[] pool = GetFighterPool(fighterName);
        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning($"No item pool found for fighter {fighterName}! Spawning Food instead.");
            SpawnFood();
            yield break;
        }

        // Seed for reproducibility
        string seedName = "ItemSystem";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        // Build list of valid items
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            string cleanName = item.name;

            bool alreadyChosen = chosenNamesThisRound.Contains(cleanName);
            bool fighterHasItem = es.HasItem(cleanName);

            if (!alreadyChosen && !fighterHasItem)
                validItems.Add(item);
        }

        GameObject chosen = null;

        if (validItems.Count == 0)
        {
            Debug.LogWarning($"No valid items left for fighter {fighterName} this round! Spawning Food instead.");
            chosen = foodPrefab;
        }
        else
        {
            chosen = validItems[Random.Range(0, validItems.Count)];
            chosenNamesThisRound.Add(chosen.name);
        }

        SeedManager.Instance.RestoreMasterSeed();

        // Spawn UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        var btn = itemCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            itemCard.transform.SetParent(itemsParent);

        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }

    private GameObject[] GetFighterPool(string fighterName)
    {
        foreach (var pool in fighterPools)
        {
            if (pool.fighterName == fighterName)
                return pool.items;
        }
        return null;
    }

    private void SpawnFood()
    {
        if (foodPrefab == null)
        {
            Debug.LogError("Food prefab not assigned in inspector!");
            return;
        }

        GameObject foodCard = Instantiate(foodPrefab, transform);
        objectName = foodCard.name.Replace("(Clone)", "");

        var btn = foodCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            foodCard.transform.SetParent(itemsParent);

        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }

    private void HoldOntoName()
    {
        ItemChoose lII = GameObject.Find("Lock In Item")?.GetComponent<ItemChoose>();
        if (lII != null) lII.itemString = objectName;
    }
}
