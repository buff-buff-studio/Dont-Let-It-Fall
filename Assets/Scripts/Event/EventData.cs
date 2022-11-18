using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event Data", menuName = "DLIFR/Event Data")]
public class EventData : ScriptableObject
{
    [Header("Event Attributes")]
    public string eventName;
    [Tooltip("In Seconds")]
    public Vector2 eventDuration;
    public float eventForce;
    [SerializeField] [Range(1,100)]
    private int eventChance;
    
    [Header("Event Style")]
    public ParticleSystem eventParticles;
    public AudioClip eventAudio;

    [Header("Event Effects")]
    public List<GameObject> eventSpawnObjects;
    public List<GameObject> eventSpawnCreatures;
}
