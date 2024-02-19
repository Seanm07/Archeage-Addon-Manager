using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Create or append to the config file
            using (StreamWriter sw = File.AppendText(configFile)) {
                // Write key-value pair
                sw.WriteLine($"{key}={value}");
            }
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
