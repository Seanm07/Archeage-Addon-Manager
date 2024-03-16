using System;
using System.IO;

namespace Archeage_Addon_Manager {
    internal class GameVersionManager {
        public static GameVersionManager instance;
        private static string latestGameVersion;

        public GameVersionManager() {
            instance ??= this;
        }

        public static async void RequestLatestGameVersion() {
            await new WebRequest().DoRequest("https://akamai-aa-gamecdn.playkakaogames.com/live/Game/Archeage/Config/config.patch.version", (string response) => {
                latestGameVersion = response;
                MainWindow.instance.SetLatestPatchLabel();
            });
        }

        public static string GetLatestGameVersion(bool encrypted = false) {
            return encrypted ? AACipher.Encrypt(latestGameVersion) : latestGameVersion;
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
