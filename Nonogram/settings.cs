using System.IO;
using Newtonsoft.Json;

public class Settings
{
    public bool animationsEnabled { get; set; }

    public static Settings Load()
    {
        try
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }
        catch
        {
            return new Settings { animationsEnabled = false }; // Default if file missing
        }
    }
}
