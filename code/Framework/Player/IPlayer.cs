
namespace Framework;
public interface IPlayer
{
	ulong Steamid { get; set; }
	string DisplayName { get; set; }
	Dictionary<int, Character> Characters { get; set; }

	void Load();

	void Save();
}
