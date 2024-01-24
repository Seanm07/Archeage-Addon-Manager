using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Archeage_Addon_Manager {
    public class AddonDataManager {
        public static AddonDataManager instance;

        public class AddonData {
            public string name { get; set; }
            public string description { get; set; }
            public string author { get; set; }
            public float version { get; set; }
            public string dataPath { get; set; }
            public List<FileInfo> fileInfos { get; set; }

            public AddonData() {
                fileInfos = new List<FileInfo>();
            }
        }

        public class FileInfo {
            public string filename { get; set; }
            public string filepath { get; set; }
            public long filesize { get; set; }
            public string checksum { get; set; }
        }

        List<AddonData> addons = new List<AddonData>();

        public AddonDataManager() {
            instance ??= this;
        }

        public void LoadAddonsFromDataSources() {
            // TODO: Allow the user to add custom addon sources which is saved to a config file/registry or something
            AddAddonsFromURL("https://www.spacemeat.space/aamods/data.json");
        }

        // Load addons from a URL containing a JSON array of AddonData objects
        public void AddAddonsFromURL(string url) {
            try {
                // Send a web request to the specified URL which should return a JSON array of AddonData objects
                string jsonInput = new System.Net.WebClient().DownloadString(url);

                // Deserialize the JSON array into a list of AddonData objects
                List<AddonData> addonList = JsonConvert.DeserializeObject<List<AddonData>>(jsonInput);

                // Iterate through each addon and call AddAddon
                foreach (AddonData addon in addonList) {
                    // Store the addon data in a list for later referencing
                    addons.Add(addon);

                    // Create a widget in the main window for this addon
                    MainWindow.instance.AddAddonWidget(addon);
                }
            } catch (Exception e) {
                DialogResult selectedOption = MessageBox.Show(url + " failed to load! " + e.Message, "Failed to load addon source!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);

                switch (selectedOption) {
                    case DialogResult.Abort: Environment.Exit(0); break;
                    case DialogResult.Retry: AddAddonsFromURL(url); break;
                }
            }
        }

        private string CalculateChecksum(string filePath) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(filePath)) {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }

        private long GetFileSize(string filePath) {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                return stream.Length;
            }
        }

        private FileInfo GetFileInfo(string filePath, string basePath) {
            return new FileInfo {
                filename = Path.GetFileName(filePath),
                filepath = RelativeFormatPath(filePath, basePath),
                filesize = GetFileSize(filePath),
                checksum = CalculateChecksum(filePath)
            };
        }

        public void GetAddonSrcInfo(string basePath, out AddonData addonInfo, out List<string> filePaths) {
            GetAddonSrcInfo(basePath, basePath, out addonInfo, out filePaths);
        }

        private void GetAddonSrcInfo(string folderPath, string basePath, out AddonData addonInfo, out List<string> filePaths) {
            addonInfo = new AddonData();
            filePaths = new List<string>();

            // Add all files in this folder to the list of files
            foreach (var filePath in Directory.GetFiles(folderPath)) {
                var fileInfo = GetFileInfo(filePath, basePath);
                addonInfo.fileInfos.Add(fileInfo);
                filePaths.Add(fileInfo.filepath);
            }

            // Add all subfolders in this folder to the list of files
            foreach (var subFolderPath in Directory.GetDirectories(folderPath)) {
                GetAddonSrcInfo(subFolderPath, basePath, out AddonData subFolderFileInfos, out List<string> subFolderFilePaths);
                addonInfo.fileInfos.AddRange(subFolderFileInfos.fileInfos);
                filePaths.AddRange(subFolderFilePaths);
            }
        }

        private string RelativeFormatPath(string path, string basePath) {
            return path.Replace(basePath, "").Replace("\\", "/");
        }

        public string CreateJsonForFolder(AddonData addonInfo) {
            var jsonSettings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(addonInfo, jsonSettings);
        }

        // Search for ArcheAge installation paths on all drives
        public string[] FindInstallationPaths() {
            List<string> validPaths = new List<string>();

            // Check for ArcheAge installation paths on all drives
            foreach (DriveInfo drive in DriveInfo.GetDrives()) {
                // Add the common ArcheAge installation paths to the directories to be scanned
                string[] possiblePaths = {
                    Path.Combine(drive.Name, "KakaoGames", "ArcheAge"),
                    Path.Combine(drive.Name, "Steam", "steamapps", "common", "ArcheAge"),
                    Path.Combine(drive.Name, "SteamLibrary", "steamapps", "common", "ArcheAge"),
                    Path.Combine(drive.Name, "Program Files (x86)", "steamapps", "common", "ArcheAge")
                };

                // Under certain installs the ArcheAge folder gets duplicated and appended with a number, scan for these too
                for (int i = 0; i < 10; i++) {
                    possiblePaths = possiblePaths.Concat(new string[] {
                        possiblePaths[0] + i,
                        possiblePaths[1] + i,
                        possiblePaths[2] + i,
                        possiblePaths[3] + i,
                    }).ToArray();
                }

                // Add all directories which exist to the list of valid paths
                validPaths.AddRange(possiblePaths.Where(path => Directory.Exists(path)));
            }

            // Add an option to browse for a custom installation path at the end of the list
            validPaths.Add("Browse..");

            return validPaths.ToArray();
        }

        public void UploadAddonToServer(string addonZipPath) {
            // TODO: This is temporary! Just using ftp for now, this will change to a POST request later
            string ftpHost = "ftp://spacemeat.space";
            string username = Microsoft.VisualBasic.Interaction.InputBox("Enter FTP Username", "FTP Details", "");
            string password = Microsoft.VisualBasic.Interaction.InputBox("Enter FTP Password", "FTP Details", "");

            try {
                using (WebClient client = new WebClient()) {
                    client.Credentials = new NetworkCredential(username, password);
                    client.UploadFile(ftpHost + "/spacemeat.space/www/aamods/data/" + Path.GetFileName(addonZipPath), WebRequestMethods.Ftp.UploadFile, addonZipPath);

                    MessageBox.Show("Addon uploaded successfully!");
                }
            } catch (Exception ex) {
                MessageBox.Show("Error uploading file to FTP server: " + ex.Message);
            }
        }
    }
}