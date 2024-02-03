using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public class AddonDataManager {
        public static AddonDataManager instance;

        public class AddonData {
            public string name { get; set; }
            public string description { get; set; }
            public string author { get; set; }
            public float version { get; set; }
            public string packagedFileName { get; set; }
            public List<AddonFileInfo> fileInfos { get; set; }

            public AddonData() {
                fileInfos = new List<AddonFileInfo>();
            }
        }

        public List<AddonData> addons = new List<AddonData>();

        public AddonDataManager() {
            instance ??= this;
        }

        public void ReloadAddonsFromDataSources() {
            MainWindow.instance.ClearAddonWidgets();
            addons.Clear();

            LoadAddonsFromDataSources();
        }

        public void LoadAddonsFromDataSources() {
            // TODO: Allow the user to add custom addon sources which is saved to a config file/registry or something
            AddAddonsFromURL("https://www.spacemeat.space/aamods/data/list.php");
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

        public void GetAddonSrcInfo(string basePath, out AddonData addonInfo, out List<string> filePaths) {
            GetAddonSrcInfo(basePath, basePath, out addonInfo, out filePaths);
        }

        private void GetAddonSrcInfo(string folderPath, string basePath, out AddonData addonInfo, out List<string> filePaths) {
            addonInfo = new AddonData();
            filePaths = new List<string>();

            // Add all files in this folder to the list of files
            foreach (var filePath in Directory.GetFiles(folderPath)) {
                var fileInfo = FileUtil.GetFileInfo(filePath, basePath);
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
            // Use Task.Run to call the async method from a non-async context
            Task.Run(() => UploadAddonToServerAsync(addonZipPath)).Wait();
        }

        // TODO: Function cleanup
        public async Task UploadAddonToServerAsync(string addonZipPath) {
            MessageBox.Show("Uploading addon to server...");

            try {
                using (HttpClient client = new HttpClient()) {
                    string phpScriptUrl = "https://www.spacemeat.space/aamods/data/upload.php";

                    // Create multipart form content
                    using (MultipartFormDataContent formData = new MultipartFormDataContent()) {
                        // Add the ZIP file to the form data
                        byte[] fileBytes = File.ReadAllBytes(addonZipPath);
                        formData.Add(new ByteArrayContent(fileBytes), "zip_file", Path.GetFileName(addonZipPath));

                        // Make the POST request
                        HttpResponseMessage response = await client.PostAsync(phpScriptUrl, formData);

                        // Check the response status
                        if (response.IsSuccessStatusCode) {
                            string responseText = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(responseText);
                        } else {
                            MessageBox.Show("Error uploading file: " + response.ReasonPhrase);
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error uploading file: " + ex.Message);
            }
        }
    }
}