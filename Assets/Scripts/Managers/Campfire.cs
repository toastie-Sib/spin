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

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (restoreHP)
        {
            foreach (var item in mainPage)
            {
                item.button.interactable = false;
            }
            SceneSwitcher.Instance.playerCurrentHP = SceneSwitcher.Instance.playerMaxHP;
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeStats)
        {

        } else if (upgradeHP)
        {
            foreach (var item in upgradePage)
            {
                item.button.interactable = false;
            }
            float heldHP = SceneSwitcher.Instance.playerMaxHP;
            SceneSwitcher.Instance.playerMaxHP += (SceneSwitcher.Instance.playerMaxHP * 0.35f);
            SceneSwitcher.Instance.playerCurrentHP += (SceneSwitcher.Instance.playerMaxHP - heldHP);
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay("Chapter0");
        } else if (upgradeAtkSpd)
        {

        } else if (upgradeDmg)
        {

        }
    }
}
