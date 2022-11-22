using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : UIClickable, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	private bool _mouseDown;

	public Sprite notPressed;

	public Sprite pressed;

	private Image image;

	private Button button;

	private void Start()
	{
		image = GetComponent<Image>();
		button = GetComponent<Button>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (button.interactable && (eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(base.transform) || eventData.pointerCurrentRaycast.gameObject.transform == base.transform))
		{
			Down();
		}
	}

	public override void Down()
	{
		_mouseDown = true;
		image.sprite = pressed;
		base.transform.position += new Vector3(0f, -2f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Up();
	}

	public override void Up()
	{
		if (_mouseDown)
		{
			_mouseDown = false;
			image.sprite = notPressed;
			base.transform.position += new Vector3(0f, 2f);
		}
	}
}
