using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignSeedInputField : Assign
{
    public TMP_InputField seedInputField;
    public TMP_Text currentSeedText;
    
    public override void Start()
    {
        base.Start();
        es.seedInputField = seedInputField;
        es.currentSeedText = currentSeedText;
    }

    public void EmptyString()
    {
        currentSeedText.text = "";
    }
}
