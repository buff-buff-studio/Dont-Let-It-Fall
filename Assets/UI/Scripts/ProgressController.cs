using UnityEngine;

public class ProgressController : MonoBehaviour
{
	public ProgressBar[] bars;

	public void SetToZero()
	{
		ProgressBar[] array = bars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].value = 0f;
		}
	}

	public void SetToOneHundred()
	{
		ProgressBar[] array = bars;
		foreach (ProgressBar obj in array)
		{
			obj.value = obj.maxValue;
		}
	}

	public void SetToRandom()
	{
		ProgressBar[] array = bars;
		foreach (ProgressBar progressBar in array)
		{
			progressBar.value = Random.Range(progressBar.minValue, progressBar.maxValue);
		}
	}
}
