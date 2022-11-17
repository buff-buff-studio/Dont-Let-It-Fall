using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickUIController : MonoBehaviour
{
	public Image cursor;

	private float cursorSpeed = 300f;

	private UIClickable last;

	private void Start()
	{
		UIController.RestartScreenCursorPosition();
	}

	private void Update()
	{
		if (!UIController.ShowingScreenController)
		{
			return;
		}
		cursor.transform.position = UIController.GetCursorPosition();
		UIController.ScreenCursorPos.x += Input.GetAxisRaw("Horizontal") * cursorSpeed * Time.deltaTime;
		UIController.ScreenCursorPos.y += Input.GetAxisRaw("Vertical") * cursorSpeed * Time.deltaTime;
		UIController.ScreenCursorPos.x = Mathf.Clamp(UIController.ScreenCursorPos.x, 0f, Screen.width);
		UIController.ScreenCursorPos.y = Mathf.Clamp(UIController.ScreenCursorPos.y, 0f, Screen.height);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			foreach (UIContainer item in UIContainer.containerStack)
			{
				int slotInMouse = item.GetSlotInMouse();
				if (slotInMouse != -1)
				{
					item.PerformPointerDown(slotInMouse, false);
				}
			}
			last = null;
			Process();
		}
		else if (Input.GetKeyUp(KeyCode.Space) && last != null)
		{
			last.Up();
			last = null;
		}
		if (!Input.GetKeyDown(KeyCode.LeftAlt))
		{
			return;
		}
		foreach (UIContainer item2 in UIContainer.containerStack)
		{
			int slotInMouse2 = item2.GetSlotInMouse();
			if (slotInMouse2 != -1)
			{
				item2.PerformPointerDown(slotInMouse2, true);
			}
		}
	}

	public void Process()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = UIController.ScreenCursorPos;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		if (list.Count <= 0)
		{
			return;
		}
		GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(list[0].gameObject);
		pointerEventData = new PointerEventData(EventSystem.current);
		if (eventHandler != null)
		{
			Selectable componentInParent = eventHandler.GetComponentInParent<Selectable>();
			if (componentInParent != null && !componentInParent.interactable)
			{
				return;
			}
			CanvasGroup componentInParent2 = eventHandler.GetComponentInParent<CanvasGroup>();
			if (componentInParent2 != null && !componentInParent2.interactable)
			{
				return;
			}
		}
		ExecuteEvents.Execute(eventHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
		if (!(eventHandler == null))
		{
			UIClickable component = eventHandler.GetComponent<UIClickable>();
			if (component != null)
			{
				component.Down();
				last = component;
			}
		}
	}
}
