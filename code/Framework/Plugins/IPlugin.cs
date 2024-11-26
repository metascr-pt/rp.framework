namespace Framework;
public interface IPlugin
{
	string Name { get; }
	void Initialize();
	void OnEnable();
	void OnDisable();
}
