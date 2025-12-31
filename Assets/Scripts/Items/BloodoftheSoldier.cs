using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheSoldier : ItemBase
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();

    public override void Start()
    {
        base.Start();
        Collider collider = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myFighter.GetComponent<Bow>() == null && myFighter.isPlayer != otherFighter.isPlayer && other.GetComponent<Turret>() == null)
            {
                for (int i = 0; i < stacks; i++)
                {
                    Vector3 scale = transform.localScale;
                    scale += new Vector3((0.001f), (0.025f), 0f);

                    transform.localScale = scale;

                    Vector3 position = transform.localPosition;
                    position -= new Vector3(0f, (0.0125f), 0f);

                    transform.localPosition = position;
                }



            }

        }


        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }
}
