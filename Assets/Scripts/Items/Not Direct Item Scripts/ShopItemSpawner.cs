using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSpawner : Assign
{
    public GameObject[] commonItems;
    public GameObject[] uncommonItems;
    public GameObject[] rareItems;
    public float waitTime;
    public int num;
    public int rarity;
    [HideInInspector] public int itemcost = 100;
    public bool trading = false;
    public GameObject costPrefab;
    public Transform costPlacement;
    private GameObject storedItemPrefab;
    private string objectName;
    private static HashSet<string> chosenNamesThisRound = new HashSet<string>();
    private static string lastRoundKey = null; // e.g., node ID or scene key
    private GameObject[] pool;

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
        if (rarity == 1) { pool = commonItems;
            string costSeedName = "Item System Cost";
            string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
            SeedManager.Instance.UseSubSeed(costName);
            itemcost = Random.Range(70, 130);
            if (trading == true) { itemcost = 2; }
        }
        else if (rarity == 2) { pool = uncommonItems;
            string costSeedName = "Item System Cost";
            string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
            SeedManager.Instance.UseSubSeed(costName);
            itemcost = Random.Range(170, 230);
            if (trading == true) { itemcost = 3; }
        }
        else if (rarity == 3) { pool = rareItems;
            string costSeedName = "Item System Cost";
            string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
            SeedManager.Instance.UseSubSeed(costName);
            int itemcost = Random.Range(270, 330);
            if (trading == true) { itemcost = 5; }
        }

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
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round


        SeedManager.Instance.RestoreMasterSeed();

        if (costPrefab != null)
        {
            GameObject cost = Instantiate(costPrefab, transform);
            cost.GetComponent<Text>().text = itemcost.ToString();
            cost.transform.SetParent(costPlacement, false);
        }

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");
        storedItemPrefab = itemCard;

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
        // Seed rarity choice separately (deterministic per node if you want)
        string seedName = "Item System Rarity";
        string rngName = seedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(rngName);

        int roll = Random.Range(0, 100);
        // Your original split was 70/20/10 (common/uncommon/rare).
        // If you actually want 70/20/10, leave it as-is. If you want 70/20/10 -> 70/20/10.
        GameObject[] result = (roll < 70) ? commonItems : (roll < 90) ? uncommonItems : rareItems;

        // Do NOT restore here; we restore once per RandomizeItem after picking the item
        return result;
    }

    private void HoldOntoName()
    {
        ItemChoose itemChoose = GameObject.Find("Purchase")?.GetComponent<ItemChoose>();
        if (itemChoose != null) itemChoose.itemString = objectName;
        itemChoose.storedCard = storedItemPrefab;
        itemChoose.trading = trading;
        itemChoose.cost = itemcost;
    }

    private void FallbackRareRandomizeItem()
    {

        string costSeedName = "Item System Cost";
        string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(costName);
        itemcost = Random.Range(270, 330);
        if (trading == true) { itemcost = 5; }

        // 1) Pick rarity pool
        pool = rareItems;

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
            FallbackUncommonRandomizeItem();
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round

        SeedManager.Instance.RestoreMasterSeed();

        if (costPrefab != null)
        {
            GameObject cost = Instantiate(costPrefab, transform);
            cost.GetComponent<Text>().text = itemcost.ToString();
            cost.transform.SetParent(costPlacement, false);
        }
        

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");
        storedItemPrefab = itemCard;

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
        string costSeedName = "Item System Cost";
        string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(costName);
        itemcost = Random.Range(170, 230);
        if (trading == true) { itemcost = 3; }

        // 1) Pick rarity pool
        pool = uncommonItems;

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
            FallbackCommonRandomizeItem();
        }

        // 5) Pick one from the remaining list
        GameObject chosen = validItems[Random.Range(0, validItems.Count)];
        chosenNamesThisRound.Add(chosen.name); // prevent duplicates across the three spawners this round

        SeedManager.Instance.RestoreMasterSeed();

        if (costPrefab != null)
        {
            GameObject cost = Instantiate(costPrefab, transform);
            cost.GetComponent<Text>().text = itemcost.ToString();
            cost.transform.SetParent(costPlacement, false);
        }

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");
        storedItemPrefab = itemCard;

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
        string costSeedName = "Item System Cost";
        string costName = costSeedName.Replace("System", SceneSwitcher.Instance.currentNodeID + num);
        SeedManager.Instance.UseSubSeed(costName);
        itemcost = Random.Range(70, 130);
        if (trading == true) { itemcost = 2; }

        // 1) Pick rarity pool
        pool = commonItems;

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

        if (costPrefab != null)
        {
            GameObject cost = Instantiate(costPrefab, transform);
            cost.GetComponent<Text>().text = itemcost.ToString();
            cost.transform.SetParent(costPlacement, false);
        }

        // 6) Spawn the UI card
        GameObject itemCard = Instantiate(chosen, transform);
        objectName = itemCard.name.Replace("(Clone)", "");
        storedItemPrefab = itemCard;

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
