using System.Collections;
using System.Collections.Generic;
using DLIFR.Props;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    [HideInInspector]
    public EventsManager instance;
    
    [SerializeField]
    private bool spawnEvents = true;
    [SerializeField]
    private Vector2 timeBetweenEvents = new Vector2(10,20);
    [SerializeField] 
    private Transform boxParent;
    
    [Header("VFX")]
    public EventVFX eventsVFX;

    private float snowValue;
    private float rainValue;
    private float iceValue;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (spawnEvents) SpawnEvent();
    }

    public void SpawnEvent()
    {

    }

    private void LightningBox()
    {
        var boxs = boxParent.GetComponentsInChildren<Cargo>();
        var box = boxs[Random.Range(0, boxs.Length)];
        
        Instantiate(eventsVFX.Lightning, box.transform.position + new Vector3(0,10,0), Quaternion.identity);
        box.Fire();
        Instantiate(eventsVFX.Fire, box.transform.position, Quaternion.identity, box.transform);
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
