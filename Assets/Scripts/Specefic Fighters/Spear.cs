using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    private int stacks = 0;
    public override void Start()
    {
        base.Start();
        Collider collider = GetComponent<Collider>();

        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Length: ", stacks + 1, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }
    }

    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, (0.1f), 0f);

        transform.localScale = scale;

        Vector3 position = transform.localPosition;
        position -= new Vector3(0f, (0.05f), 0f);

        transform.localPosition = position;

        damage += 0.5f;

        stacks += 1;
        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Length: ", stacks + 1, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }
    }

    public void NewEquipment()
    {
        StartCoroutine(NewEquipmentDo());
    }
    
    private IEnumerator NewEquipmentDo()
    {
        for (int i = 0; i < stacks; i++)
        {
            Vector3 scale = transform.localScale;
            scale -= new Vector3(0f, (0.1f), 0f);

            transform.localScale = scale;

            Vector3 position = transform.localPosition;
            position += new Vector3(0f, (0.05f), 0f);

            transform.localPosition = position;

            damage += 0.5f;

            
            if (gatitoBlade == false)
            {
                myFighter.UpdateDynamicUI("Length: ", stacks + 1, 1);
                myFighter.UpdateDynamicUI("Damage: ", damage, 2);
            }


            yield return new WaitForSeconds(0.02f); // controls speed (0.02s = 50 FPS)
        }
        stacks = 0;
    }
}
