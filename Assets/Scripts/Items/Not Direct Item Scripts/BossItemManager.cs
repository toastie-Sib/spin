using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossItemManager : MonoBehaviour
{
    public Button[] bossButtons; // assign in inspector (enough buttons for max boss items)
    private SceneSwitcher sceneSwitcher;

    void Start()
    {
        sceneSwitcher = SceneSwitcher.Instance;
        SetupBossButtons();
    }

    public void SetupBossButtons()
    {
        // Deactivate all buttons first
        foreach (Button btn in bossButtons)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.RemoveAllListeners();
        }

        // Get all items
        Dictionary<string, int> items = sceneSwitcher.GetItemsList();

        int buttonIndex = 0;

        foreach (var kvp in items)
        {
            string itemName = kvp.Key;
            int quantity = kvp.Value;
            string rarity = sceneSwitcher.GetItemRarity(itemName);

            if (rarity == "Boss") // Only care about Boss rarity
            {
                for (int i = 0; i < quantity; i++) // multiple copies = multiple buttons
                {
                    if (buttonIndex >= bossButtons.Length)
                    {
                        Debug.LogWarning("Not enough boss buttons assigned in inspector!");
                        return;
                    }

                    Button btn = bossButtons[buttonIndex];
                    btn.gameObject.SetActive(true);

                    // Update button label (if it has a Text or TMP child)
                    Text label = btn.GetComponentInChildren<Text>();
                    if (label != null) label.text += " "+itemName;

                    // Capture correct itemName in listener
                    string capturedName = itemName;
                    btn.onClick.AddListener(() => OnBossItemClicked(capturedName));

                    buttonIndex++;
                }
            }
        }
    }

    private void OnBossItemClicked(string itemName)
    {
        //Add in cooldown
        if(itemName == "NewEquipment") {
            foreach (var spawner in FindObjectsOfType<Launcher>())
            {
                if (spawner.isPlayer == true)
                    spawner.stashedProjectile.GetComponentInChildren<Spear>().NewEquipment();
            }
        }
            
    }
}
