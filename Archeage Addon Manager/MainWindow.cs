﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : Form {
        public class AddonData {
            public string name { get; set; }
            public string description { get; set; }
            public string author { get; set; }
            public float version { get; set; }
            public string dataPath { get; set; }
        }

        public class AddonWidget {
            public CheckBox checkbox { get; set; }
            public Label titleLabel { get; set; }
            public Label descriptionLabel { get; set; }
        }

        public class FolderInfo {
            public string foldername { get; set; }
            public string folderpath { get; set; }
            public List<FileInfo> files { get; set; } = new List<FileInfo>();
            public List<FolderInfo> subFolders { get; set; } = new List<FolderInfo>();
        }

        public class FileInfo {
            public string filename { get; set; }
            public string filepath { get; set; }
            public long filesize { get; set; }
            public string checksum { get; set; }
        }

        List<AddonData> addons = new List<AddonData>();
        List<AddonWidget> addonWidgets = new List<AddonWidget>();

        public MainWindow() {
            InitializeComponent();

            AddAddonsFromURL("https://www.spacemeat.space/aamods/data.json");

        }

        private void InstallButtonClick(object sender, EventArgs e) {
            for (int i = 0; i < addonWidgets.Count; i++) {
                if (addonWidgets[i].checkbox.Checked) {
                    Console.WriteLine("Checked: " + addonWidgets[i].titleLabel.Text);
                }
            }
        }

        public void OnClickAddonWidget(int addonIndex) {
            addonWidgets[addonIndex].checkbox.Checked = !addonWidgets[addonIndex].checkbox.Checked;

            OnCheckboxesUpdated();
        }

        public void OnCheckboxesUpdated() {
            int totalChecked = 0;

            for (int i = 0; i < addonWidgets.Count; i++) {
                if (addonWidgets[i].checkbox.Checked) {
                    totalChecked++;
                }
            }

            label1.Text = totalChecked + " addon" + (totalChecked > 1 ? "s" : "") + " selected";
        }

        public void AddAddon(AddonData addonData) {
            // Store the addon data in a list for later referencing
            addons.Add(addonData);

            // Visually display the addon in the addon list panel
            // Create a horizontal group to contain each addon
            FlowLayoutPanel horizontalGroup = new FlowLayoutPanel() {
                Dock = DockStyle.Top,
                Width = panel1.Width - SystemInformation.VerticalScrollBarWidth,
                AutoSize = true,
                BackColor = Color.LightGray
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
                Height = 43 + ((addonData.description.Split(new string[] { "\n" }, StringSplitOptions.None).Length - 1) * 15), // Set height based on newlines in description
                Margin = new Padding(0, 0, 0, 0)
            };

            // Title label which contains addon name, version and author
            Label titleLabel = new Label() {
                Width = textGroup.Width,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Text = addonData.name + " - v" + addonData.version + " by " + addonData.author,
                Margin = new Padding(0, 0, 0, 0)
            };

            // Description label displayed below the title label
            Label descriptionLabel = new Label() {
                Width = textGroup.Width,
                AutoSize = true,
                Text = addonData.description,
                Margin = new Padding(0, 0, 0, 0)
            };

            // Add the title and description labels to the right side text group
            textGroup.Controls.Add(titleLabel);
            textGroup.Controls.Add(descriptionLabel);

            // Add the left side checkboxGroup and right side textGroup to the horizontal group
            horizontalGroup.Controls.Add(checkboxGroup);
            horizontalGroup.Controls.Add(textGroup);

            // Create a spacer to separate each addon with a visual spacer
            FlowLayoutPanel spacerPanel = new FlowLayoutPanel(){
                Dock = DockStyle.Top,
                Width = panel1.Width - SystemInformation.VerticalScrollBarWidth,
                Height = 1,
                BackColor = Color.White
            };

            // Controls added to the panel are added bottom to top based on the top dock style
            // Add the spacer to the panel
            panel1.Controls.Add(spacerPanel);

            // Add the horizontal group to the panel
            panel1.Controls.Add(horizontalGroup);

            // Vertically center the checkbox
            int verticalMargin = (checkboxGroup.Height - addonCheckbox.Height) / 2;
            int horizontalMargin = (checkboxGroup.Width - addonCheckbox.Width) / 2;
            addonCheckbox.Margin = new Padding(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);

            int curId = addonWidgets.Count;

            horizontalGroup.Click += (sender, e) => OnClickAddonWidget(curId);
            checkboxGroup.Click += (sender, e) => OnClickAddonWidget(curId);
            addonCheckbox.Click += (sender, e) => OnClickAddonWidget(curId);
            textGroup.Click += (sender, e) => OnClickAddonWidget(curId);
            titleLabel.Click += (sender, e) => OnClickAddonWidget(curId);
            descriptionLabel.Click += (sender, e) => OnClickAddonWidget(curId);

            // Cache the addonWidgets in a list for easier referencing
            addonWidgets.Add(new AddonWidget() {
                checkbox = addonCheckbox,
                titleLabel = titleLabel,
                descriptionLabel = descriptionLabel
            });
        }

        // Load addons from a URL containing a JSON array of AddonData objects
        public void AddAddonsFromURL(string url) {
            // Send a web request to the specified URL which should return a JSON array of AddonData objects
            string jsonInput = new System.Net.WebClient().DownloadString(url);

            try {
                // Deserialize the JSON array into a list of AddonData objects
                List<AddonData> addonList = JsonConvert.DeserializeObject<List<AddonData>>(jsonInput);

                // Iterate through each addon and call AddAddon
                foreach (AddonData addon in addonList) {
                    AddAddon(addon);
                }
            } catch (Exception e) {
                Console.WriteLine("Error parsing JSON result from URL " + url + " - " + e.Message);
            }
        }

        public void ShowAboutDialog(object sender, EventArgs e) {
            MessageBox.Show("Not endorsed by XLGames or Kakao Games, we don't reflect views or opinion of anyone officially involved in Archeage.\n\nKakao strongly does not recommend addon usage, you accept the potential game breaking risks!\n\nArcheage Addon Manager created by Nidoran\n\nBig thanks to Ingram for all his work and help to make this possible and his work on AAPatcher.\n\nAdditional thanks to Tamaki & Strawberry", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private FileInfo GetFileInfo(string filePath) {
            return new FileInfo {
                filename = Path.GetFileName(filePath),
                filepath = filePath,
                filesize = GetFileSize(filePath),
                checksum = CalculateChecksum(filePath)
            };
        }

        private FolderInfo GetFolderInfo(string folderPath) {
            var folderInfo = new FolderInfo {
                foldername = Path.GetFileName(folderPath),
                folderpath = folderPath,
                files = new List<FileInfo>()
            };

            foreach (var filePath in Directory.GetFiles(folderPath)) {
                var fileInfo = GetFileInfo(filePath);
                folderInfo.files.Add(fileInfo);
            }

            foreach (var subFolderPath in Directory.GetDirectories(folderPath)) {
                var subFolderInfo = GetFolderInfo(subFolderPath);
                folderInfo.subFolders.Add(subFolderInfo);
            }

            return folderInfo;
        }

        private string CreateJsonForFolder(string folderPath) {
            var folderInfo = GetFolderInfo(folderPath);
            return JsonConvert.SerializeObject(folderInfo, Formatting.Indented);
        }

        private void UploadAddonButtonClick(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select root directory of addon source";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                string jsonOutput = CreateJsonForFolder(selectedFolder);

                MessageBox.Show(jsonOutput);
            }
        }

    }
}
