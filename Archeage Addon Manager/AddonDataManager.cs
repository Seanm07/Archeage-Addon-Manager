using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public class AddonWidget {
            public CheckBox checkbox { get; set; }
            public Label titleLabel { get; set; }
            public Label descriptionLabel { get; set; }
        }

        public List<AddonWidget> addonWidgets = new List<AddonWidget>();

        public Panel addonWidgetContainerPanel;

        public AddonDataManager(Panel addonContainerPanel) {
            instance ??= this;

            addonWidgetContainerPanel = addonContainerPanel;
        }

        public void ToggleAddonWidgetChecked(int addonIndex) {
            addonWidgets[addonIndex].checkbox.Checked = !addonWidgets[addonIndex].checkbox.Checked;
        }

        public int GetCheckedAddonWidgetCount() { 
            return addonWidgets.Count(widget => widget.checkbox.Checked); 
        }

        // Visually display a widget for an addon in the addon list panel
        public void AddAddonWidget(AddonData addonData) {
            string displayedAddonName = addonData.name.Length > 35 ? addonData.name.Substring(0, 35) + ".." : addonData.name;
            string displayedAddonDescription = addonData.description.Length > 150 ? addonData.description.Substring(0, 150) + ".." : addonData.description;
            string displayedAddonVersion = addonData.version.ToString("N2");

            // Create a horizontal group to contain each addon
            FlowLayoutPanel horizontalGroup = new FlowLayoutPanel() {
                Width = addonWidgetContainerPanel.Width - SystemInformation.VerticalScrollBarWidth,
                Height = 90,
                BackColor = Color.FromArgb(33, 35, 38),
                Margin = new Padding(0, 0, 0, 0),
                Location = new Point(0, addonWidgetContainerPanel.Controls.Count * 85)
            };

            // Create a checkbox to select the addon for installation or to show already installed
            CheckBox addonCheckbox = new CheckBox() {
                AutoCheck = false,
                Padding = new Padding(5, 5, 0, 0),
                BackColor = Color.Transparent
            };

            // Image preview
            PictureBox addonPreview = new PictureBox() {
                Width = horizontalGroup.Height - 10,
                Height = horizontalGroup.Height - 10,
                Margin = new Padding(5, 5, 0, 0),
                //BackColor = Color.Red,
                //Image = addonData.previewImage,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // TODO: Replace with actual addon thumbnail URL
            AsyncLoadImageFromURL(addonPreview, "https://picsum.photos/" + addonPreview.Height);

            // Add the checkbox to the checkboxGroup group
            addonPreview.Controls.Add(addonCheckbox);

            // Group to contain right side text labels and button
            Panel textGroup = new Panel() {
                Width = horizontalGroup.Width - addonPreview.Width - 10,
                Height = horizontalGroup.Height - 10,
                Margin = new Padding(5, 5, 0, 0)
            };

            // Title label which contains addon name, version and author
            Label titleLabel = new Label() {
                Width = textGroup.Width,
                Height = 20,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Text = displayedAddonName,
                Margin = new Padding(5, 5, 0, 0),
                ForeColor = Color.FromArgb(33, 35, 38),
                BackColor = Color.White
            };

            // Version label
            Label versionLabel = new Label() {
                Width = 100,
                Height = 12,
                Font = new Font("Microsoft Sans Serif", 6, FontStyle.Regular),
                Text = "Version: " + displayedAddonVersion,
                Margin = new Padding(0, 0, 0, 0),
                ForeColor = Color.LightGray,
                //BackColor = Color.FromArgb(80, 80, 80),
                Location = new Point(0, textGroup.Height - 12),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Author label
            Label authorLabel = new Label() {
                Width = textGroup.Width - versionLabel.Width,
                Height = versionLabel.Height,
                Font = new Font("Microsoft Sans Serif", 6, FontStyle.Regular),
                Text = "Author: " + addonData.author,
                Margin = new Padding(0, 0, 0, 0),
                ForeColor = Color.LightGray,
                //BackColor = versionLabel.BackColor,
                Location = new Point(versionLabel.Width, versionLabel.Location.Y),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Description label displayed below the title label
            Label descriptionLabel = new Label() {
                Width = textGroup.Width,
                Height = textGroup.Height - titleLabel.Height - versionLabel.Height,
                Text = displayedAddonDescription,
                Margin = new Padding(0, 0, 0, 0),
                ForeColor = Color.White,
                Location = new Point(0, titleLabel.Height + 2)
            };

            // Add controls to the textGroup
            textGroup.Controls.Add(titleLabel);
            textGroup.Controls.Add(versionLabel);
            textGroup.Controls.Add(authorLabel);
            textGroup.Controls.Add(descriptionLabel);

            // Add the left side checkboxGroup, image preview, and right side textGroup to the horizontal group
            horizontalGroup.Controls.Add(addonPreview);
            horizontalGroup.Controls.Add(textGroup);

            // Add the horizontal group to the panel
            addonWidgetContainerPanel.Controls.Add(horizontalGroup);

            int curId = addonWidgets.Count;

            // Register click events on all controls so clicking anything checks/unchecks the checkbox
            horizontalGroup.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
            addonCheckbox.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
            textGroup.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
            titleLabel.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
            descriptionLabel.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);

            // Cache the addonWidgets in a list for easier referencing
            addonWidgets.Add(new AddonWidget() {
                checkbox = addonCheckbox,
                titleLabel = titleLabel,
                descriptionLabel = descriptionLabel
            });
        }

        public async void AsyncLoadImageFromURL(PictureBox pictureBox, string imageUrl) {
            await new WebRequest().DownloadImageFromUrl(imageUrl, (image) => {
                if (image != null) {
                    // Image downloaded successfully, assign it to the PictureBox
                    pictureBox.Image = image;
                } else {
                    // Handle case where image download failed
                    Console.WriteLine("Image download failed.");
                }
            });
        }

        public void ReloadAddonsFromDataSources() {
            MainWindow.instance.ClearAddonWidgets();
            addons.Clear();

            LoadAddonsFromDataSources();
        }

        public string[] GetAddonSourcesList() {
            string addonSources = ProgramManager.ReadFromConfigFile("addon_sources", "https://www.spacemeat.space/aamods/api/list.php");

            // Split the comma separated list of addon sources into an array and remove any empty strings
            return addonSources.Split(',').Where(source => !string.IsNullOrWhiteSpace(source)).ToArray();
        }

        public void SaveAddonSourcesList(string addonSources) {
            // Remove any whitespace and split the comma separated list of addon sources into an array
            string formattedAddonSources = string.Join(",", addonSources.Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries));

            ProgramManager.WriteToConfigFile("addon_sources", formattedAddonSources);

            ReloadAddonsFromDataSources();
        }

        public void LoadAddonsFromDataSources() {
            string[] addonSourcesArray = GetAddonSourcesList();

            // If the list is empty, prompt a message to add the default source
            if (addonSourcesArray.Length <= 0) {
                MainWindow.instance.ShowMessagePopup("No addon sources found!", "No addon sources found in the config file!\nWould you like to revert back to the default source?", "Yes",
                    () => {
                        SaveAddonSourcesList("https://www.spacemeat.space/aamods/api/list.php");
                        ReloadAddonsFromDataSources();
                    }, "No");
            } else {
                foreach (string source in addonSourcesArray)
                    AddAddonsFromURL(source);
            }
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
                    AddAddonWidget(addon);
                }
            } catch (Exception e) {
                MainWindow.instance.ShowMessagePopup("Failed to load addon source!", url + " failed to load!\n\n" + e.Message, "Retry", () => AddAddonsFromURL(url), "Ignore");
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

        public int GetActiveInstallationPathIndex() {
            // Get active installation path from config file
            return int.Parse(ProgramManager.ReadFromConfigFile("installation_path_index", "0"));
        }

        public void SetActiveInstallationPathIndex(int index, string customPath = "") {
            ProgramManager.WriteToConfigFile("installation_path_index", index.ToString());

            MainWindow.instance.UpdateBackupList();
            MainWindow.instance.UpdatePatchInfoLabels();
        }

        public string GetActiveInstallationPath() {
            return FindInstallationPaths()[GetActiveInstallationPathIndex()];
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
    }
}