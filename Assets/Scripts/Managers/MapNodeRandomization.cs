using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeRandomization : MonoBehaviour
{
    public Button[] nodeButtons;
    public string chapterNum;
    public int shopMin;
    public int shopMax;

    public int campMin;
    public int campMax;

    public int elitesMin;
    public int elitesMax;

    public int eventMin;
    public int eventMax;

    void Start()
    {
        string seedName = "ShopeNodesNum";
        seedName = seedName.Replace("Num", chapterNum);
        SeedManager.Instance.UseSubSeed(seedName);

        int shopAmount = Random.Range(shopMin, shopMax);

        // Pick unique random indices for shops
        HashSet<int> chosenShops = new HashSet<int>();

        while (chosenShops.Count < shopAmount && chosenShops.Count < nodeButtons.Length)
        {
            int randomIndex = Random.Range(0, nodeButtons.Length);
            chosenShops.Add(randomIndex);
        }

        // Update chosen buttons to say "Shop"
        foreach (int Index in chosenShops)
        {
            Text buttonText = nodeButtons[Index].GetComponentInChildren<Text>();
            nodeButtons[Index].GetComponent<Image>().color = new Color32(0, 255, 15, 255);
            if (buttonText != null)
            {
                buttonText.text = "Shop";
            }

            nodeButtons[Index].GetComponent<MapNode>().sceneName = "Shop";
        }
        SeedManager.Instance.RestoreMasterSeed();





        seedName = "CampNodesNum";
        seedName = seedName.Replace("Num", chapterNum);
        SeedManager.Instance.UseSubSeed(seedName);

        int campAmount = Random.Range(campMin, campMax);

        // Pick unique random indices for shops
        HashSet<int> chosenCamps = new HashSet<int>();

        while (chosenCamps.Count < campAmount && chosenCamps.Count < nodeButtons.Length)
        {
            int randomIndex = Random.Range(0, nodeButtons.Length);
            chosenCamps.Add(randomIndex);
        }

        // Update chosen buttons to say "Shop"
        foreach (int Index in chosenCamps)
        {
            Text buttonText = nodeButtons[Index].GetComponentInChildren<Text>();
            nodeButtons[Index].GetComponent<Image>().color = new Color32(255, 98, 0, 255);
            if (buttonText != null)
            {
                buttonText.text = "Camp";
            }

            nodeButtons[Index].GetComponent<MapNode>().sceneName = "Campfire";
        }
        SeedManager.Instance.RestoreMasterSeed();




        seedName = "EliteNodesNum";
        seedName = seedName.Replace("Num", chapterNum);
        SeedManager.Instance.UseSubSeed(seedName);

        int eliteAmount = Random.Range(elitesMin, elitesMax);

        // Pick unique random indices for shops
        HashSet<int> chosenElites = new HashSet<int>();

        while (chosenElites.Count < eliteAmount && chosenElites.Count < nodeButtons.Length)
        {
            int randomIndex = Random.Range(0, nodeButtons.Length);
            chosenElites.Add(randomIndex);
        }

        // Update chosen buttons to say "Shop"
        foreach (int Index in chosenElites)
        {
            Text buttonText = nodeButtons[Index].GetComponentInChildren<Text>();
            nodeButtons[Index].GetComponent<Image>().color = new Color32(171, 129, 233, 255);
            if (buttonText != null)
            {
                buttonText.text = "Elite";
            }

            seedName = "EliteNodeNum";
            seedName = seedName.Replace("Num", chapterNum + Index);
            SeedManager.Instance.UseSubSeed(seedName);

            int eliteChosen = Random.Range(1, 4);
            string eliteScene = "EliteArena" + SceneSwitcher.Instance.chapter + eliteChosen;
            nodeButtons[Index].GetComponent<MapNode>().sceneName = eliteScene;
            nodeButtons[Index].GetComponent<MapNode>().enemyHPMult *= 1.25f;
        }
        SeedManager.Instance.RestoreMasterSeed();




        seedName = "EventNodesNum";
        seedName = seedName.Replace("Num", chapterNum);
        SeedManager.Instance.UseSubSeed(seedName);

        int eventAmount = Random.Range(eventMin, eventMax);

        // Pick unique random indices for shops
        HashSet<int> chosenEvents = new HashSet<int>();

        while (chosenEvents.Count < eventAmount && chosenEvents.Count < nodeButtons.Length)
        {
            int randomIndex = Random.Range(0, nodeButtons.Length);
            chosenEvents.Add(randomIndex);
        }

        // Update chosen buttons to say "Shop"
        foreach (int Index in chosenEvents)
        {
            Text buttonText = nodeButtons[Index].GetComponentInChildren<Text>();
            nodeButtons[Index].GetComponent<Image>().color = new Color32(81, 231, 255, 255);
            if (buttonText != null)
            {
                buttonText.text = "Event";
            }

            nodeButtons[Index].GetComponent<MapNode>().sceneName = "Event";
        }
        SeedManager.Instance.RestoreMasterSeed();
    }

}
