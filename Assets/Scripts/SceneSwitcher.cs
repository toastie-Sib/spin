using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSwitcher : MonoBehaviour
{
    [Header("UI References")] //Seed UI Manager
    public TMP_InputField seedInputField;
    public TMP_Text currentSeedText;

    public GameObject fighterPrefab;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // If a seed was saved previously, load it
        if (PlayerPrefs.HasKey("SavedSeed"))
        {
            int savedSeed = PlayerPrefs.GetInt("SavedSeed");
            seedInputField.text = savedSeed.ToString();
            UpdateSeedText(savedSeed);
        }
        else
        {
            RandomizeSeed();
        }
    }

    public void RandomizeSeed()
    {
        if (seedInputField == null) return;
        int randomSeed = Random.Range(0 , 999999999);
        seedInputField.text = randomSeed.ToString();
        UpdateSeedText(randomSeed);
    }

    public void StartGame()
    {
        string input = seedInputField.text.Trim();

        int finalSeed;
        if (string.IsNullOrEmpty(input))
        {
            finalSeed = Random.Range(0, 999999999);
        }
        else if (!int.TryParse(input, out finalSeed))
        {
            finalSeed = input.GetHashCode(); // Allow word-based seeds
        }

        // Store the chosen seed
        PlayerPrefs.SetInt("SavedSeed", finalSeed);
        PlayerPrefs.Save();

        // Apply to SeedManager
        if (SeedManager.Instance != null)
        {
            SeedManager.Instance.masterSeed = finalSeed;
            Random.InitState(finalSeed);
        }

        Debug.Log($"[SeedUIManager] Starting game with seed: {finalSeed}");

        // Load your actual game scene
        SceneManager.LoadScene("Chapter0"); // Change to your scene name
    }

    private void UpdateSeedText(int seed)
    {
        if (currentSeedText != null)
        {
            currentSeedText.text = $"Current Seed: {seed}";
        }
    }

    public void SetSelectedPrefab(GameObject prefabToSet)
    {
        fighterPrefab = prefabToSet;
    }

    public GameObject GetSelectedPrefab()
    {
        return fighterPrefab;
    }

    // Public function to load a scene by name
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Public function to load the next scene in the Build Settings order
    public void LoadNextScene()
    {
        // Get the index of the currently active scene and increment it
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Public function to load a scene by its build index
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
