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
    [HideInInspector]
    public GameObject fighterPrefab;
    [HideInInspector]
    public GameObject animatorPrefab;

    public void SetSelectedPrefab1(GameObject prefabToSet1)
    {
        fighterPrefab = prefabToSet1;
    }

    public void SetSelectedPrefab2(GameObject prefabToSet2)
    {
        animatorPrefab = prefabToSet2;
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
