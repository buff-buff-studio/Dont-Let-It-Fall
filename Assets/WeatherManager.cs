using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private WeatherData currentWeather => weatherData[weatherIndex];

    private void Awake()
    {
        wiggle = WiggleShip();
    }
    
    private void ChangeWeather()
    {
        if (!weatherEnabled) return;
        targetRotation = shipTargetRotation.rotation;
        StartCoroutine(wiggle);
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
