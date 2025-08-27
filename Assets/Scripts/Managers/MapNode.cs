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

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        if (SceneSwitcher.Instance.currentNode == null && startingNode == true)
        {
            SceneSwitcher.Instance.currentNode = this;
            Unlock();
        }


        UpdateVisual();
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
}
