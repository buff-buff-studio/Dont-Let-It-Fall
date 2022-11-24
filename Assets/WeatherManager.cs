using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeatherManager : MonoBehaviour
{
    public bool weatherEnabled = true;

    public Transform shipTargetRotation;
    public List<WeatherData> weatherData = new List<WeatherData>();
    
    private float weatherTime = 0;
    private int weatherIndex = 0;
    private Quaternion targetRotation;
    private IEnumerator wiggle;
    private EventsManager eventsManager;
    
    private WeatherData currentWeather => weatherData[weatherIndex];

    private void Awake()
    {
        TryGetComponent(out eventsManager);
        wiggle = WiggleShip();
        InitializeWeather();
    }

    private void FixedUpdate()
    {
        if (!weatherEnabled) return;
        
        if (weatherTime <= Time.time)
        {
            ChangeWeather();
        }
    }

    private void ChangeWeather()
    {
        if (!weatherEnabled) return;
        
        weatherIndex = Random.Range(0, weatherData.Count);
        StopCoroutine(wiggle);
        InitializeWeather();
    }
    
    private void InitializeWeather()
    {
        if(transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        if(currentWeather.weatherParticles != null) Instantiate(currentWeather.weatherParticles, transform);
        Debug.Log(currentWeather.name);
        StartCoroutine(wiggle);
        eventsManager.Events = currentWeather.weatherEvents;
        weatherTime = Time.time + Random.Range(currentWeather.weatherDuration.x, currentWeather.weatherDuration.y);
    }

    private IEnumerator WiggleShip()
    {
        float wiggleAmount = currentWeather.weatherWiggle;
        float time = Time.time;
        
        targetRotation = Quaternion.Euler(Random.Range(-wiggleAmount, wiggleAmount),0,Random.Range(-wiggleAmount, wiggleAmount));
        
        while (Time.time <= time+2)
        {
            shipTargetRotation.rotation = Quaternion.Lerp(shipTargetRotation.rotation, targetRotation, Time.deltaTime * wiggleAmount);
            yield return new WaitForSeconds(.1f);
        }
        
        if(wiggleAmount > 0) StartCoroutine(wiggle);
    }
}
