using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [Header("UI References")] //Seed UI Manager
    public TMP_InputField seedInputField;
    public TMP_Text currentSeedText;
    [HideInInspector] public GameObject fighterPrefab;
    [HideInInspector] public GameObject animatorPrefab;
    public GameObject otherAnimPrefab;
    [HideInInspector] public int fighterAmount = 0;
    [HideInInspector] public static SceneSwitcher Instance;

    private Dictionary<string, int> collectedItems = new Dictionary<string, int>();

    [HideInInspector] public string currentNodeID;
    [HideInInspector] public float playerMaxHP;
    [HideInInspector] public float playerCurrentHP;

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

    //Ending Battle
    private void OnEnable()
    {
        Fighter.OnFighterDied += HandleFighterDeath;
    }

    private void OnDisable()
    {
        Fighter.OnFighterDied -= HandleFighterDeath;
    }

    private void HandleFighterDeath(Fighter fighter)
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


                SlowLoadSpecificSceneDelay("ItemPick");
            }
        }
    }

    // Item manager scripts
    // Add an item (increase count if already collected)
    public void AddItem(string itemID)
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

    // Get the number of copies the player has of an item
    public int GetItemCount(string itemID)
    {
        if (collectedItems.ContainsKey(itemID))
        {
            return collectedItems[itemID];
        }
        return 0;
    }

    // Check if player has at least one copy of an item
    public bool HasItem(string itemID)
    {
        return GetItemCount(itemID) > 0;
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
