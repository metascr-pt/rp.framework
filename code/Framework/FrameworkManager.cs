using Sandbox;

namespace Framework;
public class BaseFramework
{
	public string name { get; set; } = "";
	public string font { get; set; } = "";
	public string genericFont { get; set; } = "";
	public string language { get; set; } = "EN";
	
    public List<IPlugin> loadedPlugins = new List<IPlugin>();
	public IPlugin activeSchema;

	public void LoadSchema( IPlugin schema )
	{
		activeSchema?.OnDisable(); // Disable the current schema
		activeSchema = schema;
		activeSchema.OnEnable();   // Enable the new schema
	}

	public void UnloadSchema()
	{
		activeSchema?.OnDisable();
		activeSchema = null;
	}

	public void LoadPlugin(IPlugin plugin)
    {
        plugin.Initialize();
        loadedPlugins.Add(plugin);
        plugin.OnEnable();
    }

    public void UnloadPlugin(IPlugin plugin)
    {
        plugin.OnDisable();
        loadedPlugins.Remove(plugin);
    }

    public void Initialize()
    {
        // Initialize the framework (load any necessary resources, settings, etc.)
    }

	public static Player FindLocalPlayer()
	{
		return Game.ActiveScene.GetAllComponents<Player>().Where( x => !x.IsProxy ).FirstOrDefault();;
	}
}
