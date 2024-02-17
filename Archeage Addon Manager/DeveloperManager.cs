using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using ZstdSharp;
using ZstdSharp.Unsafe;

namespace Archeage_Addon_Manager {
    internal class DeveloperManager {
        public static DeveloperManager instance;

        public bool isLoggedIn;
        public bool isDeveloper;

        public DeveloperManager() {
            instance ??= this;
        }

        public async Task BackupGamePak(string installationPath) {
            MainWindow.instance.DisplayLoadingOverlay("Backing up game files..", "Compressing backup of game_pak.. (0.00%)");
            await Task.Delay(1);

            // Make the "game_pak_backups" if it doesn't exist
            if (!Directory.Exists(installationPath + @"\game_pak_backups"))
                Directory.CreateDirectory(installationPath + @"\game_pak_backups");

            // Read the file contents of installationPath + @"\Version.dat" to get the version number, or set it to 0 if file doesn't exist
            string versionDatPath = installationPath + @"\Version.dat";
            string versionNumber = "0";

            if (File.Exists(versionDatPath)) {
                string versionDatContents = File.ReadAllText(versionDatPath);
                versionNumber = AACipher.Decrypt(versionDatContents);
            }

           // MessageBox.Show(versionNumber);

            // Compress the file at installationPath + @"\game_pak" using zstandard compression
            string gamePakPath = installationPath + @"\game_pak";
            string backupPath = installationPath + @"\game_pak_backups\" + versionNumber;

            // Check if the file at backupPath already exists, if it does append _1, _2, _3, etc to the end of the file name until it doesn't exist
            
            for (int backupIndex = 1; File.Exists(backupPath); backupIndex++)
                backupPath = installationPath + @"\game_pak_backups\" + versionNumber + "_" + backupIndex;

            using FileStream input = File.OpenRead(gamePakPath);
            using FileStream output = File.OpenWrite(backupPath);
            using CompressionStream compressionStream = new CompressionStream(output, 1);

            // Enable multithreading for faster compression
            compressionStream.SetParameter(ZSTD_cParameter.ZSTD_c_nbWorkers, Environment.ProcessorCount);

            // 10MB buffer (higher = more compression per frame, lower = less compression per frame)
            // (too high and program will lag and be even slower than a lower buffer)
            int bufferSize = 1024 * 1024 * 10; 

            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long totalBytesRead = 0;
            long totalBytes = input.Length;

            while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                await compressionStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                MainWindow.instance.DisplayLoadingOverlay("Backing up game files..", "Compressing backup of game_pak.. (" + ((float)totalBytesRead / totalBytes * 100).ToString("N2") + "%)");
            }

            MainWindow.instance.DisplayLoadingOverlay("Backing up game files..", "gam_pak backup complete!");
            await Task.Delay(500);

            MainWindow.instance.CloseLoadingOverlay();
        }

        public async Task RestoreGamePak(string backupFile) {
            MainWindow.instance.DisplayLoadingOverlay("Restore game files from backup..", "Decompressing backed up game_pak.. (0.00%)");
            await Task.Delay(1);

            // Compress the file at installationPath + @"\game_pak" using zstandard compression
            string gamePakPath = AddonDataManager.instance.GetActiveInstallationPath() + @"\game_pak";
            string backupPath = backupFile;

            // 10MB buffer (higher = more compression per frame, lower = less compression per frame)
            // (too high and program will lag and be even slower than a lower buffer)
            int bufferSize = 1024 * 1024 * 10;

            if(FileUtil.IsFileLocked(gamePakPath)) {
                MainWindow.instance.ShowMessagePopup("Failed to restore game_pak", "game_pak is currently in use by another process. Please close the game and try again.", "OK");
                return;
            }

            using FileStream input = File.OpenRead(backupPath);
            using FileStream output = File.OpenWrite(gamePakPath);
            using DecompressionStream decompressionStream = new DecompressionStream(input);

            // Enable multithreading for faster compression
            //decompressionStream.SetParameter(ZSTD_dParameter.ZSTD_c_nbWorkers, Environment.ProcessorCount);

            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long totalBytesRead = 0;
            long totalBytes = input.Length;

            while ((bytesRead = await decompressionStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                await output.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                MainWindow.instance.DisplayLoadingOverlay("Restore game files from backup..", "Decompressing backed up game_pak.. (" + ((float)totalBytesRead / totalBytes * 100).ToString("N2") + "%)");
            }

            MainWindow.instance.DisplayLoadingOverlay("Restore game files from backup..", "gam_pak restore complete!");
            await Task.Delay(500);

            MainWindow.instance.CloseLoadingOverlay();
        }

        public void OpenURL(string url) {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        public void LoginButtonClick() {
            OpenURL("https://discord.com/api/oauth2/authorize?client_id=1203510156529369178&response_type=code&redirect_uri=https%3A%2F%2Fwww.spacemeat.space%2Faamods%2Fauth.php&scope=guilds");

            MainWindow.instance.DisplayLoginOverlay();
        }

        public void LoginLinkButtonConfirm(string discordLinkCode) {
            MainWindow.instance.CloseLoginOverlay();

            MessageBox.Show("TODO auth discord with code " + discordLinkCode + " for now we're logging you in as a developer for testing", "TODO");
            
            // TODO: This is temporary for testing, remove this when the discord auth is implemented
            isLoggedIn = true;
            isDeveloper = true;

            MainWindow.instance.UpdateDeveloperButtonState();
        }

        public async void UploadAddonButtonClick(string installationPath) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() {
                Description = "Select root directory of addon source"
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                AddonDataManager.instance.GetAddonSrcInfo(selectedFolder, out AddonDataManager.AddonData addonInfo, out List<string> filePaths);

                // TODO: Make a nicer looking form for the user to enter all the info at once while the addon packs in the background
                addonInfo.name = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for your addon", "Addon Name", Path.GetFileName(selectedFolder));
                addonInfo.description = Microsoft.VisualBasic.Interaction.InputBox("Enter a description for your addon", "Addon Description", "No description provided");
                addonInfo.version = float.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter a version number for your addon", "Addon Version", "1.00"));
                addonInfo.author = Environment.UserName;
                addonInfo.packagedFileName = addonInfo.name.Replace(" ", "_").ToLower();

                string jsonOutput = AddonDataManager.instance.CreateJsonForFolder(addonInfo);

                // Create a text file named addon.json at FileUtil.TempFilePath() + "addon.json" and write the jsonOutput to it
                string jsonPath = FileUtil.TempFilePath() + "addon.json";
                File.WriteAllText(jsonPath, jsonOutput);

                MessageBox.Show("These scripts were found in your addon!\nThey'll be extracted from your game_pak as a backup.\n\n" + string.Join("\n", filePaths));

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Generating " + addonInfo.packagedFileName + " pak");
                await Task.Delay(1); // Wait a frame to allow the loading overlay to display before the pak generation begins

                try {
                    if (!PakManager.GeneratePakFile(selectedFolder, "mod"))
                        throw new IOException("Failed to generate addon pak file!");

                    if (!PakManager.GenerateUninstallPakFile(installationPath + @"\game_pak", filePaths.ToArray(), "default"))
                        throw new IOException("Failed to generate uninstall pak file!");

                    string[] addonFiles = [
                        FileUtil.TempFilePath() + "addon.json",
                        FileUtil.TempFilePath() + "mod.pak",
                        FileUtil.TempFilePath() + "default.pak"
                    ];

                    // Move the addon files into a zip
                    if (!FileUtil.CreateZipFile(addonFiles, FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip"))
                        throw new IOException("Failed to build addon zip file!");
                } catch (IOException ex) {
                    MainWindow.instance.ShowMessagePopup("Failed to package addon!", ex.Message, "Retry", () => UploadAddonButtonClick(installationPath), "Cancel");
                    return;
                }

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Uploading " + addonInfo.packagedFileName + " to cloud");

                WebRequest webRequest = new WebRequest();
                await webRequest.UploadZipFile(FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip");

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Cleaning up temporary files");
                await Task.Delay(500); // half a second delay so the loading text is readable

                // Cleanup the json file
                File.Delete(jsonPath);

                // Cleanup the pak files
                File.Delete(FileUtil.TempFilePath() + "mod.pak");
                File.Delete(FileUtil.TempFilePath() + "default.pak");

                // Cleanup the zip file
                File.Delete(FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip");

                AddonDataManager.instance.ReloadAddonsFromDataSources();

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Done! " + addonInfo.packagedFileName + " has successfully been published.");
                await Task.Delay(500); // half a second delay so the loading text is readable

                MainWindow.instance.CloseLoadingOverlay();
            }
        }

    }
}
