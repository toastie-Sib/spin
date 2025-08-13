using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    [Header("Seed Settings")]
    public string seedString = "";       // Optional: user input string
    public int masterSeed;               // Final seed used by the game
    public bool randomizeOnStart = true; // If false, will use seedString or last saved seed

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

        InitSeed();
    }

    // Initialize the master seed
    public void InitSeed()
    {
        if (randomizeOnStart)
        {
            masterSeed = Random.Range(int.MinValue, int.MaxValue);
        }
        else
        {
            if (string.IsNullOrEmpty(seedString))
            {
                // Default fallback
                masterSeed = 123456;
            }
            else
            {
                masterSeed = seedString.GetHashCode();
            }
        }

        Random.InitState(masterSeed);
        Debug.Log($"[SeedManager] Master Seed: {masterSeed}");
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
        Debug.Log($"[SeedManager] Using sub-seed for {systemName}: {subSeed}");
    }

    // Restore RNG to master sequence
    public void RestoreMasterSeed()
    {
        Random.InitState(masterSeed);
        Debug.Log($"[SeedManager] Restored master seed");
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
