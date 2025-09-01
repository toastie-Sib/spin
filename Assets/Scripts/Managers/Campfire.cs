using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{
    public List<Campfire> mainPage;
    public List<Campfire> upgradePage;
    [HideInInspector] public Button button;
    public bool restoreHP = false;
    public bool upgradeStats = false;
    public bool upgradeHP = false;
    public bool upgradeAtkSpd = false;
    public bool upgradeDmg = false;
    public bool goBackToChapter = false;
    public bool moveUI = false;
    public bool moveUIBack = false;
    public bool moveUICamp = false;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (restoreHP == true)
        {
            foreach (var item in mainPage)
            {
                item.button.interactable = false;
            }
            SceneSwitcher.Instance.playerCurrentHP = SceneSwitcher.Instance.playerMaxHP;
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeStats == true)
        {

        } else if (upgradeHP == true)
        {
            foreach (var item in upgradePage)
            {
                item.button.interactable = false;
            }
            float heldHP = SceneSwitcher.Instance.playerMaxHP;
            SceneSwitcher.Instance.playerMaxHP += (SceneSwitcher.Instance.playerMaxHP * 0.35f);
            SceneSwitcher.Instance.playerCurrentHP += (SceneSwitcher.Instance.playerMaxHP - heldHP);
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeAtkSpd == true)
        {

        } else if (upgradeDmg == true)
        {

        } else if (goBackToChapter == true)
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
