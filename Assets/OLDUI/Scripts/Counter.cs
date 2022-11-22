using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
	public int value;

	public float unitsPerSecond = 1f;

	private float _value;

	private int _pvalue;

	public string prefix = "";

	public string suffix = "";

	private TMP_Text text;

	private void Start()
	{
		text = GetComponent<TMP_Text>();
		_value = value;
		_updateText();
	}

	private void Update()
	{
		if (_value != (float)value || _pvalue != value)
		{
			_value += ((_value < (float)value) ? Mathf.Min((float)value - _value, unitsPerSecond * Time.deltaTime) : Mathf.Min(_value - (float)value, (0f - unitsPerSecond) * Time.deltaTime));
			_pvalue = value;
			_updateText();
		}
	}

	private void _updateText()
	{
		text.text = prefix + (int)_value + suffix;
	}

	public void SetValue(int value)
	{
		this.value = value;
	}

	public void SetValueInstantly(int value)
	{
		_value = value;
		this.value = value;
	}
}
