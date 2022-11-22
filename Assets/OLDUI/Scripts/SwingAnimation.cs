using System;
using UnityEngine;

public class SwingAnimation : MonoBehaviour
{
	[Range(0f, 720f)]
	public float rotationSpeed = 120f;

	[Range(0f, 90f)]
	public float rotationScale = 15f;

	[Range(0f, 720f)]
	public float scaleSpeed = 180f;

	[Range(0f, 1f)]
	public float scaleScale = 0.125f;

	private float randomStart;

	private float lsScale;

	private void Start()
	{
		randomStart = (float)UnityEngine.Random.Range(-10, 10) * 0.5f;
	}

	private void Update()
	{
		float time = GetTime();
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one * (Mathf.Sin(time * scaleSpeed * ((float)Math.PI / 180f)) * scaleScale + 1f), scaleSpeed * Time.deltaTime);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, 1f) * Mathf.Sin(time * rotationSpeed * ((float)Math.PI / 180f)) * rotationScale), rotationSpeed * Time.deltaTime);
	}

	private float GetTime()
	{
		return randomStart + Time.time;
	}
}
