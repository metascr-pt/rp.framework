using Sandbox;

namespace Framework;
public class Commands
{
	public static ItemFile GetItemByName( string name )
	{
		var Items = ResourceLibrary.GetAll<ItemFile>();
		ItemFile Item = null;
		foreach ( var item in Items )
		{
			if ( item.Name.ToLower() != name ) continue;
			Item = item;
		}
		return Item;
	}

	[ConCmd( "spawn_item" )]
	public static void SpawnItem( string input )
	{
		var player = BaseFramework.FindLocalPlayer();
		var playerHead = player.Head;
		var eyeAngles = player.eyeAngles.ToRotation();

		Vector3 spawnPosition = playerHead.WorldPosition + ( eyeAngles.Forward * 1000f );
		var tr = Game.SceneTrace.Ray( playerHead.WorldPosition, spawnPosition ).Run();

		if ( tr.Hit ) spawnPosition = tr.HitPosition;
		var item = GetItemByName( input.ToLower() );

		if ( item == null )
		{
			Log.Info( $"Item {input} not found" );
			return;
		}

		GameObject itemObj = new();
		itemObj.SetPrefabSource( "prefabs/item.prefab" );
		itemObj.UpdateFromPrefab();
		itemObj.WorldPosition = spawnPosition;
		itemObj.BreakFromPrefab();
		itemObj.Name = item.Name;

		var itemComponent = itemObj.Components.Get<Item>();
		itemComponent.Name = item.Name;
		itemComponent.Description = item.Description;
		itemComponent.Category = item.Category;
		itemComponent.Weight = item.Weight;
		itemComponent.StackMax = item.StackMax;
		itemComponent.Stackable = item.Stackable;

		var rendererComponent = itemObj.Components.Get<ModelRenderer>();
		rendererComponent.Model = item.Model;

		var colliderComponent = itemObj.Components.Get<ModelCollider>();
		colliderComponent.Model = item.Model;

		Log.Info( $"Spawn Item: {input} at {spawnPosition}" );
	}
}
