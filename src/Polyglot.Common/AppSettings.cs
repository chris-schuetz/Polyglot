// Copyright (c) 2025 Christopher Schuetz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Polyglot.Common;

public static class AppSettings
{
    public static void Update<T>(T options, string sectionName, string appSettingsPath = "appsettings.json")
        where T : class
    {
        // Read the existing JSON file
        var json = File.ReadAllText(appSettingsPath);

        // Parse the JSON into a JsonNode object
        var jsonNode = JsonNode.Parse(json);

        if (jsonNode == null)
        {
            throw new ArgumentException("Unable to open / read appSettingsPath");
        }

        // Create options for serialization
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true // For pretty printing
        };

        // Replace the section with the updated options
        jsonNode[sectionName] = JsonSerializer.SerializeToNode(options, serializerOptions);

        // Serialize back to JSON
        var updatedJson = jsonNode.ToJsonString(serializerOptions);

        // Write back to the file
        File.WriteAllText(appSettingsPath, updatedJson);
    }
}
