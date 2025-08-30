using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBans : Assign
{
    public string[] blockedFighters; // names of fighters who CANNOT use this item

    public bool CannotBeUsedBy(GameObject fighterPrefab)
    {
        string fighterName = fighterPrefab.name.Replace("(Clone)", "");
        foreach (string blocked in blockedFighters)
        {
            if (fighterName == blocked)
                return true;
        }
        return false;
    }
}
