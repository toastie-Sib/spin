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
                SlowLoadSpecificSceneDelay("ItemPick");
            }
        }
    }
}
