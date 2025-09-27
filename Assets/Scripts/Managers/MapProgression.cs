using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MapProgression : MonoBehaviour
{
    public List<MapNode> allNodes;       // All nodes in the map (assign in inspector or find at runtime)
    public MapNode finalNode;            // The final node to unlock after 7 visits

    //public bool trackVisitedNodes = false;
    //private bool hasCopied = false;

    private void Start()
    {

        //if (trackVisitedNodes == false || hasCopied == false) return;
        StartCoroutine(StartSetupChecks());
        
    }

    private IEnumerator StartSetupChecks()
    {
        
        // At start, lock the final node until we hit threshold
        if (finalNode != null)
        {
            if (SceneSwitcher.Instance.chapter2VisitedNodes.Count >= SceneSwitcher.Instance.chapter2UnlockThreshold && finalNode != null)
            {
                finalNode.SetActive(true);
            }
            else { finalNode.SetActive(false); }
        }


        // Mark all nodes as available initially
        foreach (var node in allNodes)
        {
            node.Unlock();
            foreach (var visitedNode in SceneSwitcher.Instance.chapter2VisitedNodes)
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
        if (!SceneSwitcher.Instance.chapter2VisitedNodes.Contains(node))
        {
            SceneSwitcher.Instance.chapter2VisitedNodes.Add(node);
            if (node.sceneName == "Shop" || node.sceneName == "Campfire" || node.sceneName == "Event")
                SceneSwitcher.Instance.chapter2UnlockThreshold += 1;

            node.GetComponent<MapNode>().enemyHPMult = 10 + Mathf.Round(SceneSwitcher.Instance.chapter2VisitedNodes.Count / 1.25f);
            node.GetComponent<MapNode>().OnClick();

            // Deactivate this node so it can't be visited again
            node.SetActive(false);

            // Check if we should unlock final node
            if (SceneSwitcher.Instance.chapter2VisitedNodes.Count >= SceneSwitcher.Instance.chapter2UnlockThreshold && finalNode != null)
            {
                finalNode.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Returns all nodes that are currently available (not visited and not final if locked).
    /// </summary>
    //public List<MapNode> GetAvailableNodes()
    //{
    //    List<MapNode> available = new List<MapNode>();
    //
    //    foreach (var node in allNodes)
    //    {
    //        if (!visitedNodes.Contains(node))
    //        {
    //            // If it's the final node, only include if unlocked
    //            if (node == finalNode && visitedNodes.Count < unlockThreshold)
    //                continue;
    //
    //            available.Add(node);
    //        }
    //    }
    //
    //    return available;
    //}
}