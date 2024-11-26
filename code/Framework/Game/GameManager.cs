using Sandbox;
using Framework;
using System;
using System.Collections.Generic;

namespace Framework;
public sealed class GameManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab { get; set; }

    private BaseFramework framework;

	/// <summary>
	/// The client is fully connected and completely the handshake. After this call they will close the loading screen and start playing.
	/// </summary>
	public void OnActive( Connection channel )
	{
		var playerObj = PlayerPrefab.Clone();
		//player.WorldPosition = spawn.Transform.Position;
		playerObj.BreakFromPrefab();
		playerObj.Name = channel.DisplayName;
		playerObj.Network.SetOrphanedMode( NetworkOrphaned.ClearOwner );

		var player = playerObj.GetComponent<Player>();
		player.Steamid = channel.SteamId;
		player.DisplayName = channel.DisplayName;
	}

	/// <summary>
	/// The client has connected to the server. They’re about to start handshaking, in which they’ll load the game and download all the required packages.
	/// </summary>
	public void OnConnected( Connection channel )
	{
		Log.Info( $"Player {channel.DisplayName} is connecting" );
	}

	public void OnDisconnected( Connection channel )
	{
		Log.Info( $"Player {channel.DisplayName} has disconnected" );
	}

	protected override void OnStart()
	{
		if ( !Networking.IsActive ) // Create a lobby if not connected
		{
			Networking.CreateLobby();
		}
	}

    public void StartServer()
    {
        framework = new BaseFramework();
        
        // For example, dynamically load HL2RP schema
        //IPlugin hl2rp = new HL2RP();
        //framework.LoadPlugin(hl2rp);
    }

    public void StopServer()
    {
        // Unload all plugins (schemas)
        foreach ( var plugin in framework.loadedPlugins )
        {
            framework.UnloadPlugin( plugin );
        }
    }
}
