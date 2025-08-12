using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    private bool speedUp = false;

    public float timeScale = 2.0f;

    // Update is called once per frame
    void Update()
    {
        //Speed up function
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (speedUp == false)
            {
                speedUp = true;
            } else {
                speedUp = false;
            }
        }

        if (speedUp == true)
        {
            Time.timeScale = timeScale;
            if (Input.GetMouseButtonDown(0))
            {
                if (timeScale <= 0.5f)
                {
                    timeScale *= (0.5f);
                }
                else { timeScale -= 0.5f; }

            }
            if (Input.GetMouseButtonDown(1))
            {
                if (timeScale <= 0.5f)
                {
                    timeScale *= (2f);
                }
                else { timeScale += 0.5f; }
            }
        } else {
            Time.timeScale = 1.0f; // Normal speed
        }
    }
}
