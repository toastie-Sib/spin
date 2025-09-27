using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MapProgression : MonoBehaviour
{
    public List<MapNode> allNodes;       // All nodes in the map (assign in inspector or find at runtime)
    public MapNode finalNode;            // The final node to unlock after 7 visits
    private HashSet<MapNode> visitedNodes = new HashSet<MapNode>();

    public int unlockThreshold = 7;      // How many nodes before final unlocks
    public bool trackVisitedNodes = false;
    private bool hasCopied = false;

    private void Start()
    {
        if (hasCopied == false)
        {
            GameObject es = GameObject.Find("EventSystem");
            if (es.GetComponent<MapProgression>() != null) return;
            MapProgression newMapProgression = GetComponent<MapProgression>().GetCopyOf(es);
            newMapProgression.hasCopied = true;
            es.name = "EventSystem";

        }

        if (trackVisitedNodes == false || hasCopied == false) return;
        StartCoroutine(StartSetupChecks());
        
    }

    private IEnumerator StartSetupChecks()
    {
        
        // At start, lock the final node until we hit threshold
        if (finalNode != null)
        {
            if (visitedNodes.Count >= unlockThreshold && finalNode != null)
            {
                finalNode.SetActive(true);
                Debug.Log("Final node unlocked!");
            }
            else { finalNode.SetActive(false); }
        }


        // Mark all nodes as available initially
        foreach (var node in allNodes)
        {
            node.Unlock();
            foreach (var visitedNode in visitedNodes)
            {
                if (visitedNode == node)
                {
                    node.SetActive(false);
                    break;
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// Call this when a node is entered by the player
    /// </summary>
    public void VisitNode(MapNode node)
    {
        if (node == null) return;

        // If not visited yet, add it
        if (!visitedNodes.Contains(node))
        {
            visitedNodes.Add(node);
            if (node.sceneName == "Shop" || node.sceneName == "Campfire" || node.sceneName == "Event")
                unlockThreshold += 1;

            // Deactivate this node so it can't be visited again
            node.SetActive(false);

            Debug.Log($"Visited {node.name}. Total visited: {visitedNodes.Count}");

            // Check if we should unlock final node
            if (visitedNodes.Count >= unlockThreshold && finalNode != null)
            {
                finalNode.SetActive(true);
                Debug.Log("Final node unlocked!");
            }
        }
    }

    /// <summary>
    /// Returns all nodes that are currently available (not visited and not final if locked).
    /// </summary>
    public List<MapNode> GetAvailableNodes()
    {
        List<MapNode> available = new List<MapNode>();

        foreach (var node in allNodes)
        {
            if (!visitedNodes.Contains(node))
            {
                // If it's the final node, only include if unlocked
                if (node == finalNode && visitedNodes.Count < unlockThreshold)
                    continue;

                available.Add(node);
            }
        }

        return available;
    }



    
}

public static class ComponentCopier
{
    public static T GetCopyOf<T>(this T originalComponent, GameObject destinationGameObject) where T : Component
    {
        // Add a new component of the same type to the destination GameObject
        T newComponent = destinationGameObject.AddComponent<T>();

        // Get the type of the component
        System.Type type = originalComponent.GetType();

        // Get all public and non-public fields
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            // Copy the value of each field
            field.SetValue(newComponent, field.GetValue(originalComponent));
        }

        // Get all public properties
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            // Check if the property can be written to and read from
            if (property.CanWrite && property.CanRead)
            {
                // Copy the value of each property
                property.SetValue(newComponent, property.GetValue(originalComponent, null), null);
            }
        }

        return newComponent;
    }
}