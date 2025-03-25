using System.IO;
using Newtonsoft.Json;

public class Settings
{
    public bool animationsEnabled { get; set; }

    public static Settings Load()
    {
        try
        {
            string json = File.ReadAllText("settings.json");
            return JsonConvert.DeserializeObject<Settings>(json);
        }
        catch
        {
            return new Settings { animationsEnabled = true }; // Default if file missing
        }
    }
}
