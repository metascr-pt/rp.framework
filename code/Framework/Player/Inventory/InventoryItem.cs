using Sandbox;
using Framework;

namespace Framework;
public class InventoryItem : Item
{
	public Item Item { get; set; }
	public int Quantity { get; set; }

	public InventoryItem( Item item, int quantity )
	{
		Item = item;
		Quantity = quantity;
	}

	public float Weight()
	{
		return Quantity * Item.Weight;
	}
}
