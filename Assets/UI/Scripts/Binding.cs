using System;
using UnityEngine;

[Serializable]
public struct Binding
{
	public BindingType type;

	public KeyCode Key;
    public int mouseButton;
	public string[] axis;

	public float axisMinValue;

	public bool IsDown()
	{
		return Input.GetKeyDown(Key);
	}

	public bool IsHoldingDown()
	{
		return Input.GetKey(Key);
	}

	public bool IsUp()
	{
		return Input.GetKeyUp(Key);
	}

	public bool Active()
	{
		if (type == BindingType.Key)
		{
			return IsHoldingDown();
		}

        if (type == BindingType.MouseButton)
		{
			return Input.GetMouseButton(mouseButton);
		}

		string[] array = axis;
		for (int i = 0; i < array.Length; i++)
		{
			if (Mathf.Abs(Input.GetAxisRaw(array[i])) > axisMinValue)
			{
				return true;
			}
		}
		return false;
	}
}
