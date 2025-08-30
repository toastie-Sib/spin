using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayItems : MonoBehaviour
{
    public GameObject itemDisplayPrefab; // assign in inspector
    public Transform contentParent;      // assign ScrollView Content

    private SceneSwitcher sceneSwitcher;

    void Start()
    {
        sceneSwitcher = SceneSwitcher.Instance;
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        // Clear old entries
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Get current collected items
        Dictionary<string, int> items = sceneSwitcher.GetItemsList();

        foreach (var kvp in items)
        {
            GameObject newEntry = Instantiate(itemDisplayPrefab, contentParent);

            // Assign values
            TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            Image icon = newEntry.GetComponentInChildren<Image>();

            // Assuming prefab has: NameText, QuantityText
            texts[0].text = kvp.Key;            // Item name
            texts[1].text = "x" + kvp.Value;   // Quantity

            // Optionally: assign icon from Resources or ScriptableObject database
            Sprite loadedSprite = Resources.Load<Sprite>("Items/" + kvp.Key);
            if (loadedSprite != null)
                icon.sprite = loadedSprite;
        }
    }
}
