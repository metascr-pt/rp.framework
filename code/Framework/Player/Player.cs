using Sandbox;
using Framework;

namespace Framework;
public partial class Player : Component, IPlayer
{
	public ulong Steamid { get; set; }
	public string DisplayName { get; set; }
	public Dictionary<int, Character> Characters { get; set; }
	public PlayerState State { get; set; }

	public Player()
	{
		Characters = new();
		State = PlayerState.UNLOADED;
	}

	public Dictionary<int, Character> GetCharacters()
	{
		return Characters;
	}

	public void Load()
	{
		Characters = GetCharacters();
		foreach ( var character in Characters.Values )
		{
			character.Load();
		}
	}

	public void Save()
	{
		foreach ( var character in Characters.Values )
		{
			character.Save();
		}
	}
}
