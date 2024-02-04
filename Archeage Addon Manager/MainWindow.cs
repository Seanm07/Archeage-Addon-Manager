using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : Form {
        // TODO: Probably move this to a separate script
        public class ToolstripRenderer : ToolStripProfessionalRenderer {

            // Override the system default menu item background rendering
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
                ToolStripItem item = e.Item;
                Graphics g = e.Graphics;

                Rectangle bounds = new Rectangle(Point.Empty, new Size(item.Width - 3, item.Height));
                bounds.X += 2;

                if (item.IsOnDropDown) {
                    // This is an item inside the dropdown
                    // If the current item is being hovered/selected via keyboard
                    if (item.Selected) {
                        if (item.Enabled)
                            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 33, 35, 38)), bounds);
                    } else {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(200, 33, 35, 38)), bounds);
                    }
                } else {
                    // This is the top level menu item container
                    // If the current item is being hovered/selected via keyboard
                    if (item.Selected || item.Pressed) {
                        if (item.Enabled)
                            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), bounds);
                    }
                }


            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
                e.TextColor = Color.White;

                base.OnRenderItemText(e);
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
                ToolStripItem item = e.Item;
                Graphics g = e.Graphics;

                if (item.Image != null)
                    g.DrawImage(item.Image, new Rectangle(13, 4, 13, 13));
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder(e);
            }
        }

        public class AddonWidget {
            public CheckBox checkbox { get; set; }
            public Label titleLabel { get; set; }
            public Label descriptionLabel { get; set; }
        }

        public static MainWindow instance;

        List<AddonWidget> addonWidgets = new List<AddonWidget>();

        public MainWindow() {
            instance ??= this;

            InitializeComponent();

            // Use our custom renderer for the menu strip
            menuStrip1.Renderer = new ToolstripRenderer();

            UpdateDeveloperButtonState();

            // Load addons from data sources
            AddonDataManager.instance.LoadAddonsFromDataSources();

            // Find archeage installation directories
            installationPathComboBox.Items.AddRange(AddonDataManager.instance.FindInstallationPaths());
            installationPathComboBox.SelectedIndex = 0;
        }

        private void InstallButtonClick(object sender, EventArgs e) {
            // List of file paths
            List<string> scriptsWhichWillBeUpdated = new List<string>();

            // Check if any of the files in the addons are duplicates of another selected addon
            for (int i = 0; i < addonWidgets.Count; i++) {
                if (addonWidgets[i].checkbox.Checked) {
                    foreach (AddonFileInfo fileInfo in AddonDataManager.instance.addons[i].fileInfos) {
                        scriptsWhichWillBeUpdated.Add(fileInfo.filepath);
                    }
                }
            }

            // Check for any duplicates in the scriptsWhichWillBeUpdated list, throw an error if there are any
            // Converting it to a hasset will remove duplicates then we can check if the count changed
            if (scriptsWhichWillBeUpdated.Count != scriptsWhichWillBeUpdated.ToHashSet().Count()) {
                MessageBox.Show("Two or more selected addons attempt to modify the same scripts so cannot be installed together.", "Conflicting addons selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < addonWidgets.Count; i++) {
                if (addonWidgets[i].checkbox.Checked) {
                    // Download file from https://www.spacemeat.space/aamods/data/addon.zip into FileUtil.TempFilePath()
                    string zipPath = FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName + ".zip";
                    new System.Net.WebClient().DownloadFile("https://www.spacemeat.space/aamods/data/" + AddonDataManager.instance.addons[i].packagedFileName + ".zip", zipPath);

                    // Extract mod.zip from the zip file into FileUtil.TempFilePath() then delete the zip
                    FileUtil.ExtractZipFile(zipPath, FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName);
                    File.Delete(zipPath);

                    MessageBox.Show("Beginning " + addonWidgets[i].titleLabel.Text + " installation!", "Press ok to begin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PakManager.InstallPakFile(installationPathComboBox.Text + @"\game_pak", FileUtil.TempFilePath() + AddonDataManager.instance.addons[i].packagedFileName + @"\mod.pak");
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

        public void ClearAddonWidgets() {
            panel1.Controls.Clear();
            addonWidgets.Clear();
        }

        // Visually display a widget for an addon in the addon list panel
        public void AddAddonWidget(AddonDataManager.AddonData addonData) {
            // Create a horizontal group to contain each addon
            FlowLayoutPanel horizontalGroup = new FlowLayoutPanel() {
                Dock = DockStyle.Top,
                Width = panel1.Width - SystemInformation.VerticalScrollBarWidth,
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
                Height = 50 + ((addonData.description.Split(new string[] { "\n" }, StringSplitOptions.None).Length - 1) * 15), // Set height based on newlines in description
                Margin = new Padding(0, 0, 0, 0)
            };

            // Title label which contains addon name, version and author
            Label titleLabel = new Label() {
                Width = textGroup.Width,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Text = addonData.name + " - v" + addonData.version + " by " + addonData.author,
                Margin = new Padding(0, 5, 0, 0),
                ForeColor = Color.White
            };

            // Description label displayed below the title label
            Label descriptionLabel = new Label() {
                Width = textGroup.Width,
                AutoSize = true,
                Text = addonData.description,
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
                Width = panel1.Width - SystemInformation.VerticalScrollBarWidth,
                Height = 1,
                BackColor = Color.FromArgb(21, 23, 26)
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

        private void DeveloperActionButtonClick(object sender, EventArgs e) {
            if(DeveloperManager.instance.isLoggedIn) {
                if (DeveloperManager.instance.isDeveloper) {
                    DeveloperManager.instance.UploadAddonButtonClick(installationPathComboBox.Text);
                } else {
                    MessageBox.Show("Sorry but you don't have permission to upload addons!\nDM Nidoran on discord if you're interested in getting upload access.", "Developer access required!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                DeveloperManager.instance.LoginButtonClick();
            }
        }

        private Panel loginOverlayPanel;
        private TextBox loginOverlayTextbox;
        private Button loginOverlayButton;

        public void DisplayLoginOverlay() {
            loginOverlayPanel = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(200, 33, 35, 38),
            };

            loginOverlayTextbox = new TextBox() {
                Width = 200,
                Height = 20,
                Location = new Point((this.Width / 2) - 100, (this.Height / 2) - 10),
                Text = "Enter code after authorising discord"
            };

            loginOverlayButton = new Button() {
                Width = 200,
                Height = 20,
                Location = new Point((this.Width / 2) - 100, (this.Height / 2) + 10),
                Text = "Link to Discord",
                BackColor = Color.FromArgb(255, 255, 255),
            };

            Controls.Add(loginOverlayPanel);
            loginOverlayPanel.Controls.Add(loginOverlayTextbox);
            loginOverlayPanel.Controls.Add(loginOverlayButton);

            loginOverlayPanel.BringToFront();

            loginOverlayButton.Click += (sender, e) => {
                DeveloperManager.instance.LoginLinkButtonConfirm(loginOverlayTextbox.Text);
            };
        }

        public void CloseLoginOverlay() {
            Controls.Remove(loginOverlayPanel);
        }

        private void UpdateDeveloperButtonState() {
            if (DeveloperManager.instance.isLoggedIn) {
                if (DeveloperManager.instance.isDeveloper) {
                    developerItemToolStripMenuItem.Text = "Upload Addon";
                    developerItemToolStripMenuItem.Image = Image.FromFile("Resources/cloud_upload.png");
                    developerItemToolStripMenuItem.Enabled = true;
                } else {
                    developerItemToolStripMenuItem.Text = "No Access!";
                    developerItemToolStripMenuItem.Image = Image.FromFile("Resources/lock.png");
                    developerItemToolStripMenuItem.Enabled = false;
                }
            } else {
                developerItemToolStripMenuItem.Text = "Developer Login";
                developerItemToolStripMenuItem.Image = Image.FromFile("Resources/lock.png");
                developerItemToolStripMenuItem.Enabled = true;
            }
        }

        private bool isDragging = false;
        private bool dragReady = false;
        private Point lastMousePosition;

        private void windowDragPanel_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = true;
                dragReady = false;
                lastMousePosition = e.Location;
            }
        }

        private void windowDragPanel_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = false;
            }
        }

        private void windowDragPanel_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                Point currentMousePosition = Control.MousePosition;
                if (dragReady) {
                    int deltaX = currentMousePosition.X - lastMousePosition.X;
                    int deltaY = currentMousePosition.Y - lastMousePosition.Y;

                    this.Location = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
                } else {
                    dragReady = true;
                }

                lastMousePosition = currentMousePosition;
            }
        }

        private void CloseButtonClick(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void MinimiseButtonClick(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
