using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{
    [Header("Campfire")]
    public List<Campfire> mainPage;
    public List<Campfire> upgradePage;
    [HideInInspector] public Button button;
    //Behold my collection of bools
    public bool restoreHP = false;
    public bool upgradeHP = false;
    public bool upgradeAtkSpd = false;
    public bool upgradeDmg = false;
    public bool goBackToChapter = false;
    public bool moveUI = false;
    public bool moveUIBack = false;
    public bool moveUICamp = false;
    public bool UItextInit = false;
    public bool MapUIInit = false;
    [Header("UI Tracking")]
    public Text hpMaxUIText;
    public Text hpCurrentText;
    public Text atkSpdText;
    public Text damageText;
    public Text extraText;
    private float spinMult;
    private float refreshInterval;

    void Start()
    {
        if (UItextInit == true) {
            if (MapUIInit == true) {
                if(hpCurrentText != null)
                { hpCurrentText.text = ("HP: " + (Mathf.Round(SceneSwitcher.Instance.playerCurrentHP)) + "/" + (Mathf.Round(SceneSwitcher.Instance.playerMaxHP))).ToString(); }
                extraText.text = ("Currancy: $" + (SceneSwitcher.Instance.playerMoney)).ToString();
            } else {
                hpMaxUIText.text = ("Max HP: " + (Mathf.Round(SceneSwitcher.Instance.playerMaxHP))).ToString();
                hpCurrentText.text = ("Current HP: " + (Mathf.Round(SceneSwitcher.Instance.playerCurrentHP))).ToString();
                damageText.text = ("Damage Increase: " + (Mathf.Round(SceneSwitcher.Instance.playerBonusDamage))).ToString();
                spinMult = SceneSwitcher.Instance.fighterPrefab.GetComponent<Fighter>().spinMult;
                if(SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>() != null)
                {
                    refreshInterval = SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>().refreshInterval;
                    for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
                    {
                        spinMult += (spinMult * 0.25f);
                        refreshInterval *= 0.90f;
                    }
                } else {
                    for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
                    {
                        spinMult += (spinMult * 0.5f);
                    }
                }
                
                atkSpdText.text = ("Spin Speed: " + (Mathf.Round(spinMult))).ToString();
                if (SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>() != null)
                    extraText.text = ("Attack Rate: " + (1 + Mathf.Round(refreshInterval))).ToString();
            }
        } else {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        //Campfire UI Navigation
        if (restoreHP == true)
        {
            foreach (var item in mainPage)
            {
                item.button.interactable = false;
            }
            SceneSwitcher.Instance.playerCurrentHP = SceneSwitcher.Instance.playerMaxHP;
            hpCurrentText.text = ("Current HP: " + (Mathf.Round(SceneSwitcher.Instance.playerCurrentHP))).ToString();
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeHP == true)
        {
            foreach (var item in upgradePage)
            {
                item.button.interactable = false;
            }
            float heldHP = SceneSwitcher.Instance.playerMaxHP;
            SceneSwitcher.Instance.playerMaxHP += (SceneSwitcher.Instance.playerMaxHP * 0.35f);
            SceneSwitcher.Instance.playerCurrentHP += (SceneSwitcher.Instance.playerMaxHP - heldHP);
            hpMaxUIText.text = ("Max HP: " + (Mathf.Round(SceneSwitcher.Instance.playerMaxHP))).ToString();
            hpCurrentText.text = ("Current HP: " + (Mathf.Round(SceneSwitcher.Instance.playerCurrentHP))).ToString();
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeAtkSpd == true)
        {
            foreach (var item in upgradePage)
            {
                item.button.interactable = false;
            }
            
            SceneSwitcher.Instance.playerBonusAtkSpd += (1);
            spinMult = SceneSwitcher.Instance.fighterPrefab.GetComponent<Fighter>().spinMult;
            if (SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>() != null)
            {
                refreshInterval = SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>().refreshInterval;
                for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
                {
                    spinMult += (spinMult * 0.25f);
                    refreshInterval *= 0.90f;
                }
            }
            else
            {
                for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
                {
                    spinMult += (spinMult * 0.5f);
                }
            }

            atkSpdText.text = ("Spin Speed: " + (Mathf.Round(spinMult))).ToString();
            if (SceneSwitcher.Instance.fighterPrefab.GetComponent<Bow>() != null)
                extraText.text = ("Attack Rate: " + (1 + Mathf.Round(refreshInterval))).ToString();
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeDmg == true)
        {
            foreach (var item in upgradePage)
            {
                item.button.interactable = false;
            }

            SceneSwitcher.Instance.playerBonusDamage += (1);
            damageText.text = ("Damage Increase: " + (Mathf.Round(SceneSwitcher.Instance.playerBonusDamage))).ToString();
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } 
        
        //Additional Campfire Utility Functions
        else if (goBackToChapter == true)
        {
            SceneSwitcher.Instance.LoadSpecificScene("Chapter0");
        } if (moveUI == true)
        {
            GameObject nM = GameObject.Find("No Money");
            nM.transform.SetParent(GameObject.Find("No Item").transform, false);
            if (moveUICamp == true)
            {
                foreach (var item in mainPage)
                {
                    item.transform.SetParent(GameObject.Find("GoAwayHolder").transform, false);
                }
            }
        } if (moveUIBack == true)
        {
            GameObject nM = GameObject.Find("No Money");
            nM.transform.SetParent(GameObject.Find("GoAwayHolder").transform, false);
            if (moveUICamp == true)
            {
                foreach (var item in mainPage)
                {
                    item.transform.SetParent(GameObject.Find("No Item").transform, false);
                }
            }
        }
    }
}
