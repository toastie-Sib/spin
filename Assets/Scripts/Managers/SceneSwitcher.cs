using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [Header("UI References for Seed")] //Seed UI Manager
    public TMP_InputField seedInputField;
    public TMP_Text currentSeedText;
    [Header("Memory")]
    [HideInInspector] public GameObject fighterPrefab;
    [HideInInspector] public GameObject animatorPrefab;
    [HideInInspector] public GameObject otherAnimPrefab;
    [HideInInspector] public int fighterAmount = 0;
    [HideInInspector] public static SceneSwitcher Instance;
    [HideInInspector] public int enemyHP;
    [HideInInspector] public int chapter = 0;
    [Header("Player Info")]
    private Dictionary<string, int> collectedItems = new Dictionary<string, int>();
    [HideInInspector] public string currentNodeID; //Where on Map
    public float playerMaxHP;
    public float playerCurrentHP;
    public float playerBonusDamage = 0;
    public float playerBonusAtkSpd = 0;
    public float playerMoney = 100;


    void Awake()
    {
        
        if (transform.parent == null)
        {
            // Singleton pattern so only one exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public void SetSelectedPrefab1(GameObject prefabToSet1)
    {
        fighterPrefab = prefabToSet1;
    }

    public void SetSelectedPrefab2(GameObject prefabToSet2)
    {
        animatorPrefab = prefabToSet2;
    }

    public void SetStartHP(float hp)
    {
        playerCurrentHP = hp;
        playerMaxHP = hp;
    }

    public void SetButtonActive(GameObject button)
    {
        Button buttonRef = button.GetComponent<Button>();
        buttonRef.interactable = true;
    }

    public GameObject GetSelectedPrefab()
    {
        return fighterPrefab;
    }

    // Public function to load a scene by name
    public void LoadSpecificScene(string sceneName)
    {
        fighterAmount = 0;
        SceneManager.LoadScene(sceneName);
    }

    public void SlowLoadSpecificSceneDelay(string sceneName)
    {
        fighterAmount = 0;
        StartCoroutine(LoadSpecificSceneDelayed(sceneName));
    }

    private IEnumerator LoadSpecificSceneDelayed(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
        if(sceneName == "Title")
        {
            Destroy(gameObject);
        }
    }

    public void LoadMapScene()
    {
        if(chapter == 0) {
            SceneManager.LoadScene("Chapter0");
        } else if (chapter == 1) {
            SceneManager.LoadScene("Chapter1");
        } else if (chapter == 2) {
            SceneManager.LoadScene("Chapter2");
        } else if (chapter == 3) {
            SceneManager.LoadScene("Chapter3");
        } else if (chapter == 4)
        {
            SceneManager.LoadScene("Chapter4");
        }
    }

    //Ending Battle
    private void OnEnable()
    {
        Fighter.OnFighterDied += HandleFighterDeath;
    }

    private void OnDisable()
    {
        Fighter.OnFighterDied -= HandleFighterDeath;
    }

    private void HandleFighterDeath(Fighter fighter) //Game End
    {
        if (fighter.isPlayer)
        {
            SlowLoadSpecificSceneDelay("Title");
        }
        else
        {
            fighterAmount--;

            if (fighterAmount <= 1)
            {
                GameObject pSC = GameObject.Find("PlayerSpawnCannon");
                playerCurrentHP = pSC.GetComponent<Launcher>().fighter.hp;

                string name = "MoneySystem";
                string rngName = name.Replace("System", currentNodeID);
                SeedManager.Instance.UseSubSeed(rngName); //generate random amount of money
                int rewardMoney = Random.Range(50, 75);
                SeedManager.Instance.RestoreMasterSeed();

                playerMoney += rewardMoney;

                SlowLoadSpecificSceneDelay("ItemPick");
            }
        }
    }

    // Item manager scripts
    public void AddItem(string itemID)// Add an item (increase count if already collected)
    {
        if (itemID == "GlassBall") //Items
        {
            playerMaxHP *= 0.5f;
        }
        if (itemID == "Food")
        {
            float heldHP = playerMaxHP;
            playerMaxHP += (playerMaxHP * 0.3f);
            playerCurrentHP += (playerMaxHP - heldHP);
        }
        if (collectedItems.ContainsKey(itemID))
        {
            collectedItems[itemID]++; // add another copy
        }
        else
        {
            collectedItems[itemID] = 1; // first copy
        }

        Debug.Log($"Collected {itemID}. Total: {collectedItems[itemID]}");
    }

    public void RemoveItem(string itemName, int amount) // Remove Item from Cards Collected
    {
        if (collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName] -= amount;
            if (collectedItems[itemName] <= 0)
                collectedItems.Remove(itemName);
        }
    }

    public int GetItemCount(string itemID)// Get the number of copies the player has of an item
    {
        if (collectedItems.ContainsKey(itemID))
        {
            return collectedItems[itemID];
        }
        return 0;
    }

    public bool HasItem(string itemID)// Check if player has at least one copy of an item
    {
        return GetItemCount(itemID) > 0;
    }

    public Dictionary<string, int> GetItemsList()
    {
        return collectedItems;
    }

    private Dictionary<string, string> itemRarities = new Dictionary<string, string>()
    {
        { "BloodoftheArcher", "Uncommon" }, { "BloodoftheBandit", "Common" }, { "BloodoftheKnight", "Common" }, { "BloodoftheSoldier", "Common" }, { "Food", "Common" }, { "RaiseTheRoof", "Common" }, { "Training", "Common" }, 
        { "TriTippedDagger", "Common" }, { "GlassBall", "Rare" }, { "GatitoBlade", "Rare" },
        //Not in yet
    { "Poison", "Common" },
    { "WindTurbine", "Rare" },
    { "Electricity", "Uncommon" },
    { "Bomb", "Uncommon" },
    { "Fission", "Rare" },
    { "HMIYC", "Boss" },
    { "SkillShot", "Boss" },
    { "BladeBeam", "Boss" },
    { "PerfectDash", "Boss" },
    { "ShadowStep", "Boss" },
    { "BloodSacrifice", "Boss" },
    { "RemoteDetonation", "Boss" },
    { "RubberArrows", "Boss" },
    { "NewEquipment", "Boss" },
    { "Reflect", "Uncommon" },
    { "TungstonSphere", "Uncommon" },
    { "Brimstone", "Rare" },
    { "Flashback", "Rare" },
    { "228000LeafClover", "Rare" },
    { "TinyPlanet", "Uncommon" },
    { "TripleA", "Uncommon" },
    { "TrainingDummy", "Rare" },
    { "Candy", "Common" },
    { "ShatteredStopwatch", "Rare" },
    { "Stand Strong", "Uncommon" },
    { "MiniMushroom", "Uncommon" },
    { "Statue", "Rare" },
    { "BloodoftheFighter", "Uncommon" },
    { "BloodoftheMage", "Rare" },
    { "BloodofthePhalanx", "Uncommon" },
    { "BloodoftheReaper", "Uncommon" },
    { "BloodoftheEngineer", "Uncommon" },
    { "BloodoftheShinobi", "Rare" },
    { "BloodoftheBarbarian", "Uncommon" },
    { "BloodoftheCaptain", "Common" },
    { "TheRodofFishman", "Rare" },
    { "Loop", "Uncommon" },
    { "MagicHat", "Common" },
    { "BouncyBall", "Uncommon" },
    { "Defunded", "Uncommon" },
    { "Rob", "Boss" },
    { "VVVVVVV", "Uncommon" },
    { "MoneyPower", "Uncommon" },
    };

    public string GetItemRarity(string itemName)
    {
        return itemRarities.ContainsKey(itemName) ? itemRarities[itemName] : "Common";
    }

    public void SetCurrentNode(string id)
    {
        currentNodeID = id;
    }

    // Utility to find the node again in the map scene
    public MapNode GetCurrentNode()
    {
        foreach (var node in FindObjectsOfType<MapNode>())
        {
            if (node.nodeID == currentNodeID)
                return node;
        }
        return null;
    }
}
