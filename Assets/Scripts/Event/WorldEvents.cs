using System.Collections;
using System.Collections.Generic;
using DLIFR.Props;
using UnityEngine;
using UnityEngine.Events;

public class WorldEvents : MonoBehaviour
{
    public WorldEvents instance;
    
    [SerializeField]
    private bool spawnEvents = true;

    [SerializeField]
    private List<EventItem> events = new List<EventItem>();
    [SerializeField]
    private Vector2 timeBetweenEvents = new Vector2(10,20);

    [SerializeField] private Transform shipTargetRotation;
    [SerializeField] private Transform boxParent;
    public Quaternion targetRotation;
    public Vector3 v;

    private int eventIndex;
    private EventData currentEvent => events[eventIndex].EventData;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (spawnEvents) SpawnEvent();
    }

    public void SpawnEvent()
    {
        if (spawnEvents)
        {
            StartCoroutine(WiggleShip());
        }
    }

    private void FireBox()
    {
        var boxs = boxParent.GetComponentsInChildren<Cargo>();
        var box = boxs[Random.Range(0, boxs.Length)];

        box.Fire();
    }
    
    private IEnumerator WiggleShip()
    {
        float wiggleAmount = currentEvent.eventForce;
        float time = Time.time;
        
        targetRotation = Quaternion.Euler(Random.Range(-wiggleAmount, wiggleAmount),0,Random.Range(-wiggleAmount, wiggleAmount));
        
        while (Time.time <= time+2)
        {
            shipTargetRotation.rotation = Quaternion.Lerp(shipTargetRotation.rotation, targetRotation, Time.deltaTime * wiggleAmount);
            v = shipTargetRotation.eulerAngles;
            yield return new WaitForSeconds(.1f);
        }

        FireBox();
        StartCoroutine(WiggleShip());
    }
    
    private IEnumerator SpawnEventCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(timeBetweenEvents.x, timeBetweenEvents.y));
        if (!spawnEvents) yield break;
        
        var index = Random.Range(0, events.Count);
        StartCoroutine(SpawnEventCoroutine());
    }
}

[System.Serializable]
public class EventItem
{
    [SerializeField]
    private EventData eventData;
    public UnityEvent OnEventStart;
    public UnityEvent OnEventEnd;
    
    public EventData EventData { get { return eventData; } }
}
