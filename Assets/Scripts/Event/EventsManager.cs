using System;
using System.Collections;
using System.Collections.Generic;
using DLIFR.Props;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EventsManager : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;
    
    [SerializeField]
    private bool spawnEvents = true;
    [SerializeField] 
    private Transform boxParent;
    
    [SerializeField]
    private Vector2 eventTimeRange = new Vector2(5, 10);
    private float eventTime = 2;
    
    [Header("VFX")]
    public EventVFX eventsVFX;

    private EventSpawn events = new EventSpawn(false,false,false,false);
    public EventSpawn Events { set => events = value; }

    private float snowValue = 0;
    private float rainValue = 0;
    private float iceValue = 0;

    private void FixedUpdate()
    {
        if(!spawnEvents) return;
        
        if(Time.time > eventTime)
        {
            SelectEvent();
        }
        else
        {
            rainValue = events.increaseRain
                ? Mathf.Clamp01(rainValue + Time.fixedDeltaTime)
                : Mathf.Clamp01(rainValue - Time.fixedDeltaTime);
            
            snowValue = events.increaseSnow
                ? Mathf.Clamp01(snowValue + Time.fixedDeltaTime)
                : Mathf.Clamp01(snowValue - Time.fixedDeltaTime);
            
            if(snowValue > 0 && rainValue > 0) iceValue = Mathf.Clamp01(snowValue + rainValue);
            
            UpdateMaterials();
        }
    }

    private void SelectEvent()
    {
        var eventSelect = Random.Range(0, 2);
        SpawnEvent(eventSelect);
    }

    private void SpawnEvent(int e)
    {
        if(!events.spawnLightning && !events.spawnLightning) return;
        
        eventTime = Time.time + Random.Range(eventTimeRange.x, eventTimeRange.y);
        switch (e)
        {
            case 0:
                if (!events.spawnLightning)
                    SelectEvent();
                else
                    LightningBox();
                break;
            
            case 1:
                if (!events.spawnLightning)
                    SelectEvent();
                else
                    WindedBox();
                break;
            
            default:
                SelectEvent();
                break;
        }
    }

    private void LightningBox()
    {
        var boxs = boxParent.GetComponentsInChildren<Cargo>();
        var box = boxs[Random.Range(0, boxs.Length)];
        
        Instantiate(eventsVFX.Lightning, box.transform.position, Quaternion.identity, box.transform);
        box.Fire();
        Instantiate(eventsVFX.FirePrefab, box.transform.position, Quaternion.identity, box.transform);
    }

    private void WindedBox()
    {
        _spawner.Spawn(false);
    }

    private void UpdateMaterials()
    {
        foreach (var mat in eventsVFX.ShipTopMaterials)
        {
            mat.SetFloat("_SnowOpacity", snowValue);
            mat.SetFloat("_FreezeLevel", iceValue);
        }
    }
}

[System.Serializable]
public class EventVFX
{
    [Header("Particles")]
    public ParticleSystem Fire;
    public ParticleSystem Lightning;
    public ParticleSystem Explosion;
    public ParticleSystem Wind;

    [Header("Materials")] 
    public Material[] ShipTopMaterials = new Material[2];

    [Header("Prefabs")] 
    public GameObject FirePrefab;
}

[System.Serializable]
public class EventSpawn
{
    public bool spawnLightning;
    public bool spawnWindedBox;
    public bool increaseSnow;
    public bool increaseRain;
    
    public EventSpawn (bool lightning, bool windedBox, bool snow, bool rain)
    {
        spawnLightning = lightning;
        spawnWindedBox = windedBox;
        increaseSnow = snow;
        increaseRain = rain;
    }
}
