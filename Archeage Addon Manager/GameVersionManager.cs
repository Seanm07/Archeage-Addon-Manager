using System;
using System.IO;

namespace Archeage_Addon_Manager {
    internal class GameVersionManager {
        public static GameVersionManager instance;

        public GameVersionManager() {
            instance ??= this;
        }

        public static string GetExpectedGameVersion(bool encrypted = false) {
            DateTime startDate = new DateTime(2020, 9, 30, 10, 0, 0, DateTimeKind.Utc);
            DateTime currentDate = DateTime.UtcNow;

            // If the current date is before the start date, return version 0
            if (currentDate < startDate)
                return "0";

            // Calculate the number of Thursdays after 10am UTC since the start date
            int daysSinceStart = (int)(currentDate - startDate).TotalDays;
            int weeksSinceStart = daysSinceStart / 7;
            int versionNumber = weeksSinceStart;

            // Adjust version number if today is Thursday after 10am UTC
            if (currentDate.DayOfWeek == DayOfWeek.Thursday && currentDate.Hour >= 10) {
                versionNumber++;
            }

            return encrypted ? AACipher.Encrypt(versionNumber.ToString()) : versionNumber.ToString();
        }

        public static string GetInstalledGameVersion(bool encrypted = false) {
            string versionDatPath = AddonDataManager.instance.GetActiveInstallationPath() + @"\Version.dat";
            string versionNumber = "0";

            if (File.Exists(versionDatPath)) {
                string versionDatContents = File.ReadAllText(versionDatPath);
                versionNumber = encrypted ? versionDatContents : AACipher.Decrypt(versionDatContents);
            }

            return versionNumber;
        }
    }
}
