using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignAnimation : MonoBehaviour
{
    public SceneSwitcher es;
    public GameObject stashedAnimation;
    // Start is called before the first frame update
    void Start()
    {
        GameObject esObject = GameObject.Find("EventSystem");
        es = esObject.GetComponent<SceneSwitcher>();
        GameObject animation = Instantiate(es.animatorPrefab, transform.position, Quaternion.identity);

        stashedAnimation = animation;

        animation.transform.localScale *= 1.5f;
    }
}
