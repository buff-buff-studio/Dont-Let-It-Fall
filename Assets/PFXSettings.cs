using System;
using System.Collections;
using System.Collections.Generic;
using DLIFR.Data;
using UnityEngine;
using UnityEngine.Rendering;

public class PFXSettings : MonoBehaviour
{
    public Value<bool> pfxEnabled;

    private void Start()
    {
        GetComponent<Volume>().enabled = pfxEnabled;
    }
    
    public void UpdatePFXEnabled()
    {
        GetComponent<Volume>().enabled = pfxEnabled;
    }
}
