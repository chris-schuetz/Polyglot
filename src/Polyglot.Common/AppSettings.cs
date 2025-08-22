using System.Text.Json;
using System.Text.Json.Nodes;

namespace Polyglot.Common;

public static class AppSettings
{
    public static void Update<T>(T options, string sectionName, string appSettingsPath = "appsettings.json") where T : class
    {
        // Read the existing JSON file
        string json = File.ReadAllText(appSettingsPath);
        
        // Parse the JSON into a JsonNode object
        JsonNode jsonNode = JsonNode.Parse(json);
        
        // Create options for serialization
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true // For pretty printing
        };
        
        // Replace the section with the updated options
        jsonNode[sectionName] = JsonSerializer.SerializeToNode(options, serializerOptions);
        
        // Serialize back to JSON
        string updatedJson = jsonNode.ToJsonString(serializerOptions);
        
        // Write back to the file
        File.WriteAllText(appSettingsPath, updatedJson);
    }
}
