using System.Collections.Generic;

public class Item
{
	private static Dictionary<string, Item> ITEMS = new Dictionary<string, Item>();

	public static Item NOTHING = new Item("nothing", "nothing");

	public static Item GEM = new Item("gem", "gem");

	public static Item EGG = new Item("egg", "egg");

	public static Item POTION = new Item("potion", "potion");

	public static Item SHOVEL = new ItemShovel("shovel", "shovel");

	private string id;

	private string unlocalizedName;

	public static Item GetItem(string id)
	{
		if (!ITEMS.ContainsKey(id))
		{
			return NOTHING;
		}
		return ITEMS[id];
	}

	public Item(string id, string unlocalizedName)
	{
		this.id = id;
		this.unlocalizedName = unlocalizedName;
		ITEMS[id] = this;
	}

	public virtual bool HasDurability()
	{
		return false;
	}

	public virtual int GetMaxDurability()
	{
		return 0;
	}

	public virtual int GetMaxStackSize()
	{
		return 16;
	}

	public virtual string GetUnlocalizedName()
	{
		return unlocalizedName;
	}

	public string GetId()
	{
		return id;
	}
}
