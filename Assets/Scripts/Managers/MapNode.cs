using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    public string sceneName;                     // Scene to load
    public List<MapNode> connectedNodes;         // What nodes can be reached from here
    public bool isUnlocked = false;              // Can you click this node?
    private Button button;

    public bool startingNode = false;

    [Header("Path Settings")]
    public Material lineMaterial;   // Assign a simple UI/Unlit material
    private List<LineRenderer> paths = new List<LineRenderer>();

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        DrawPaths();

        UpdateVisual();

        UpdatePathColors();


        if (SceneSwitcher.Instance.currentNode == null && startingNode == true)
        {
            SceneSwitcher.Instance.currentNode = this;
            Unlock();
        }


    }

    void OnClick()
    {
        if (isUnlocked)
        {
            // Save which node you're at
            SceneSwitcher.Instance.SetCurrentNode(this);
            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay(sceneName);
            isUnlocked = false;
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        button.interactable = isUnlocked;
    }

    private void DrawPaths()
    {
        foreach (var target in connectedNodes)
        {
            if (target == null) continue;

            GameObject lineObj = new GameObject("PathLine");
            lineObj.transform.SetParent(GameObject.Find("MapLineContainer").transform, false); // put under root Canvas

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = 0.15f; // adjust thickness
            lr.positionCount = 2;
            lr.sortingOrder = 200; // ensure above background

            // Use UI space conversion
            Vector3 startPos = GetComponentInChildren<SphereCollider>().transform.position;
            Vector3 endPos = target.GetComponentInChildren<SphereCollider>().transform.position;

            lr.useWorldSpace = true;
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);

            // Start all inactive
            lr.startColor = Color.black;
            lr.endColor = Color.black;

            paths.Add(lr);
        }
    }

    private void UpdatePathColors()
    {
        foreach (var lr in paths)
        {
            if (isUnlocked)
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.material.color = Color.red;
            }
            else
            {
                lr.startColor = Color.black;
                lr.endColor = Color.black;
                lr.material.color = Color.black;
            }
        }
    }
}
