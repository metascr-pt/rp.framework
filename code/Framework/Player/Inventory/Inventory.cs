using Sandbox;

namespace Framework;
public sealed class Inventory
{
	private float MaxWeight { get; set; } = 50f;
	private float CurrentWeight { get; set; }
	private Dictionary<int, InventoryItem> Items { get; set; }

	public Inventory()
	{
		CurrentWeight = 0f;
		Items = new();
	}

	public void AddItem( Item item )
	{
		if ( CurrentWeight + item.Weight > MaxWeight ) return;

		var inventoryItem = new InventoryItem( item, 1 );
		Items.Add( Items.Count + 1, inventoryItem );
		CurrentWeight += item.Weight;
	}

	public void RemoveItem( Item item )
	{

	}
}
