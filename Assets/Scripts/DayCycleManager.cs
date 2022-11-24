using System;
using System.Collections;
using System.Collections.Generic;
using DLIFR.Data;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    [Header("Lights")]
    public Light sunLight;
    private Transform sunTransform => sunLight.transform;
    private Vector3 sunAngles => sunTransform.localEulerAngles;
    
    [Header("Skybox")]
    public Material skyboxMaterial;
    public float sunsetPower = 16;

    [Header("Colors")] 
    public Color dayColor;
    public Color nightColor;
    public Color rainingColor;
    
    [Header("Position")] 
    private Coordinates maxCoordinates = new Coordinates(5,10);
    public Coordinates coordinates;
    
    [Header("Time")]
    public bool updateTime = true;
    public Value<float> timeOfDay;    
    [Range(0f,2)]
    public float timeSpeed;

    public float latitudeAngle;
    public float timeAngle;

    private bool isDay => timeOfDay > 6f && timeOfDay < 18f;

    private Vector3 WorldAngles => transform.eulerAngles;
    private float sunsetLevel = 0f;

    private void FixedUpdate()
    {
        if (updateTime)
        {
            timeOfDay.value += timeSpeed * Time.fixedDeltaTime;

            if (timeOfDay.value >= 24f)
                timeOfDay.value = 0f;
        }

        SetSunAngles();
        SkyboxChange();
    }

    private void SkyboxChange()
    {
        var rain = skyboxMaterial.GetFloat("_Rain");
        if (rain > 0) sunLight.color = Color.Lerp(sunLight.color, rainingColor, rain);
        
        switch (timeOfDay.value)
        {
            case >= 5.25f and <= 6.25f:
                sunsetLevel = Mathf.Clamp01(GetSunsetLevel(5.25f));
                //sunLight.color = Color.Lerp(dayColor, nightColor, sunsetLevel);
                break;
            
            case >= 17.8f and <= 18.8f:
                sunsetLevel = Mathf.Clamp01(GetSunsetLevel(17.5f));
                //sunLight.color = Color.Lerp(nightColor, dayColor, sunsetLevel);
                break;
            
            default:
                sunsetLevel = 0;
                sunLight.color = isDay ? dayColor : nightColor;
                sunLight.shadows = isDay ? LightShadows.Soft : LightShadows.None;
                break;
        }
        
        //skyboxMaterial.SetFloat("_SunsetGradientLevel", sunsetLevel);
    }

    private float GetSunsetLevel(float timeStart)
    {
        var sunsetTime = timeOfDay - timeStart;
        return -sunsetPower * (Mathf.Pow(sunsetTime, 2) - sunsetTime);
    }

    private Vector2 CoordToAngle(Vector2 coord)
    {
        float coordX = (coord.x * 18) + 180;
        float coordY = -coord.y * 18;
        return new Vector2(coordY, coordX);
    }

    private void OnValidate()
    {
        SetSunAngles();
        SkyboxChange();
    }

    private void SetSunAngles()
    {
        latitudeAngle = coordinates.latitude * 10;
        timeAngle = (timeOfDay * 15) - 90;
        
        transform.eulerAngles = new Vector3(0, 0, latitudeAngle);
        sunTransform.localEulerAngles = new Vector3(timeAngle, 0, 0);
    }
}

[System.Serializable]
public class Coordinates
{
    [Range(-5,5)]
    [Tooltip("Y Coordinate")]
    public float latitude;
    [Range(-10,10)]
    [Tooltip("X Coordinate")]
    public float longitude;
    
    public Vector2 GetPosition()
    {
        return new Vector3(longitude, latitude);
    }
    
    public void SetPosition(Vector3 position)
    {
        latitude = position.x;
        longitude = position.z;
    }
    
    public Coordinates(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }
}

[System.Serializable]
public class SkyColors
{
    [Header("Colors")]
    public Gradient dayColor;
    public Gradient dayLatitudeColor;
    public Gradient nightColor;

    [Range(0, .5f)] public float mergeFill;
    public float nightTime;
    
    [Header("Gradients")]
    public Gradient skyboxDayGradient;
    public Gradient skyboxNightGradient;

    public Color GetDayColor(float latitude, float maxLatitude, float timeOfDay)
    {
        mergeFill = (Mathf.Abs(latitude) / maxLatitude)*.5f;
        GradientAlphaKey[] alphaKeys = dayColor.alphaKeys;
        GradientColorKey[] colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(){color = Color.Lerp(dayColor.Evaluate(0), dayLatitudeColor.Evaluate(latitude / maxLatitude), mergeFill), time = 0f},
            new GradientColorKey(){color = Color.Lerp(dayColor.Evaluate(.5f), dayLatitudeColor.Evaluate(latitude / maxLatitude), mergeFill), time = 0.5f},
            new GradientColorKey(){color = Color.Lerp(dayColor.Evaluate(1), dayLatitudeColor.Evaluate(latitude / maxLatitude), mergeFill), time = 1f}
        };

        skyboxDayGradient.SetKeys(colorKeys, alphaKeys);
        return skyboxDayGradient.Evaluate(timeOfDay);
    }

    public Color GetNightColor(float timeOfDay, bool hdr = false)
    {
        if (timeOfDay >= 18)
            nightTime = (timeOfDay - 18)/12;
        else
            nightTime = Mathf.Clamp(timeOfDay+6, 6, 12)/12;

        Color nightEmit;
        
        if(hdr)
            nightEmit = new Color(nightColor.Evaluate(nightTime).r*12, nightColor.Evaluate(nightTime).g*12, nightColor.Evaluate(nightTime).b*12);
        else
            nightEmit = new Color(nightColor.Evaluate(nightTime).r, nightColor.Evaluate(nightTime).g, nightColor.Evaluate(nightTime).b);
        
        return nightEmit;
    }
}
