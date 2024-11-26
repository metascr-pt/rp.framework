using Sandbox;
namespace Framework;
public class Item : Component
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Category { get; set; }
	public float Weight { get; set; } = 1f;
	public int StackMax { get; set; } = 1;
	public bool Stackable { get; set; } = false;

	public GameObject itemObj { get; set; }

	public Item()
	{
		itemObj = this.GameObject;
	}

	public void Use()
	{
		OnUse();
	}

	public void Pickup()
	{
		OnPickup();
	}

	public void Drop()
	{
		OnDrop();
	}

	public void OnUse()
	{
		
	}

	public void OnPickup()
	{
		itemObj.Destroy();
	}

	public void OnDrop()
	{

	}
}
