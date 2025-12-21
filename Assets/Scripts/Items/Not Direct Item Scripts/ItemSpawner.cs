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
    public int num;
    public int commonDropRate = 70;
    public int uncommonDropRate = 90;
    private GameObject storedItemPrefab;
    private string objectName;
    private static HashSet<string> chosenNamesThisRound = new HashSet<string>();
    private static string lastRoundKey = null; // e.g., node ID or scene key
    public GameObject garbagePrefab;
    public GameObject junkPrefab;
    public GameObject scrapPrefab;

    private ItemRarity chosenRarity;

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // New round? Clear the set so the three spawners can pick unique items again
        string roundKey = SceneSwitcher.Instance.currentNodeID; // or SceneManager.GetActiveScene().name
        if (lastRoundKey != roundKey)
        {
            chosenNamesThisRound.Clear();
            lastRoundKey = roundKey;
        }

        StartCoroutine(RandomizeItem());
    }

    private IEnumerator RandomizeItem()
    {
        yield return new WaitForSeconds(waitTime);

        // 1) Pick rarity pool
        GameObject[] pool = ChooseRarityPool();

        // 2) Seed for item choice (deterministic within a node/seed, still varied among the 3)
        string seedName = "ItemSystem";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        // 3) Build valid list (not banned, and not already chosen this round)
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            var bans = item.GetComponent<ItemBans>();
            bool blocked = (bans != null && bans.CannotBeUsedBy(es.fighterPrefab));
            bool alreadyPickedThisRound = chosenNamesThisRound.Contains(item.name);

            if (!blocked && !alreadyPickedThisRound)
                validItems.Add(item);
        }

        // 4) If that emptied the pool (e.g., bans + uniqueness), relax the uniqueness rule as a fallback
        //if (validItems.Count == 0)
        //{
        //    foreach (var item in pool)
        //    {
        //        var bans = item.GetComponent<ItemBans>();
        //        if (bans == null || !bans.CannotBeUsedBy(es.fighterPrefab))
        //            validItems.Add(item);
        //    }
        //}

        if (validItems.Count == 0)
        {
            Debug.LogWarning("No valid items left for this fighter in any pool!");
            FallbackRareRandomizeItem();
            yield break;
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        string chosenName = chosen.name.Replace("(Clone)", "");

        // check if player already has too many of this item ---
        int itemCount = SceneSwitcher.Instance.GetItemCount(chosenName);

        var limit = chosen.GetComponent<ItemBans>();
        int maxAllowed = limit.maxAllowed;
        if (maxAllowed > 0 && itemCount >= maxAllowed)
        {
            switch (chosenRarity)
            {
                case ItemRarity.Common:
                    chosen = garbagePrefab;
                    break;

                case ItemRarity.Uncommon:
                    chosen = junkPrefab;
                    break;

                case ItemRarity.Rare:
                    chosen = scrapPrefab;
                    break;
            }
        }


        // Now mark and restore RNG as before
        chosenNamesThisRound.Add(chosenName);
        SeedManager.Instance.RestoreMasterSeed();


        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        var btn = itemCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        // Keep UI hierarchy tidy; don't keep world position
        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            itemCard.transform.SetParent(GameObject.Find("Items").transform);

        // Optional: scale/animate the card
        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }

    private GameObject[] ChooseRarityPool()
    {
        string seedName = "Item System Rarity";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        int roll = Random.Range(0, 100);

        if (roll < commonDropRate)
        {
            chosenRarity = ItemRarity.Common;
            return commonItems;
        }
        else if (roll < uncommonDropRate)
        {
            chosenRarity = ItemRarity.Uncommon;
            return uncommonItems;
        }
        else
        {
            chosenRarity = ItemRarity.Rare;
            return rareItems;
        }
    }


    private void HoldOntoName()
    {
        ItemChoose lII = GameObject.Find("Lock In Item")?.GetComponent<ItemChoose>();
        if (lII != null) lII.itemString = objectName;
    }

    private void FallbackRareRandomizeItem()
    {

        // 1) Pick rarity pool
        GameObject[] pool = rareItems;

        // 2) Seed for item choice (deterministic within a node/seed, still varied among the 3)
        string seedName = "ItemRareSystem";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        // 3) Build valid list (not banned, and not already chosen this round)
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            var bans = item.GetComponent<ItemBans>();
            bool blocked = (bans != null && bans.CannotBeUsedBy(es.fighterPrefab));
            bool alreadyPickedThisRound = chosenNamesThisRound.Contains(item.name);

            if (!blocked && !alreadyPickedThisRound)
                validItems.Add(item);
        }

        // 4) If that emptied the pool (e.g., bans + uniqueness), relax the uniqueness rule as a fallback
        if (validItems.Count == 0)
        {
            foreach (var item in pool)
            {
                var bans = item.GetComponent<ItemBans>();
                if (bans == null || !bans.CannotBeUsedBy(es.fighterPrefab))
                    validItems.Add(item);
            }
        }

        if (validItems.Count == 0)
        {
            Debug.LogWarning("No valid items left for this fighter in any pool!");
            FallbackUncommonRandomizeItem();
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round

        SeedManager.Instance.RestoreMasterSeed();

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        var btn = itemCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        // Keep UI hierarchy tidy; don't keep world position
        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            itemCard.transform.SetParent(GameObject.Find("Items").transform);

        // Optional: scale/animate the card
        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }

    private void FallbackUncommonRandomizeItem()
    {

        // 1) Pick rarity pool
        GameObject[] pool = uncommonItems;

        // 2) Seed for item choice (deterministic within a node/seed, still varied among the 3)
        string seedName = "ItemUncommonSystem";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        // 3) Build valid list (not banned, and not already chosen this round)
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            var bans = item.GetComponent<ItemBans>();
            bool blocked = (bans != null && bans.CannotBeUsedBy(es.fighterPrefab));
            bool alreadyPickedThisRound = chosenNamesThisRound.Contains(item.name);

            if (!blocked && !alreadyPickedThisRound)
                validItems.Add(item);
        }

        // 4) If that emptied the pool (e.g., bans + uniqueness), relax the uniqueness rule as a fallback
        if (validItems.Count == 0)
        {
            foreach (var item in pool)
            {
                var bans = item.GetComponent<ItemBans>();
                if (bans == null || !bans.CannotBeUsedBy(es.fighterPrefab))
                    validItems.Add(item);
            }
        }

        if (validItems.Count == 0)
        {
            Debug.LogWarning("No valid items left for this fighter in any pool!");
            FallbackCommonRandomizeItem();
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round

        SeedManager.Instance.RestoreMasterSeed();

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        var btn = itemCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        // Keep UI hierarchy tidy; don't keep world position
        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            itemCard.transform.SetParent(GameObject.Find("Items").transform);

        // Optional: scale/animate the card
        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }

    private void FallbackCommonRandomizeItem()
    {

        // 1) Pick rarity pool
        GameObject[] pool = commonItems;

        // 2) Seed for item choice (deterministic within a node/seed, still varied among the 3)
        string seedName = "ItemCommonSystem";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        // 3) Build valid list (not banned, and not already chosen this round)
        List<GameObject> validItems = new List<GameObject>();
        foreach (var item in pool)
        {
            var bans = item.GetComponent<ItemBans>();
            bool blocked = (bans != null && bans.CannotBeUsedBy(es.fighterPrefab));
            bool alreadyPickedThisRound = chosenNamesThisRound.Contains(item.name);

            if (!blocked && !alreadyPickedThisRound)
                validItems.Add(item);
        }

        // 4) If that emptied the pool (e.g., bans + uniqueness), relax the uniqueness rule as a fallback
        if (validItems.Count == 0)
        {
            foreach (var item in pool)
            {
                var bans = item.GetComponent<ItemBans>();
                if (bans == null || !bans.CannotBeUsedBy(es.fighterPrefab))
                    validItems.Add(item);
            }
        }

        if (validItems.Count == 0)
        {
            Debug.LogWarning("No valid items left for this fighter in any pool! Could spawn Food.");
            return;
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round

        SeedManager.Instance.RestoreMasterSeed();

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");

        var btn = itemCard.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(HoldOntoName);

        // Keep UI hierarchy tidy; don't keep world position
        Transform itemsParent = GameObject.Find("Items")?.transform;
        if (itemsParent != null)
            itemCard.transform.SetParent(GameObject.Find("Items").transform);

        // Optional: scale/animate the card
        transform.localScale += new Vector3(0.15f, 0.15f, 0f);
    }
}
