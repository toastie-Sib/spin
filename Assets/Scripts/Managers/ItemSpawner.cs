using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    private GameObject storedItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SeedManager.Instance.UseSubSeed("ItemSystem"); //generate random item

        int itemTypeIndex = Random.Range(0, itemPrefabs.Length);
        storedItemPrefab = itemPrefabs[itemTypeIndex];

        SeedManager.Instance.RestoreMasterSeed();
    }


}
