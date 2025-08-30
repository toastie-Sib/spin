using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopTradeItems : MonoBehaviour
{
    public GameObject itemDisplayPrefab; // assign in inspector
    public Transform contentParent;      // assign ScrollView Content
    [HideInInspector] public int selectionLimit = 3;       // max allowed selection
    [HideInInspector] public string filterRarity = "Common"; // which rarity to show
    [HideInInspector] public string itemString;

    private SceneSwitcher sceneSwitcher;

    // Track selected buttons
    private List<GameObject> selectedButtons = new List<GameObject>();

    void Start()
    {
        sceneSwitcher = SceneSwitcher.Instance;
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        // Clear old entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        selectedButtons.Clear();

        // Get all items
        Dictionary<string, int> items = sceneSwitcher.GetItemsList();

        foreach (var kvp in items)
        {
            string itemName = kvp.Key;
            int quantity = kvp.Value;
            string rarity = sceneSwitcher.GetItemRarity(itemName);

            // Skip if rarity doesn’t match filter
            if (rarity != filterRarity) continue;

            // Spawn one entry per copy
            for (int i = 0; i < quantity; i++)
            {
                GameObject newEntry = Instantiate(itemDisplayPrefab, contentParent);

                // Assign UI values
                TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
                Image icon = newEntry.GetComponentInChildren<Image>();

                texts[0].text = itemName;
                texts[1].text = ""; // no stack number

                Sprite loadedSprite = Resources.Load<Sprite>("Sprites/Items/" + itemName);
                if (loadedSprite != null)
                    icon.sprite = loadedSprite;

                // Reset background color
                newEntry.GetComponent<Image>().color = Color.white;

                // Add toggle functionality
                Button btn = newEntry.GetComponent<Button>();
                btn.onClick.AddListener(() => OnItemClicked(newEntry, itemName));
            }
        }
    }

    private void OnItemClicked(GameObject buttonObj, string itemName)
    {
        Image bg = buttonObj.GetComponent<Image>();

        // If already selected → deselect
        if (selectedButtons.Contains(buttonObj))
        {
            selectedButtons.Remove(buttonObj);
            bg.color = Color.white; // normal
            return;
        }

        // If selection limit reached → ignore
        if (selectedButtons.Count >= selectionLimit)
        {
            Debug.Log("Reached selection limit!");
            return;
        }

        // Otherwise select it
        selectedButtons.Add(buttonObj);
        bg.color = Color.yellow; // highlight
    }

    public void ConfirmTrade()
    {
        if (selectedButtons.Count < selectionLimit) return;
        // Count selected items by name
        Dictionary<string, int> tradeCounts = new Dictionary<string, int>();

        foreach (GameObject buttonObj in selectedButtons)
        {
            string itemName = buttonObj.GetComponentInChildren<TextMeshProUGUI>().text;
            if (!tradeCounts.ContainsKey(itemName))
                tradeCounts[itemName] = 0;
            tradeCounts[itemName]++;
        }

        sceneSwitcher.AddItem(itemString);

        // Remove from SceneSwitcher
        foreach (var kvp in tradeCounts)
        {
            sceneSwitcher.RemoveItem(kvp.Key, kvp.Value);
        }

        transform.SetParent(GameObject.Find("No Item").transform, false);

        selectedButtons.Clear();

        RefreshDisplay();
    }

    public void CancelTrade()
    {
        transform.SetParent(GameObject.Find("No Item").transform, false);

        selectedButtons.Clear();

        RefreshDisplay();
    }
}
