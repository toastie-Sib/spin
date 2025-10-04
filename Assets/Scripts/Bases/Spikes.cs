using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float damage = 1f;

    public virtual void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Fighter"))
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
            otherFighter.HitDetect(damage);
        }

    }
}
