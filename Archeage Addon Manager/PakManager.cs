using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace Archeage_Addon_Manager {
    public class PakManager {
        public static bool GenerateUninstallPakFile(string gamePakPath, string[] targetFilePaths, string pakFilename) {
            bool filesExtracted = true;

            // If the input filenames are .lua, change them to .alb
            targetFilePaths = targetFilePaths.Select(path => path.Replace(".lua", ".alb")).ToArray();

            string tempCreatedDir = FileUtil.TempFilePath() + "uninstall_src";

            // Delete the temp directory if it still exists from a previous extract
            FileUtil.DeleteDirectory(tempCreatedDir);

            // Create the temp directory for the files to be placed into
            Directory.CreateDirectory(tempCreatedDir);

            try {
                // Create the temporary file system
                if (!XLPack.CreateFileSystem())
                    throw new IOException("Failed to create temporary file system!");

                // Mount the generated pak file into the /pak directory of the temporary file system
                if (XLPack.Mount("/src", gamePakPath, true) == IntPtr.Zero)
                    throw new IOException("Failed to mount game pak to temporary file system!");

                // Mount the addon source directory into the /dir directory of the temporary file system
                if (XLPack.Mount("/dst", tempCreatedDir + @"\", true) == IntPtr.Zero)
                    throw new IOException("Failed to mount directory to temporary file system!");

                // Iterate through each target file path to copy each file from the game pak
                foreach (string targetFilePath in targetFilePaths) {
                    if (!XLPack.Copy("src" + targetFilePath, "dst" + targetFilePath))
                        throw new IOException("Failed to copy directory into pak within temporary file system!\n" + "src" + targetFilePath + " -> " + "dst" + targetFilePath);
                }
            } catch (IOException ex) {
                filesExtracted = false;

                if (MessageBox.Show(ex.Message, "Failed to extract from game pak!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    filesExtracted = GenerateUninstallPakFile(gamePakPath, targetFilePaths, pakFilename);
            }

            // Destroy the temporary file system regardless of success or failure
            XLPack.DestroyFileSystem();

            if (filesExtracted) {
                // Rename all .alb files to .lua recursively within the directory
                FileUtil.ChangeFileExtensions(tempCreatedDir, "alb", "lua");

                filesExtracted = GeneratePakFile(tempCreatedDir, pakFilename);
            }

            // Delete the temporary directory if it was created
            FileUtil.DeleteDirectory(tempCreatedDir);

            return filesExtracted;
        }

        public static bool GeneratePakFile(string selectedFolder, string pakFilename) {
            bool pakGenerated = true;

            // The generated pak will be created in the temp directory
            string pakPath = FileUtil.TempFilePath() + pakFilename + ".pak";
            string tempCreatedDir = FileUtil.TempFilePath() + "generated_pak_" + pakFilename;

            // Delete the temporary directory if it already exists from a previous mod
            FileUtil.DeleteDirectory(tempCreatedDir);

            // Copy the addon source directory to the temporary directory
            FileUtil.CopyDirectory(selectedFolder, tempCreatedDir);

            // Rename all .lua files to .alb recursively within the temporary directory
            FileUtil.ChangeFileExtensions(tempCreatedDir, "lua", "alb");

            // If only 1 of scriptsbin or scriptsbin64 exists, duplicate it in the pak
            if (Directory.Exists(tempCreatedDir + @"\game\scriptsbin") && !Directory.Exists(tempCreatedDir + @"\game\scriptsbin64")) {
                FileUtil.CopyDirectory(tempCreatedDir + @"\game\scriptsbin", tempCreatedDir + @"\game\scriptsbin64");
            } else if (!Directory.Exists(tempCreatedDir + @"\game\scriptsbin") && Directory.Exists(tempCreatedDir + @"\game\scriptsbin64")) {
                FileUtil.CopyDirectory(tempCreatedDir + @"\game\scriptsbin64", tempCreatedDir + @"\game\scriptsbin");
            }

            try {
                // Create the temporary file system
                if (!XLPack.CreateFileSystem())
                    throw new IOException("Failed to create temporary file system!");

                // Generate a blank template pak which the addon will be built into
                if (!XLPack.CreatePak(pakPath, false))
                    throw new IOException("Failed to generate blank template pak file!");

                // Mount the generated pak file into the /pak directory of the temporary file system
                if (XLPack.Mount("/pak", pakPath, true) == IntPtr.Zero)
                    throw new IOException("Failed to mount pak to temporary file system!");

                // Mount the addon source directory into the /dir directory of the temporary file system
                if (XLPack.Mount("/dir", tempCreatedDir + @"\", true) == IntPtr.Zero)
                    throw new IOException("Failed to mount directory to temporary file system!");

                // Copy the /dir directory into the /pak directory within the temporary file system
                if (!XLPack.CopyDir("/dir", "/pak"))
                    throw new IOException("Failed to copy directory into pak within temporary file system!");
            } catch (IOException ex) {
                pakGenerated = false;

                if (MessageBox.Show(ex.Message, "Failed to generate pak file!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    pakGenerated = GeneratePakFile(selectedFolder, pakFilename);
            }

            // Destroy the temporary file system regardless of success or failure
            XLPack.DestroyFileSystem();

            // Delete the temporary directory if it was created
            FileUtil.DeleteDirectory(tempCreatedDir);

            return pakGenerated;
        }

        public static bool InstallPakFile(string gamePakPath, string pakPath) {
            bool pakInstalled = true;

            try {
                if(FileUtil.IsFileLocked(gamePakPath))
                    throw new IOException("Game pak already in use by another program!\nClose the ArcheAge client and launcher to install addons.");

                // Create the temporary file system
                if (!XLPack.CreateFileSystem())
                    throw new IOException("Failed to create temporary file system!");

                // Mount the game pak into the /src directory of the temporary file system
                IntPtr masterMount = XLPack.Mount("/master", gamePakPath, true);

                if (masterMount == IntPtr.Zero)
                    throw new IOException("Failed to mount game pak to temporary file system!");

                if(!XLPack.ApplyPatchPak("/master", pakPath))
                    throw new IOException("Failed to apply patch pak!");

                if(!XLPack.Unmount(masterMount))
                    throw new IOException("Failed to unmount game pak from temporary file system!");
            } catch (IOException ex) {
                pakInstalled = false;

                if (MessageBox.Show(ex.Message, "Failed to install addon!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    pakInstalled = InstallPakFile(gamePakPath, pakPath);
            }

            // Destroy the temporary file system regardless of success or failure
            XLPack.DestroyFileSystem();

            return pakInstalled;
        }

        public static async void InstallSelectedAddons(string installationPath) {
            // Get the list of addon widgets from the main window
            List<AddonDataManager.AddonWidget> addonWidgets = AddonDataManager.instance.addonWidgets;

            int selectedAddons = addonWidgets.Count(widget => widget.checkbox.Checked);

            // Create a list of all the scripts which will be updated
            List<string> scriptsPendingUpdate = new List<string>();

            // Check if any of the files in the addons are duplicates of another selected addon
            for (int i = 0; i < addonWidgets.Count; i++) {
                // If the addon is not selected, skip it
                if (!addonWidgets[i].checkbox.Checked) continue;

                // Add all files modified by this addon to the scripts pending update list
                foreach (AddonFileInfo fileInfo in AddonDataManager.instance.addons[i].fileInfos)
                    scriptsPendingUpdate.Add(fileInfo.filepath);
            }

            // Check for any duplicates in the list of scripts pending update (converting to hash set removes duplicates)
            if (scriptsPendingUpdate.Count != scriptsPendingUpdate.ToHashSet().Count()) {
                MessageBox.Show("Two or more selected addons attempt to modify the same scripts so cannot be installed together.", "Conflicting addons selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int curAddon = 1;

            MainWindow.instance.DisplayLoadingOverlay("Installing addon " + curAddon + " of " + selectedAddons + "..", "");

            for (int i = 0; i < addonWidgets.Count; i++) {
                if (addonWidgets[i].checkbox.Checked) {
                    string addonUrl = "https://www.spacemeat.space/aamods/data/" + AddonDataManager.instance.addons[i].packagedFileName + ".zip";
                    string zipPath = FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName + ".zip";

                    MainWindow.instance.DisplayLoadingOverlay("Installing addon " + curAddon + " of " + selectedAddons + "..", "Downloading " + Path.GetFileName(zipPath));

                    WebRequest webRequest = new WebRequest();
                    await webRequest.DownloadFile(addonUrl, zipPath);

                    MainWindow.instance.DisplayLoadingOverlay("Installing addon " + curAddon + " of " + selectedAddons + "..", "Extracting " + Path.GetFileName(zipPath));

                    // Extract mod.zip from the zip file into FileUtil.TempFilePath() then delete the zip
                    FileUtil.ExtractZipFile(zipPath, FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName);
                    File.Delete(zipPath);

                    MainWindow.instance.DisplayLoadingOverlay("Installing addon " + curAddon + " of " + selectedAddons + "..", "Patching " + Path.GetFileNameWithoutExtension(zipPath) + " to game client");

                    InstallPakFile(installationPath + @"\game_pak", FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName + @"\mod.pak");

                    curAddon++;
                }
            }

            MainWindow.instance.CloseLoadingOverlay();
        }
    }
}
