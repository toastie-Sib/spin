using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defunded : MonoBehaviour
{
    private int stacks;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneSwitcher.Instance.HasItem("Defunded"))
        {
            stacks = SceneSwitcher.Instance.GetItemCount("Defunded");
            Vector3 newScale = gameObject.transform.localScale;

            for (int i = 0; i < stacks; i++)
            {
                newScale.y *= 0.75f;
            }

            // Assign the modified Vector3 back to localScale
            gameObject.transform.localScale = newScale;
        }
    }

}
