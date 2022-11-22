using System.Collections;
using UnityEngine;

public class UIDialog : MonoBehaviour
{
	public CanvasGroup box;

	public DialogStartMode mode;

	private CanvasGroup background;

	private void Start()
	{
		background = GetComponent<CanvasGroup>();
		if (mode == DialogStartMode.CLOSED)
		{
			__hide();
		}
		else if (mode == DialogStartMode.OPENING)
		{
			Open();
		}
		else if (mode == DialogStartMode.OPEN)
		{
			__show();
		}
	}

	private void OnEnable()
	{
		background = GetComponent<CanvasGroup>();
	}

	public void Close()
	{
		if (box != null)
		{
			box.interactable = false;
		}
		else if (background != null)
		{
			background.interactable = true;
		}
		StartCoroutine(_animClose());
	}

	public void Open()
	{
		base.gameObject.SetActive(true);
		if (background != null)
		{
			background.blocksRaycasts = true;
		}
		StartCoroutine(_animOpen());
	}

	private IEnumerator _animClose()
	{
		if (box != null)
		{
			box.interactable = false;
		}
		if (background != null)
		{
			background.interactable = false;
		}
		if (box != null)
		{
			Vector3 pre = box.transform.localScale;
			while (box.alpha > 0f)
			{
				box.alpha -= Mathf.Min(box.alpha, Time.deltaTime * 2f);
				box.transform.localScale = pre * box.alpha;
				yield return new WaitForEndOfFrame();
				if (box.alpha <= 0.5f && background != null)
				{
					background.alpha = box.alpha * 2f;
				}
			}
		}
		else if (background != null)
		{
			Vector3 localScale = background.transform.localScale;
			while (background.alpha > 0f)
			{
				background.alpha -= Mathf.Min(background.alpha, Time.deltaTime * 4f);
				yield return new WaitForEndOfFrame();
			}
		}
		__hide();
		base.gameObject.SetActive(false);
	}

	private IEnumerator _animOpen()
	{
		__hide();
		Vector3 pre = Vector3.one;
		if (box != null)
		{
			box.interactable = false;
		}
		if (background != null)
		{
			background.interactable = false;
		}
		if (box != null)
		{
			while (box.alpha < 1f)
			{
				box.alpha += Mathf.Min(1f - box.alpha, Time.deltaTime * 2f);
				box.transform.localScale = pre * box.alpha;
				yield return new WaitForEndOfFrame();
				if (background != null)
				{
					background.alpha = Mathf.Clamp(box.alpha * 2f, 0f, 1f);
				}
			}
		}
		else if (background != null)
		{
			while (background.alpha < 1f)
			{
				background.alpha += Mathf.Min(1f - background.alpha, Time.deltaTime * 4f);
				yield return new WaitForEndOfFrame();
			}
		}
		__show();
	}

	private void __hide()
	{
		if (box != null)
		{
			box.blocksRaycasts = false;
			box.alpha = 0f;
			box.interactable = false;
		}
		if (background != null)
		{
			background.blocksRaycasts = false;
			background.alpha = 0f;
			background.interactable = false;
		}
	}

	private void __show()
	{
		if (box != null)
		{
			box.blocksRaycasts = true;
			box.alpha = 1f;
			box.interactable = true;
		}
		if (background != null)
		{
			background.blocksRaycasts = true;
			background.alpha = 1f;
			background.interactable = true;
		}
	}
}
