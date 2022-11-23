using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBinding : MonoBehaviour
{
	public Binding binding;

	public Image image;

	public TMP_Text text;

	private float scale = 1.075f;

	private float scaleSpeed = 20f;

	private bool isButton;

	public bool autoSize;

	private void Start()
	{
		if (autoSize)
		{
			ReloadSize();
		}
	}

	public void ReloadSize()
	{
		if (text != null && !(text.preferredWidth < 0f))
		{
			float width = image.GetComponent<RectTransform>().rect.width;
			float x = text.preferredWidth + 20f + width;
			image.rectTransform.anchoredPosition = new Vector2(-6f, 0f);
			GetComponent<RectTransform>().sizeDelta = new Vector2(x, GetComponent<RectTransform>().sizeDelta.y);
		}
	}

	private void Update()
	{
		if (binding.Active())
		{
			if (binding.IsDown())
			{
				UIButton component = GetComponent<UIButton>();
				CanvasGroup componentInParent = GetComponentInParent<CanvasGroup>();
				if (componentInParent != null && !componentInParent.interactable)
				{
					return;
				}
				if (component != null)
				{
					isButton = true;
					component.Down();
				}
			}
			if (!isButton)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, scale * Vector3.one, Time.unscaledDeltaTime * scaleSpeed);
			}
			return;
		}
		if (binding.IsUp())
		{
			Button component2 = GetComponent<Button>();
			if (component2 != null)
			{
				CanvasGroup componentInParent2 = GetComponentInParent<CanvasGroup>();
				if (componentInParent2 != null && !componentInParent2.interactable)
				{
					return;
				}
				if (component2.IsInteractable())
				{
					component2.onClick.Invoke();
					UIButton component3 = GetComponent<UIButton>();
					if (component3 != null)
					{
						component3.Up();
					}
				}
			}
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.unscaledDeltaTime * scaleSpeed);
	}
}
