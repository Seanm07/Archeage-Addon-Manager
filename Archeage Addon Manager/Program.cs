using System;
using System.IO;
using System.Windows.Forms;

namespace Archeage_Addon_Manager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string envFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");

            if (File.Exists(envFilePath)) {
                foreach (string line in File.ReadLines(envFilePath)) {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2) {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
                    }
                }
            }

            // Initialise our AddonDataManager
            new AddonDataManager();

            Application.Run(new MainWindow());            
        }
    }
}
