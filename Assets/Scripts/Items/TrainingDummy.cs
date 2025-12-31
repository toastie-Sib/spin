using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingDummy : ItemBase
{
    private GameObject dummy;
    private BoxCollider[] spawnAreas; // size = 4
    private BoxCollider[] blockedSpawnAreas;
    [Header("Spawn Restrictions")]
    public float minDistanceFromFighters = 1.2f;
    public float dummyRadius = 0.75f; // approximate size of the dummy
    public int maxSpawnAttempts = 20;
    private Transform[] fighterSpawns;

    public override void Start()
    {
        base.Start();
        dummy = Resources.Load<GameObject>("Spawns/TrainingDummy");
        // Find specifically named objects
        GameObject spawnArea = GameObject.Find("SpawnArea");
        GameObject antiSpawnArea = GameObject.Find("AntiSpawnArea");
        //GameObject rightObj = GameObject.Find("Right");
        //GameObject topObj = GameObject.Find("Top");
        //GameObject bottomObj = GameObject.Find("Bottom");

        // Combine colliders from both into a single list
        List<BoxCollider> allColliders = new List<BoxCollider>();

        if (spawnArea != null)
            allColliders.AddRange(spawnArea.GetComponents<BoxCollider>());


        //if (rightObj != null)
        //    allColliders.AddRange(rightObj.GetComponents<BoxCollider>());
        //
        //if (topObj != null)
        //    allColliders.AddRange(topObj.GetComponents<BoxCollider>());
        //
        //if (bottomObj != null)
        //    allColliders.AddRange(bottomObj.GetComponents<BoxCollider>());

        // Assign to your array

        fighterSpawns = GameObject.FindObjectsOfType<Fighter>()
        .Select(f => f.transform)
        .ToArray();

        spawnAreas = allColliders.ToArray();

        blockedSpawnAreas = antiSpawnArea.GetComponents<BoxCollider>();

        for (int i = 0; i < stacks; i++)
        {
             Spawner();
        }
    }

    public void Spawner()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            BoxCollider box = spawnAreas[Random.Range(0, spawnAreas.Length)];
            Vector3 point = GetRandomPointInBox(box);

            if (IsValidSpawnPoint(point))
            {
                Instantiate(dummy, point, Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning("Failed to find valid spawn point for Training Dummy.");
    }

    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        Bounds b = box.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float y = Random.Range(b.min.y, b.max.y);
        float z = Random.Range(b.min.z, b.max.z);

        return new Vector3(x, y, z);
    }

    bool IsValidSpawnPoint(Vector3 point)
    {
        // Blocked / invalid spawn areas
        foreach (BoxCollider blocked in blockedSpawnAreas)
        {
            if (blocked != null && IsInsideCollider(blocked, point))
                return false;
        }

        // Distance from fighter spawns
        foreach (Transform spawn in fighterSpawns)
        {
            if (Vector3.Distance(point, spawn.position) < minDistanceFromFighters)
                return false;
        }

        // Prevent overlapping other dummies
        Collider[] hits = Physics.OverlapSphere(point, dummyRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Fighter"))
                return false;
        }

        return true;
    }
    bool IsInsideCollider(Collider col, Vector3 point)
    {
        return col.ClosestPoint(point) == point;
    }
}
