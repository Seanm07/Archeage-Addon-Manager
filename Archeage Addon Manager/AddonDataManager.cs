using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            string displayedAddonName = addonData.name.Length > 20 ? addonData.name.Substring(0, 20) + ".." : addonData.name;
            string displayedAddonDescription = addonData.description.Length > 100 ? addonData.description.Substring(0, 100) + ".." : addonData.description;
            string displayedAddonVersion = addonData.version.ToString("N2");

            // Insert a \n at the last space before the 50th character to prevent cutting off words
            if(displayedAddonDescription.Length > 50) {
                int lastSpaceIndex = displayedAddonDescription.LastIndexOf(' ', 50);
                if (lastSpaceIndex != -1)
                    displayedAddonDescription = displayedAddonDescription.Substring(0, lastSpaceIndex) + "\n" + displayedAddonDescription.Substring(lastSpaceIndex + 1);
            }

            // Create a horizontal group to contain each addon
            FlowLayoutPanel horizontalGroup = new FlowLayoutPanel() {
                Dock = DockStyle.Top,
                Width = addonWidgetContainerPanel.Width - SystemInformation.VerticalScrollBarWidth,
                AutoSize = true,
                BackColor = Color.FromArgb(33, 35, 38),
            };

            // Container for the checkbox so we can center it
            FlowLayoutPanel checkboxGroup = new FlowLayoutPanel() {
                Dock = DockStyle.Left,
                Width = 30,
                Margin = new Padding(0, 0, 0, 0)
            };

            // Create a checkbox to select the addon for installation or to show already installed
            CheckBox addonCheckbox = new CheckBox() {
                Width = 15,
                AutoCheck = false
            };

            // Add the checkbox to the checkboxGroup group
            checkboxGroup.Controls.Add(addonCheckbox);

            // Group to contain right side text labels
            FlowLayoutPanel textGroup = new FlowLayoutPanel() {
                Width = horizontalGroup.Width - checkboxGroup.Width,
                Height = 50 + ((displayedAddonDescription.Split(new string[] { "\n" }, StringSplitOptions.None).Length - 1) * 15), // Set height based on newlines in description
                Margin = new Padding(0, 0, 0, 0)
            };

            // Title label which contains addon name, version and author
            Label titleLabel = new Label() {
                Width = textGroup.Width,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Text = displayedAddonName + " - v" + displayedAddonVersion + " by " + addonData.author,
                Margin = new Padding(0, 5, 0, 0),
                ForeColor = Color.White
            };

            // Description label displayed below the title label
            Label descriptionLabel = new Label() {
                Width = textGroup.Width,
                AutoSize = true,
                Text = displayedAddonDescription,
                Margin = new Padding(0, 0, 0, 0),
                ForeColor = Color.White
            };

            // Add the title and description labels to the right side text group
            textGroup.Controls.Add(titleLabel);
            textGroup.Controls.Add(descriptionLabel);

            // Add the left side checkboxGroup and right side textGroup to the horizontal group
            horizontalGroup.Controls.Add(checkboxGroup);
            horizontalGroup.Controls.Add(textGroup);

            // Create a spacer to separate each addon with a visual spacer
            FlowLayoutPanel spacerPanel = new FlowLayoutPanel() {
                Dock = DockStyle.Top,
                Width = addonWidgetContainerPanel.Width - SystemInformation.VerticalScrollBarWidth,
                Height = 1,
                BackColor = Color.FromArgb(21, 23, 26)
            };

            // Controls added to the panel are added bottom to top based on the top dock style
            // Add the spacer to the panel
            addonWidgetContainerPanel.Controls.Add(spacerPanel);

            // Add the horizontal group to the panel
            addonWidgetContainerPanel.Controls.Add(horizontalGroup);

            // Vertically center the checkbox
            int verticalMargin = (checkboxGroup.Height - addonCheckbox.Height) / 2;
            int horizontalMargin = (checkboxGroup.Width - addonCheckbox.Width) / 2;
            addonCheckbox.Margin = new Padding(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);

            int curId = addonWidgets.Count;

            // Register click events on all controls so clicking anything checks/unchecks the checkbox
            horizontalGroup.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
            checkboxGroup.Click += (sender, e) => MainWindow.instance.OnClickAddonWidget(curId);
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

        public void ReloadAddonsFromDataSources() {
            MainWindow.instance.ClearAddonWidgets();
            addons.Clear();

            LoadAddonsFromDataSources();
        }

        public string[] GetAddonSourcesList() {
            string addonSources = ProgramManager.ReadFromConfigFile("addon_sources", "https://www.spacemeat.space/aamods/api/list.php");

            // Split the comma separated list of addon sources into an array
            return addonSources.Split(',');
        }

        public void LoadAddonsFromDataSources() {
            foreach (string source in GetAddonSourcesList())
                AddAddonsFromURL(source);
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
            return 0; // TODO: Load this from a config file
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