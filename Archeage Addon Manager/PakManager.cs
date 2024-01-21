using System;
using System.IO;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public class PakManager {
        public static bool GeneratePakFile(string selectedFolder) {
            bool pakGenerated = true;

            // The generated pak will be created in the temp directory
            string pakPath = Path.GetTempPath() + "mod.pak";
            string tempCreatedDir = Path.GetTempPath() + "modsrc";

            // Delete the temporary directory if it already exists from a previous mod
            if (Directory.Exists(tempCreatedDir))
                Directory.Delete(tempCreatedDir, true);

            // Copy the addon source directory to the temporary directory
            FileUtil.CopyDirectory(selectedFolder, tempCreatedDir);

            // Rename all .lua files to .alb recursively within the temporary directory
            string[] luaFiles = Directory.GetFiles(tempCreatedDir, "*.lua", SearchOption.AllDirectories);
            foreach (string luaFile in luaFiles)
                File.Move(luaFile, Path.ChangeExtension(luaFile, ".alb"));

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
                    GeneratePakFile(selectedFolder);
            }

            // Destroy the temporary file system regardless of success or failure
            XLPack.DestroyFileSystem();

            // Delete the temporary directory if it was created
            if (Directory.Exists(tempCreatedDir))
                Directory.Delete(tempCreatedDir, true);

            return pakGenerated;
        }
    }
}
