using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            // Prevent both sides from triggering — only do it on the one with lower ID
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            {

                Fighter myFighter = GetComponentInParent<Fighter>();
                if (myFighter != null)
                    myFighter.ReverseDirection();

                Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
                if (otherFighter != null)
                    otherFighter.ReverseDirection();

                // Trigger impact frames
                ImpactPause.Instance.PauseForImpact(0.1f); // 0.05s = 3 frames at 60fps
            }
        }

        if (other.gameObject.CompareTag("Fighter"))
        {
            // Prevent both sides from triggering — only do it on the one with lower ID
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            {

                Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
                if (otherFighter != null)
                    otherFighter.HitDetect();

                // Trigger impact frames
                ImpactPause.Instance.PauseForImpact(0.1f); // 0.05s = 3 frames at 60fps
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            currentContacts.Remove(other);
        }
    }
}
