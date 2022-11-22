using UnityEngine;
using UnityEngine.UI;

public class UIItemStack : MonoBehaviour
{
	public Image icon;

	public Sprite[] sprites;

	public Image progressFill;

	public void ShouldPop()
	{
		icon.transform.localScale = Vector3.zero;
	}

	public void SetIcon(string id)
	{
		switch (id)
		{
		case "gem":
			icon.sprite = sprites[1];
			break;
		case "potion":
			icon.sprite = sprites[2];
			break;
		case "egg":
			icon.sprite = sprites[3];
			break;
		case "shovel":
			icon.sprite = sprites[4];
			break;
		default:
			icon.sprite = sprites[0];
			break;
		}
	}

	public void SetDurability(bool hasDurability, float durability)
	{
		if (!hasDurability)
		{
			Object.Destroy(progressFill.transform.parent.gameObject);
			return;
		}
		progressFill.GetComponent<RectTransform>().sizeDelta = new Vector2(progressFill.transform.parent.GetComponent<RectTransform>().rect.width * durability, progressFill.GetComponent<RectTransform>().sizeDelta.y);
		progressFill.color = Color.Lerp(Color.green, Color.red, 1f - durability);
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		icon.transform.localScale = Vector3.Lerp(icon.transform.localScale, Vector3.one, Time.deltaTime * 32f);
	}
}
