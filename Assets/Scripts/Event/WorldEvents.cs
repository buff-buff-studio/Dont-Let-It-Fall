using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldEvents : MonoBehaviour
{
    [SerializeField]
    private bool spawnEvents = true; 
    
    [SerializeField]
    private List<EventItem> events = new List<EventItem>();
    [SerializeField]
    private Vector2 timeBetweenEvents = new Vector2(10,20);

    private void Awake()
    {
        if (spawnEvents) SpawnEvent();
    }

    public void SpawnEvent()
    {
        if (spawnEvents)
        {
            StartCoroutine(SpawnEventCoroutine());
        }
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
