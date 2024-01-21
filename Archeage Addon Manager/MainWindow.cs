using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : Form {
        public static MainWindow instance;

        public class AddonWidget
        {
            public CheckBox checkbox { get; set; }
            public Label titleLabel { get; set; }
            public Label descriptionLabel { get; set; }
        }

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
                string jsonOutput = AddonDataManager.instance.CreateJsonForFolder(selectedFolder);

                MessageBox.Show(jsonOutput);

                if (PakManager.GeneratePakFile(selectedFolder)) {
                    // Upload the generated pak file to the specified URL
                    //string uploadUrl = "https://www.spacemeat.space/aamods/upload.php";
                    //string response = AddonDataManager.instance.UploadFile(uploadUrl, pakPath);

                    MessageBox.Show("TODO upload the pak to the server here");
                }

                PakManager.ExtractFilesFromGamePak(installationPathComboBox.Text + @"\game_pak", new string[] { "/game/scriptsbin/auction/auction.alb", "/game/scriptsbin/auction/auction_view.alb" });

                // TODO: Delete the pakPath file once done
            }
        }
    }
}
