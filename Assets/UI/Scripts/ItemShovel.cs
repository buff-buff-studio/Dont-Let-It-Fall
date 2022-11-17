public class ItemShovel : Item
{
	public ItemShovel(string id, string unlocalizedName)
		: base(id, unlocalizedName)
	{
	}

	public override bool HasDurability()
	{
		return true;
	}

	public override int GetMaxDurability()
	{
		return 100;
	}
}
