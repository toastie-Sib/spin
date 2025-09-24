using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossItemManager : MonoBehaviour
{
    public Button[] bossButtons; // assign in inspector (enough buttons for max boss items)
    private SceneSwitcher sceneSwitcher;

    // Quick references for hotkeys
    private string itemQ, itemW, itemE, itemR, itemA, itemS, itemD, itemF;
    private Dictionary<string, Button> itemToButton = new Dictionary<string, Button>();

    // Cooldown tracking
    private Dictionary<string, float> itemCooldownTimes = new Dictionary<string, float>(); // itemName -> cooldown duration
    private Dictionary<string, float> itemCooldownEnd = new Dictionary<string, float>();   // itemName -> when cooldown ends

    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();
    private Dictionary<Button, string> originalTexts = new Dictionary<Button, string>();


    void Start()
    {
        sceneSwitcher = SceneSwitcher.Instance;

        itemCooldownTimes = new Dictionary<string, float>()
        {
            { "New Equipment", 12f },   // based on secs
            { "Rubber Arrows", 15f },
        };

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

        itemToButton.Clear();
        originalColors.Clear();
        originalTexts.Clear();

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
                    if (label != null)
                    {
                        label.text = itemName;
                        originalColors[btn] = label.color;
                        originalTexts[btn] = itemName;
                    }

                    // Capture correct itemName in listener
                    string capturedName = itemName;
                    btn.onClick.AddListener(() => TryUseBossItem(capturedName));

                    // Assign button reference
                    itemToButton[itemName] = btn;

                    //Assign item name to button
                    if (buttonIndex == 0) itemQ = itemName;
                    else if (buttonIndex == 1) itemW = itemName;
                    else if (buttonIndex == 2) itemE = itemName;
                    else if (buttonIndex == 3) itemR = itemName;
                    else if (buttonIndex == 4) itemA = itemName;
                    else if (buttonIndex == 5) itemS = itemName;
                    else if (buttonIndex == 6) itemD = itemName; else if (buttonIndex == 7) itemF = itemName;

                    buttonIndex++;
                }
            }
        }
    }

    private void TryUseBossItem(string itemName)
    {
        // Check cooldown
        if (itemCooldownEnd.ContainsKey(itemName) && Time.time < itemCooldownEnd[itemName])
        {
            Debug.Log($"{itemName} is still on cooldown for {itemCooldownEnd[itemName] - Time.time:F1}s");
            return;
        }

        // Trigger effect
        OnBossItemEffectTrigger(itemName);

        // Start cooldown if defined
        if (itemCooldownTimes.ContainsKey(itemName))
        {
            float cd = itemCooldownTimes[itemName];
            itemCooldownEnd[itemName] = Time.time + cd;

            if (itemToButton.ContainsKey(itemName))
            {
                Button btn = itemToButton[itemName];
                btn.interactable = false;
                StartCoroutine(CooldownRoutine(btn, itemName, cd));
            }
        }
    }

    private IEnumerator CooldownRoutine(Button btn, string itemName, float duration)
    {
        Text label = btn.GetComponentInChildren<Text>();
        if (label == null) yield break;

        // Change to cooldown look
        label.color = Color.white;

        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            float remaining = endTime - Time.time;
            label.text = $"{remaining:F1}s"; // countdown format
            yield return null; // update every frame
        }

        // Restore original look
        if (originalTexts.ContainsKey(btn)) label.text = originalTexts[btn];
        if (originalColors.ContainsKey(btn)) label.color = originalColors[btn];
        btn.interactable = true;
    }

    private void OnBossItemEffectTrigger(string itemName)
    {
        // Example effect for "New Equipment"
        if (itemName == "New Equipment") { foreach (var spawner in FindObjectsOfType<Launcher>()) { if (spawner.isPlayer == true)
                    spawner.stashedProjectile.GetComponentInChildren<Spear>().NewEquipment(); }
        } else if (itemName == "Rubber Arrows") { foreach (var spawner in FindObjectsOfType<Launcher>()) { if (spawner.isPlayer == true)
                    spawner.stashedProjectile.GetComponentInChildren<Bow>().RubberArrows(); }
        } else if (itemName == "Remote Detonation") { foreach (var spawner in FindObjectsOfType<Launcher>()) { if (spawner.isPlayer == true)
                    foreach (var fireball in FindObjectsOfType<Fireball>()) { if (fireball.side == true) { fireball.damage *= 1.5f;  fireball.DestroySelf(); } } }
        }
    }

    void Update()
    {
        // Handle hotkeys (check cooldowns through TryUseBossItem)
        if (Input.GetKeyDown(KeyCode.Q) && itemQ != null) TryUseBossItem(itemQ);
        if (Input.GetKeyDown(KeyCode.W) && itemW != null) TryUseBossItem(itemW);
        if (Input.GetKeyDown(KeyCode.E) && itemE != null) TryUseBossItem(itemE);
        if (Input.GetKeyDown(KeyCode.R) && itemR != null) TryUseBossItem(itemR);
        if (Input.GetKeyDown(KeyCode.A) && itemA != null) TryUseBossItem(itemA);
        if (Input.GetKeyDown(KeyCode.S) && itemS != null) TryUseBossItem(itemS);
        if (Input.GetKeyDown(KeyCode.D) && itemD != null) TryUseBossItem(itemD);
        if (Input.GetKeyDown(KeyCode.F) && itemF != null) TryUseBossItem(itemF);
    }
}
