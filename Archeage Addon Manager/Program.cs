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

            // Create the temporary directory if it doesn't exist
            if(!Directory.Exists(FileUtil.TempFilePath()))
                Directory.CreateDirectory(FileUtil.TempFilePath());

            new DeveloperManager();

            Application.Run(new MainWindow());            
        }
    }
}
