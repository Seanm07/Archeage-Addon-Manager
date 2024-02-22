using System.IO;
using System.Linq;

namespace Archeage_Addon_Manager {
    internal class ProgramManager {
        public static ProgramManager instance;

        private static readonly string configFile = "config.ini";

        public ProgramManager() {
            instance ??= this;
        }

        public static void WriteToConfigFile(string key, string value) {
            // If the config file doesn't exist, create it
            if (!File.Exists(configFile))
                File.Create(configFile).Close();

            // Read all lines from the config file
            var lines = File.ReadAllLines(configFile).ToList();

            // Check if the key already exists
            var index = lines.FindIndex(line => line.StartsWith(key + "="));

            if (index != -1) {
                // If the key exists, replace the line with the new key-value pair
                lines[index] = $"{key}={value}";
            } else {
                // If the key doesn't exist, add the new key-value pair
                lines.Add($"{key}={value}");
            }

            // Write the lines back to the config file
            File.WriteAllLines(configFile, lines);
        }

        public static string ReadFromConfigFile(string key, string defaultValue = "") {
            // If the config file doesn't exist, create it
            if (!File.Exists(configFile))
                File.Create(configFile).Close();

            // Read all lines from the config file
            string[] lines = File.ReadAllLines(configFile);

            // Search for the key and return its corresponding value
            foreach (string line in lines) {
                string[] parts = line.Split('=');
                if (parts.Length == 2 && parts[0] == key) {
                    return parts[1];
                }
            }

            WriteToConfigFile(key, defaultValue);

            return defaultValue;
        }
    }
}
