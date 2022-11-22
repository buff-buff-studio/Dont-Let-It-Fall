using System;

[Serializable]
public struct ItemStack
{
	public static ItemStack EMPTY = new ItemStack("nothing", 0, 0);

	public int amount;

	public string item;

	private Item _itemRef;

	public int durability;

	public static ItemStack CloneWithAmount(ItemStack stack, int amount)
	{
		return new ItemStack(stack.item, amount, stack.durability);
	}

	public ItemStack(Item item, int amount, int durability)
	{
		this.item = item.GetId();
		this.amount = amount;
		this.durability = durability;
		_itemRef = item;
	}

	public ItemStack(string item, int amount, int durability)
	{
		this.item = item;
		this.amount = amount;
		this.durability = durability;
		_itemRef = Item.GetItem(item);
	}

	public bool HasDurabilityBar()
	{
		return _itemRef.HasDurability();
	}

	public float GetDurabilityLevel()
	{
		return (float)durability / ((float)GetMaxDurability() * 1f);
	}

	public int GetMaxDurability()
	{
		return _itemRef.GetMaxDurability();
	}

	public static bool CanStack(ItemStack a, ItemStack b)
	{
		return a.item == b.item;
	}

	public int GetMaxStackSize()
	{
		if (!HasDurabilityBar())
		{
			return _itemRef.GetMaxStackSize();
		}
		return 1;
	}

	public string GetUnlocalizedName()
	{
		return _itemRef.GetUnlocalizedName();
	}

	public bool IsEmpty()
	{
		if (amount != 0)
		{
			return item == "nothing";
		}
		return true;
	}

	public static bool operator ==(ItemStack a, ItemStack b)
	{
		if ((a.amount != 0 || b.amount != 0) && (!(a.item == "nothing") || !(b.item == "nothing")))
		{
			if (a.item == "nothing")
			{
				return a.amount == b.amount;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(ItemStack a, ItemStack b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return this == (ItemStack)obj;
	}

	public override int GetHashCode()
	{
		return (item.GetHashCode() << 8) | amount;
	}
}
