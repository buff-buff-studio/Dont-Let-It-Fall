using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public static RectTransform grabbedObject;

	public static ItemStack grabbedItem;

	private static UIContainer grabbedItemContainer;

	private static int grabbedItemSlot;

	private static Vector3 grabbedItemOffset;

	private Image currentHoverSlot;

	private float lastHoverUpdate;

	public static List<UIContainer> containerStack = new List<UIContainer>();

	public Transform[] slots;

	public ItemStack[] items;

	public GameObject itemPrefab;

	private HingeJoint2D mouseJoint;

	private static Vector3 lastMousePos;

	private static int clickedSlot = 0;

	private static UIContainer lastClickedContainer = null;

	public virtual void OnGrabItemEnd(int slot)
	{
		OnSlotItemChange(slot);
	}

	public virtual void OnSwapItem(int slot)
	{
		OnSlotItemChange(slot);
	}

	public virtual void OnPutItem(int slot)
	{
		OnSlotItemChange(slot);
	}

	public virtual void OnSlotItemChange(int slot)
	{
	}

	public virtual void SetItem(int slot, ItemStack stack)
	{
		items[slot] = (stack.IsEmpty() ? ItemStack.EMPTY : stack);
		ReloadSlot(slot, true);
	}

	public virtual void SetItem(int slot, ItemStack stack, bool pop)
	{
		items[slot] = (stack.IsEmpty() ? ItemStack.EMPTY : stack);
		ReloadSlot(slot, pop);
	}

	public virtual ItemStack GetItem(int slot)
	{
		return items[slot];
	}

	public virtual int GetSlotCount()
	{
		return slots.Length;
	}

	public virtual void ReloadSlot(int i, bool pop)
	{
		if (slots[i].childCount > 0)
		{
			Object.Destroy(slots[i].GetChild(0).gameObject);
		}
		if (!items[i].IsEmpty())
		{
			_createItem(items[i], slots[i], true, pop).transform.localPosition = Vector3.zero;
		}
	}

	public virtual bool CanGrabItem(int slot)
	{
		return true;
	}

	public virtual int GetAmountToGrab(int slot, bool rightButton)
	{
		if (!rightButton)
		{
			return items[slot].amount;
		}
		return Mathf.CeilToInt((float)items[slot].amount / 2f);
	}

	public virtual bool CanPutItem(ItemStack itemStack, int slot)
	{
		return true;
	}

	public virtual int GetSlotMaxStackSize(int slot)
	{
		return 9999;
	}

	public virtual int GetAmountToPut(int slot, ItemStack item, bool rightButton)
	{
		if (items[slot] == ItemStack.EMPTY)
		{
			return Mathf.Min(GetSlotMaxStackSize(slot), Mathf.Min(rightButton ? 1 : item.amount, item.GetMaxStackSize()));
		}
		if (ItemStack.CanStack(items[slot], item))
		{
			return Mathf.Min(GetSlotMaxStackSize(slot) - items[slot].amount, Mathf.Min(items[slot].GetMaxStackSize() - items[slot].amount, Mathf.Min(rightButton ? 1 : item.amount, item.GetMaxStackSize())));
		}
		return 0;
	}

	public virtual void DropInWorld(ItemStack item)
	{
	}

	private void OnEnable()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (items[i].item.Length == 0)
			{
				items[i].item = "nothing";
			}
			items[i] = ItemStack.CloneWithAmount(items[i], items[i].amount);
			ReloadSlot(i, false);
		}
		containerStack.Add(this);
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		mouseJoint = gameObject.AddComponent<HingeJoint2D>();
		mouseJoint.transform.parent = base.transform;
		mouseJoint.transform.name = "HingePoint";
	}

	public int GetSlotInMouse()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
		{
			pointerId = -1
		};
		pointerEventData.position = UIController.GetCursorPosition();
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		if (list.Count > 0)
		{
			int num = 0;
			Transform[] array = slots;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == list[0].gameObject.transform)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	private void UpdateHover(bool justRemoveEffect)
	{
		if (justRemoveEffect)
		{
			if (currentHoverSlot != null)
			{
				currentHoverSlot.transform.localScale = Vector3.one;
				currentHoverSlot = null;
			}
			return;
		}
		int slotInMouse = GetSlotInMouse();
		if (slotInMouse == -1)
		{
			if (currentHoverSlot != null)
			{
				currentHoverSlot.transform.localScale = Vector3.one;
				currentHoverSlot = null;
			}
			return;
		}
		Image component = slots[slotInMouse].GetComponent<Image>();
		if (currentHoverSlot != null)
		{
			if (currentHoverSlot == component)
			{
				return;
			}
			currentHoverSlot.transform.localScale = Vector3.one;
		}
		component.transform.localScale = Vector3.one * (1f + 1f / (component.GetComponent<RectTransform>().sizeDelta.x / 5f));
		currentHoverSlot = component;
	}

	private void Update()
	{
		if (Time.time > lastHoverUpdate + 0.2f)
		{
			UpdateHover(false);
		}
		if (!(containerStack[containerStack.Count - 1] != this))
		{
			if (grabbedItem != ItemStack.EMPTY)
			{
				mouseJoint.transform.position = UIController.GetCursorPosition();
				mouseJoint.connectedBody = grabbedObject.GetComponent<Rigidbody2D>();
				grabbedObject.transform.position += UIController.GetCursorPosition() - lastMousePos;
				grabbedObject.GetComponent<Rigidbody2D>().AddTorque((0f - (UIController.GetCursorPosition().x - lastMousePos.x)) * 800f);
				float num = Mathf.Abs(UIController.GetCursorPosition().y - lastMousePos.y) * (float)((grabbedObject.GetComponent<Rigidbody2D>().angularVelocity > 0f) ? 1 : (-1));
				grabbedObject.GetComponent<Rigidbody2D>().AddTorque(num * 1500f);
			}
			lastMousePos = UIController.GetCursorPosition();
		}
	}

	private void OnDisable()
	{
		containerStack.Remove(this);
		if (grabbedItem != ItemStack.EMPTY && grabbedItemContainer == this)
		{
			PerformPointerDown(grabbedItemSlot, false);
			if (grabbedItem != ItemStack.EMPTY)
			{
				DropInWorld(grabbedItem);
				grabbedItem = ItemStack.EMPTY;
			}
		}
		Object.Destroy(mouseJoint.gameObject);
		UpdateHover(true);
	}

	public void OnPointerDown(PointerEventData data)
	{
		int slot = (clickedSlot = _getSlot(data));
		lastClickedContainer = this;
		PerformPointerDown(slot, data.button == PointerEventData.InputButton.Right);
	}

	public void PerformPointerDown(int slot, bool rightButton)
	{
		if (slot >= 0)
		{
			if (items[slot] != ItemStack.EMPTY)
			{
				if (!CanGrabItem(slot))
				{
					return;
				}
				if (grabbedItem != ItemStack.EMPTY)
				{
					if (!CanPutItem(grabbedItem, slot))
					{
						if (ItemStack.CanStack(grabbedItem, items[slot]) && grabbedItem.amount < grabbedItem.GetMaxStackSize())
						{
							items[slot].amount--;
							SetItem(slot, items[slot]);
							grabbedItem.amount++;
							Object.Destroy(grabbedObject.gameObject);
							grabbedItemContainer = this;
							grabbedItemSlot = slot;
							grabbedItemOffset = new Vector2(0f, 20f) / (slots[slot].GetComponent<RectTransform>().rect.width / 60f);
							grabbedObject = _createItem(grabbedItem, slots[slot], false, true).GetComponent<RectTransform>();
							grabbedObject.transform.parent = base.transform;
							grabbedObject.transform.localEulerAngles = Vector3.zero;
							grabbedObject.transform.position = UIController.GetCursorPosition() - grabbedItemOffset;
							_AlwaysOnTop(grabbedObject);
							OnGrabItemEnd(slot);
						}
						return;
					}
					int amountToPut = GetAmountToPut(slot, grabbedItem, rightButton);
					if (amountToPut < 1)
					{
						if (rightButton || GetAmountToGrab(slot, rightButton) < items[slot].amount || GetSlotMaxStackSize(slot) < grabbedItem.amount)
						{
							return;
						}
						if (ItemStack.CanStack(grabbedItem, items[slot]))
						{
							if (grabbedItem.amount >= grabbedItem.GetMaxStackSize())
							{
								ItemStack stack = grabbedItem;
								grabbedItem = items[slot];
								Vector2 vector = grabbedObject.transform.position;
								Object.Destroy(grabbedObject.gameObject);
								grabbedObject = _createItem(grabbedItem, base.transform, false, true).GetComponent<RectTransform>();
								grabbedObject.position = vector;
								_AlwaysOnTop(grabbedObject);
								SetItem(slot, stack);
								OnSwapItem(slot);
							}
						}
						else
						{
							ItemStack stack2 = grabbedItem;
							grabbedItem = items[slot];
							Vector2 vector2 = grabbedObject.transform.position;
							Object.Destroy(grabbedObject.gameObject);
							grabbedObject = _createItem(grabbedItem, base.transform, false, true).GetComponent<RectTransform>();
							grabbedObject.position = vector2;
							_AlwaysOnTop(grabbedObject);
							SetItem(slot, stack2);
							OnSwapItem(slot);
						}
					}
					else
					{
						items[slot].amount += amountToPut;
						grabbedItem.amount -= amountToPut;
						Vector2 vector3 = grabbedObject.transform.position;
						Object.Destroy(grabbedObject.gameObject);
						if (grabbedItem.amount < 1)
						{
							grabbedItem = ItemStack.EMPTY;
						}
						else
						{
							grabbedObject = _createItem(grabbedItem, base.transform, false, true).GetComponent<RectTransform>();
							grabbedObject.position = vector3;
							_AlwaysOnTop(grabbedObject);
						}
						SetItem(slot, items[slot]);
						OnPutItem(slot);
					}
				}
				else
				{
					int amountToGrab = GetAmountToGrab(slot, rightButton);
					grabbedItem = ((amountToGrab < 1) ? ItemStack.EMPTY : ItemStack.CloneWithAmount(items[slot], amountToGrab));
					items[slot].amount -= amountToGrab;
					SetItem(slot, items[slot]);
					grabbedItemContainer = this;
					grabbedItemSlot = slot;
					grabbedItemOffset = new Vector2(0f, 20f) / (slots[slot].GetComponent<RectTransform>().rect.width / 60f);
					grabbedObject = _createItem(grabbedItem, slots[slot], false, true).GetComponent<RectTransform>();
					grabbedObject.transform.parent = base.transform;
					grabbedObject.transform.localEulerAngles = Vector3.zero;
					grabbedObject.transform.position = UIController.GetCursorPosition() - grabbedItemOffset;
					_AlwaysOnTop(grabbedObject);
					OnGrabItemEnd(slot);
				}
			}
			else
			{
				if (grabbedItem == ItemStack.EMPTY || !CanPutItem(grabbedItem, slot))
				{
					return;
				}
				int amountToPut2 = GetAmountToPut(slot, grabbedItem, rightButton);
				if (amountToPut2 >= 1)
				{
					ItemStack stack3 = ((items[slot] == ItemStack.EMPTY) ? ItemStack.CloneWithAmount(grabbedItem, amountToPut2) : ItemStack.CloneWithAmount(grabbedItem, items[slot].amount + amountToPut2));
					Vector2 vector4 = grabbedObject.transform.position;
					grabbedItem.amount -= amountToPut2;
					Object.Destroy(grabbedObject.gameObject);
					if (grabbedItem.amount < 1)
					{
						grabbedItem = ItemStack.EMPTY;
					}
					else
					{
						grabbedObject = _createItem(grabbedItem, base.transform, false, true).GetComponent<RectTransform>();
						grabbedObject.position = vector4;
						_AlwaysOnTop(grabbedObject);
					}
					SetItem(slot, stack3);
					OnPutItem(slot);
				}
			}
		}
		else if (grabbedItem != ItemStack.EMPTY && grabbedItemContainer == this)
		{
			PerformPointerDown(grabbedItemSlot, false);
		}
	}

	private void _AlwaysOnTop(RectTransform obj)
	{
		Canvas canvas = obj.gameObject.AddComponent<Canvas>();
		canvas.overrideSorting = true;
		canvas.sortingOrder = 2;
		Rigidbody2D rigidbody2D = obj.gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.gravityScale = 100f;
		rigidbody2D.drag = 2f;
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (!(grabbedItem != ItemStack.EMPTY) || data.button != 0)
		{
			return;
		}
		foreach (UIContainer item in containerStack)
		{
			if (clickedSlot != _getSlot(data) || lastClickedContainer != item)
			{
				int slotInMouse = item.GetSlotInMouse();
				if (slotInMouse >= 0 && (slotInMouse != grabbedItemSlot || this != item))
				{
					item.PerformPointerDown((slotInMouse == -1) ? grabbedItemSlot : slotInMouse, false);
					return;
				}
			}
		}
		if (_getSlot(data) < 0)
		{
			grabbedItemContainer.PerformPointerDown(grabbedItemSlot, false);
		}
	}

	private int _getSlot(PointerEventData data)
	{
		int num = 0;
		Transform[] array = slots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == data.pointerCurrentRaycast.gameObject.transform)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private GameObject _createItem(ItemStack itemStack, Transform parent, bool applyScale, bool withPopAnimation)
	{
		GameObject gameObject = UIManager.RenderItem(itemStack, itemPrefab, parent, withPopAnimation);
		if (applyScale)
		{
			gameObject.transform.localScale = parent.GetComponent<RectTransform>().rect.width / 60f * Vector3.one;
		}
		return gameObject;
	}
}
