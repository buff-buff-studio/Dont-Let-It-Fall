using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Weather Data", menuName = "DLIFR/Weather Data")]
public class WeatherData : ScriptableObject
{
    [Header("Event Attributes")]
    public string weatherName;
    [Tooltip("In Seconds")]
    public Vector2 weatherDuration = new Vector2(10,30);
    [SerializeField] [Range(0,25)]
    public float weatherWiggle;
    [SerializeField] [Range(1,100)]
    public int weatherChance = 20;

    public EventSpawn weatherEvents;
    
    [Header("Event Style")]
    public GameObject weatherParticles;
    public AudioClip weatherAudio;
}
