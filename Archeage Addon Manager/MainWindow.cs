using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : Form {
        public class AddonWidget
        {
            public CheckBox checkbox { get; set; }
            public Label titleLabel { get; set; }
            public Label descriptionLabel { get; set; }
        }

        public static MainWindow instance;

        List<AddonWidget> addonWidgets = new List<AddonWidget>();

        public MainWindow() {
            instance ??= this;

            InitializeComponent();

            // Load addons from data sources
            AddonDataManager.instance.LoadAddonsFromDataSources();

            // Find archeage installation directories
            installationPathComboBox.Items.AddRange(AddonDataManager.instance.FindInstallationPaths());
            installationPathComboBox.SelectedIndex = 0;
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

        // Visually display a widget for an addon in the addon list panel
        public void AddAddonWidget(AddonDataManager.AddonData addonData) {
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

            // Register click events on all controls so clicking anything checks/unchecks the checkbox
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

        public void ShowAboutDialog(object sender, EventArgs e) {
            MessageBox.Show("Not endorsed by XLGames or Kakao Games, we don't reflect views or opinion of anyone officially involved in Archeage.\n\nKakao strongly does not recommend addon usage, you accept the potential game breaking risks!\n\nArcheage Addon Manager created by Nidoran\n\nBig thanks to Ingram for all his work and help to make this possible and his work on AAPatcher.\n\nAdditional thanks to Tamaki & Strawberry", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UploadAddonButtonClick(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog(){
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

                string jsonOutput = AddonDataManager.instance.CreateJsonForFolder(addonInfo);

                // Create a text file named addon.json at FileUtil.TempFilePath() + "addon.json" and write the jsonOutput to it
                string jsonPath = FileUtil.TempFilePath() + "addon.json";
                File.WriteAllText(jsonPath, jsonOutput);

                MessageBox.Show("These scripts were found in your addon!\nThey'll be extracted from your game_pak as a backup.\n\n" + string.Join("\n", filePaths));

                string addonFileName = addonInfo.name.Replace(" ", "_").ToLower();

                try {
                    if (!PakManager.GeneratePakFile(selectedFolder, "mod"))
                        throw new IOException("Failed to generate addon pak file!");

                    // TODO: Make this asyncronous
                    if (!PakManager.GenerateUninstallPakFile(installationPathComboBox.Text + @"\game_pak", filePaths.ToArray(), "default"))
                        throw new IOException("Failed to generate uninstall pak file!");

                    // TODO: Make a function to load these names so we don't have different scripts directly referencing them by string
                    string[] addonFiles = new string[3] {
                        FileUtil.TempFilePath() + "addon.json",
                        FileUtil.TempFilePath() + "mod.pak",
                        FileUtil.TempFilePath() + "default.pak"
                    };

                    // Move the addon files into a zip
                    if (!FileUtil.CreateZipFile(addonFiles, FileUtil.TempFilePath() + addonFileName + ".zip"))
                        throw new IOException("Failed to build addon zip file!");
                } catch (IOException ex) {
                    if (MessageBox.Show(ex.Message, "Failed to package addon!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        UploadAddonButtonClick(sender, e);
                    return;
                }

                AddonDataManager.instance.UploadAddonToServer(FileUtil.TempFilePath() + addonFileName + ".zip");

                // Cleanup the json file
                File.Delete(jsonPath);

                // Cleanup the pak files
                File.Delete(FileUtil.TempFilePath() + "mod.pak");
                File.Delete(FileUtil.TempFilePath() + "default.pak");

                // Cleanup the zip file
                File.Delete(FileUtil.TempFilePath() + addonFileName + ".zip");
            }
        }
    }
}
