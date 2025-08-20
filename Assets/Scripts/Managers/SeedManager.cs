using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    [HideInInspector]
    public static SeedManager Instance;
    [HideInInspector]
    public string seedString = "";       // Optional: user input string

    [Header("Seed Settings")]
    public int masterSeed;               // Final seed used by the game
    public bool randomizeOnStart = true; // If false, will use seedString or last saved seed IT ONLY DOES ONE THING

    void Awake()
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

    // Initialize the master seed
    public void ApplySeed()
    {
        if (randomizeOnStart) // THIS BOOL BE ONLY USED HERE
        {
            masterSeed = Random.Range(000000000, 999999999);
        }
        else
        {
            if (string.IsNullOrEmpty(seedString))
            {
                masterSeed = Random.Range(000000000, 999999999);
            }
            else
            {
                masterSeed = ConvertStringToIntManual(seedString);
            }
        }

        Random.InitState(masterSeed);
        //Debug.Log($"[SeedManager] Master Seed: {masterSeed}");
    }

    // Generate a sub-seed for a specific system
    public int GetSubSeed(string systemName)
    {
        // Use the master seed combined with a system name hash
        return masterSeed + systemName.GetHashCode();
    }

    // Apply a sub-seed and return RNG to master
    public void UseSubSeed(string systemName)
    {
        int subSeed = GetSubSeed(systemName);
        Random.InitState(subSeed);
        //Debug.Log($"[SeedManager] Using sub-seed for {systemName}: {subSeed}");
    }

    // Restore RNG to master sequence
    public void RestoreMasterSeed()
    {
        Random.InitState(masterSeed);
        //Debug.Log($"[SeedManager] Restored master seed");
    }

    // Save seed to PlayerPrefs (or your save system)
    public void SaveSeed()
    {
        PlayerPrefs.SetInt("SavedSeed", masterSeed);
        PlayerPrefs.Save();
        Debug.Log($"[SeedManager] Seed saved: {masterSeed}");
    }

    // Load saved seed
    public void LoadSeed()
    {
        if (PlayerPrefs.HasKey("SavedSeed"))
        {
            masterSeed = PlayerPrefs.GetInt("SavedSeed");
            Random.InitState(masterSeed);
            Debug.Log($"[SeedManager] Seed loaded: {masterSeed}");
        }
        else
        {
            Debug.LogWarning("[SeedManager] No saved seed found!");
        }
    }

    public void ApplyStringSeed()
    {
        GameObject esObject = GameObject.Find("EventSystem");
        SceneSwitcher es = esObject.GetComponent<SceneSwitcher>();
        string rawInput = es.currentSeedText.text;
        rawInput = rawInput.Replace("​", string.Empty);
        rawInput = System.Text.RegularExpressions.Regex.Replace(rawInput, "[^0-9-]", "");
        seedString = rawInput.Trim();
        randomizeOnStart = false;
    }

    public int ConvertStringToIntManual(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            //Debug.LogWarning("Input string is null or empty, returning 0.");
            return 0;
        }

        s = s.Trim(); // Remove leading/trailing whitespace

        int result = 0;
        int sign = 1;
        int startIndex = 0;

        // Handle negative sign
        if (s.Length > 0 && s[0] == '-')
        {
            sign = -1;
            startIndex = 1;
        }
        // Handle positive sign (optional)
        else if (s.Length > 0 && s[0] == '+')
        {
            startIndex = 1;
        }

        for (int i = startIndex; i < s.Length; i++)
        {
            char c = s[i];

            // Check if the character is a digit
            if (c >= '0' && c <= '9') //
            {
                // Convert char digit to int value and add to result
                result = result * 10 + (c - '0'); //
                // Check for potential overflow (simplistic check)
                if (result < 0 && sign > 0) // If positive number overflows, result wraps to negative
                {
                    //Debug.LogError($"Overflow detected for string: {s}, returning int.MaxValue.");
                    return int.MaxValue;
                }
                if (result > 0 && sign < 0 && result > int.MaxValue) // If negative number overflows, result wraps to positive
                {
                    //Debug.LogError($"Overflow detected for string: {s}, returning int.MinValue.");
                    return int.MinValue;
                }
            }
            else
            {
                //Debug.LogError($"Invalid character '{c}' found at index {i}. Cannot convert string '{s}' to integer, returning 0.");
                return 0; // Invalid character found, stop and return error value
            }
        }

        return result * sign;
    }
}

//// Get consistent loot spawns
//SeedManager.Instance.UseSubSeed("LootSystem");
//
//for (int i = 0; i < 5; i++)
//{
//    int lootValue = Random.Range(0, 100);
//    Debug.Log($"Loot roll: {lootValue}");
//}
//
//// Return to main seed
//SeedManager.Instance.RestoreMasterSeed();
//
//// Now spawn enemies with a different sub-seed
//SeedManager.Instance.UseSubSeed("EnemySystem");
//for (int i = 0; i < 5; i++)
//{
//    int enemyType = Random.Range(0, 3);
//    Debug.Log($"Enemy type: {enemyType}");
//}
//SeedManager.Instance.RestoreMasterSeed();
