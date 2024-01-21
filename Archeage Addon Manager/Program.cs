using System;
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

            // Initialise our AddonDataManager
            new AddonDataManager();

            Application.Run(new MainWindow());            
        }
    }
}
