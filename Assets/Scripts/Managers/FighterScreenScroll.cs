using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FighterScreenScroll : MonoBehaviour
{
    public RectTransform screenParent;   // The GameObject holding all fighter buttons
    public Button leftScroll;
    public Button rightScroll;
    public int direction = 1; // 1 = right, -1 = left
    public int currentScreen = 1;
    public int rightScreenBounds = 2;
    public float slideDistance = 2000f;
    public float slideDuration = 1.5f; // how long it takes

    public void MoveScreen()
    {
        leftScroll.interactable = false;
        rightScroll.interactable = false;
        StartCoroutine(SlideScreen());
    }

    private IEnumerator SlideScreen()
    {
        float elapsed = 0f;

        Vector3 startPos = screenParent.localPosition;
        Vector3 targetPos = startPos + new Vector3(direction * slideDistance, 0f, 0f);

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            // Smoothstep easing (ease in/out)
            float easedT = t * t * (3f - 2f * t);

            screenParent.localPosition = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        // Snap exactly to target
        screenParent.localPosition = targetPos;

        ButtonInteractableCheck();
    }

    public void ButtonInteractableCheck()
    {
        if (currentScreen == 1)
        {
            leftScroll.interactable = false;
            rightScroll.interactable = true;
        }
        else if (currentScreen == rightScreenBounds)
        {
            leftScroll.interactable = true;
            rightScroll.interactable = false;
        }
        else
        {
            leftScroll.interactable = true;
            rightScroll.interactable = true;
        }
    }

    public void ScrolledLeft()
    {
        direction = 1;
        currentScreen -= 1;
    }

    public void ScrolledRight()
    {
        direction = -1;
        currentScreen += 1;
    }
}
