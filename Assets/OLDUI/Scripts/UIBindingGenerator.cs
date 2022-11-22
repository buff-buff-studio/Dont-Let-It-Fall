using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBindingGenerator : MonoBehaviour
{
	[Serializable]
	public struct VisualBinding
	{
		public string message;

		public string inButtonText;

		public Sprite icon;

		public Binding binding;
	}

	public VisualBinding[] bindings;

	public GameObject prefab;

	private List<UIBinding> objects = new List<UIBinding>();

	public void OnEnable()
	{
		float num = 0f;
		VisualBinding[] array = bindings;
		for (int i = 0; i < array.Length; i++)
		{
			VisualBinding visualBinding = array[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, base.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = visualBinding.message;
			gameObject.transform.GetChild(1).GetComponent<Image>().sprite = visualBinding.icon;
			gameObject.transform.GetChild(1).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(visualBinding.icon.rect.width, visualBinding.icon.rect.height);
			if (visualBinding.inButtonText.Length == 0)
			{
				UnityEngine.Object.Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
			}
			else
			{
				gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
					.text = visualBinding.inButtonText;
			}
			UIBinding component = gameObject.GetComponent<UIBinding>();
			component.binding = visualBinding.binding;
			component.ReloadSize();
			objects.Add(component);
			num += component.GetComponent<RectTransform>().rect.width;
		}
		num += (float)((bindings.Length - 1) * 10);
		float num2 = (0f - num) / 2f;
		foreach (UIBinding @object in objects)
		{
			float width = @object.GetComponent<RectTransform>().rect.width;
			@object.transform.localPosition = new Vector3(num2 + width / 2f, 0f, 0f);
			num2 += width + 10f;
		}
	}

	public void OnDisable()
	{
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		objects.Clear();
	}
}
