using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.IO;

public class Settings
{
    internal string currentUser;

    public bool animationsEnabled { get; set; }

    // Add solvedPuzzlesCount to track the count of solved puzzles
    public int solvedPuzzlesCount { get; set; }

    // Load settings from the JSON file
    public static Settings Load()
    {
        try
        {
            string json = File.ReadAllText("settings.json");
            return JsonConvert.DeserializeObject<Settings>(json);
        }
        catch
        {
            return new Settings { animationsEnabled = true, solvedPuzzlesCount = 0 }; // Default if file missing
        }
    }

    // Save the current settings (including solvedPuzzlesCount) to the JSON file
    public void Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText("settings.json", json);
    }

    // Increment the solved puzzles count and save it back to the file
    public void IncrementSolvedPuzzlesCount()
    {
        solvedPuzzlesCount++;
        Save();  // Save the updated settings to the JSON file
    }
}
