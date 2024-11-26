using Sandbox;
using Framework;

namespace Framework;

[ GameResource( "Item", "item", "An item for the roleplay framework" ) ] // Title, File Extension (.item), Description
public class ItemFile : GameResource
{
	[ Category( "Item Information" ), ResourceType( "vmdl" ) ] public Model Model { get; set; }
	[ Category( "Item Information" ) ] public string Name { get; set; }
	[ Category( "Item Information" ) ] public string Description { get; set; }
	[ Category( "Item Information" ) ] public string Category { get; set; }

	[ Category( "Item Information" ) ] public float Weight { get; set; } = 1f;
	[ Category( "Item Information" ) ] public int StackMax { get; set; } = 1;
	[ Category( "Item Information" ) ] public bool Stackable { get; set; } = false;

	//[ Hide ] public GameObject Prefab { get; set; } = "prefabs/item.prefab";
}
