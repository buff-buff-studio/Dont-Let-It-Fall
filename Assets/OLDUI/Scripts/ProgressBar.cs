using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProgressBar : MonoBehaviour
{
	public float maxValue = 100f;

	public float minValue;

	public float value = 50f;

	public float syncSpeed = 5f;

	public float speedEasing = 1f;

	public float maxSpeedEasing = 10f;

	public float minSpeed;

	public bool vertical;

	public TMP_Text progress;

	private float _value;

	public RectTransform fill;

	public RectTransform fillImage;

	private RectTransform rect;

	private float _bar_anim_pos;

	private float _bar_speed = 5f;

	private void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		_value = Mathf.Lerp(_value, value, Time.deltaTime * syncSpeed);
		if (progress != null)
		{
			progress.text = Mathf.RoundToInt((_value - minValue) / (maxValue - minValue) * 100f) + "%";
		}
		float num = (vertical ? rect.rect.height : rect.rect.width) * (_value - minValue) / (maxValue - minValue);
		fill.sizeDelta = (vertical ? new Vector2(fill.sizeDelta.x, num) : new Vector2(num, fill.sizeDelta.y));
		float num2 = _bar_anim_pos * 15f % 16f;
		fillImage.offsetMin = (vertical ? new Vector2(fillImage.offsetMin.x, num2 - 16f) : new Vector2(num2 - 16f, fillImage.offsetMin.y));
		float num3 = Mathf.Clamp((value - _value) / speedEasing, 0f - maxSpeedEasing, maxSpeedEasing);
		num3 = ((minSpeed == 0f) ? num3 : Mathf.Max(minSpeed, num3));
		_bar_anim_pos += Time.deltaTime * num3;
	}

	public void SetValue(float value)
	{
		this.value = value;
	}

	public void SetValueInstantly(float value)
	{
		this.value = (_value = value);
	}
}
