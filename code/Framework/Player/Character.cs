using Sandbox;
using Framework;

namespace Framework;
public sealed class Character : Component
{
	public string Name { get; set; }
	public string Description { get; set; }
	public SkinnedModelRenderer Model { get; set; }
	public Player Player { get; set; }
	public Inventory Inventory { get; set; }

	public Character()
	{
		Inventory = new();
	}

	public void Load()
	{

	}

	public void Save()
	{

	}
}
