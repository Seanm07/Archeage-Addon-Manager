using System;
using System.Drawing;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : CompositedForm {
        public static MainWindow instance;

        private Panel? loginOverlayPanel;
        private Panel? loadingOverlayPanel;
        private Panel? settingsOverlayPanel;

        public MainWindow() {
            instance ??= this;

            InitializeComponent();

            // Initialise the AddonDataManager
            new AddonDataManager(panel1);

            // Use our custom renderer for the menu strip
            menuStrip1.Renderer = new ToolstripRenderer();

            UpdateDeveloperButtonState();

            // Load addons from data sources
            AddonDataManager.instance.LoadAddonsFromDataSources();

            // Find archeage installation directories
            installationPathComboBox.Items.AddRange(AddonDataManager.instance.FindInstallationPaths());
            installationPathComboBox.SelectedIndex = 0;
        }

        // Apply Patches bottom right form button clicked
        private void InstallButtonClick(object sender, EventArgs e) {
            PakManager.InstallSelectedAddons(installationPathComboBox.Text);
        }

        public void OnClickAddonWidget(int addonIndex) {
            AddonDataManager.instance.ToggleAddonWidgetChecked(addonIndex);

            OnCheckboxesUpdated();
        }

        public void OnCheckboxesUpdated() {
            int totalChecked = AddonDataManager.instance.GetCheckedAddonWidgetCount();

            label1.Text = totalChecked + " addon" + (totalChecked > 1 ? "s" : "") + " selected";
        }

        public void ClearAddonWidgets() {
            panel1.Controls.Clear();
            AddonDataManager.instance.addonWidgets.Clear();
        }

        public void ShowAboutDialog(object sender, EventArgs e) {
            ShowMessagePopup("About ArcheAge Addon Manager", "Not endorsed by XLGames or Kakao Games!\nWe don't reflect views or opinion of anyone officially involved in Archeage.Kakao strongly doesn't recommend addon usage, you accept potential game breaking risks!\n\nArcheage Addon Manager program created by Nidoran\nBig thanks to Ingram for his AAPatcher work, additional thanks to Tamaki + Strawberry", "Close");
        }

        private void DeveloperActionButtonClick(object sender, EventArgs e) {
            if (DeveloperManager.instance.isLoggedIn) {
                if (DeveloperManager.instance.isDeveloper) {
                    DeveloperManager.instance.UploadAddonButtonClick(installationPathComboBox.Text);
                } else {
                    ShowMessagePopup("Developer access required!", "Sorry but you don't have permission to upload addons!\n\nDM Nidoran on discord if you're interested in getting upload access.", "Ok");
                }
            } else {
                DeveloperManager.instance.LoginButtonClick();
            }
        }

        public void DisplayLoadingOverlay(string loadingText, string subLoadingText) {
            // Set the window to a loading state while the loading overlay is displayed
            this.Cursor = Cursors.WaitCursor;

            if (loadingOverlayPanel == null) {
                loadingOverlayPanel = new Panel() {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(240, 33, 35, 38)
                };

                Label loadingLabel = new Label() {
                    Width = this.Width,
                    Height = 25,
                    Location = new Point(0, (this.Height - 25) / 2 - 35),
                    Text = loadingText,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };

                Label subLoadingLabel = new Label() {
                    Width = this.Width,
                    Height = 20,
                    Location = new Point(0, (this.Height - 20) / 2),
                    Text = subLoadingText,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };

                Controls.Add(loadingOverlayPanel);
                loadingOverlayPanel.Controls.Add(loadingLabel);
                loadingOverlayPanel.Controls.Add(subLoadingLabel);

                loadingOverlayPanel.BringToFront();
                BringMenuBarToFront();
            } else {
                loadingOverlayPanel.Controls[0].Text = loadingText;
                loadingOverlayPanel.Controls[1].Text = subLoadingText;
            }
        }

        public void CloseLoadingOverlay() {
            Controls.Remove(loadingOverlayPanel);
            loadingOverlayPanel = null;

            // Restore default cursor when hiding the loading overlay
            this.Cursor = Cursors.Default;
        }

        public void ShowMessagePopup(string title, string message, string yesValue, Action yesCallback = default, string noValue = "", Action noCallback = default) {
            Panel messagePopupPanel = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 33, 35, 38)
            };

            Label titleLabel = new Label() {
                Width = this.Width,
                Height = 25,
                Location = new Point(0, (this.Height - 25) / 2 - 60),
                Text = title,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            Label messageLabel = new Label() {
                Width = this.Width - 40,
                MinimumSize = new Size(this.Width - 40, 0),
                MaximumSize = new Size(this.Width - 40, 0),
                AutoSize = true,
                Location = new Point(20, (this.Height - 20) / 2 - 30),
                Text = message,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            // Realign the messageLabel to the center of the form
            messageLabel.Location = new Point(messageLabel.Location.X, ((this.Height / 2) - (messageLabel.PreferredHeight / 2)));

            // Adjust titleLabel position based on the messageLabel height
            titleLabel.Location = new Point(titleLabel.Location.X, messageLabel.Bottom - titleLabel.Height - 20);

            Controls.Add(messagePopupPanel);
            messagePopupPanel.Controls.Add(titleLabel);
            messagePopupPanel.Controls.Add(messageLabel);

            if (yesValue != "") {
                bool singleButtonMode = noValue == "";

                Button yesButton = new Button() {
                    Width = singleButtonMode ? 200 : 100,
                    Height = 30,
                    Location = new Point((this.Width - (singleButtonMode ? 200 : 100)) / 2 - (singleButtonMode ? 0 : 60), messageLabel.Bottom + 20),
                    Text = yesValue,
                    BackColor = singleButtonMode ? Color.FromArgb(33, 33, 101) : Color.FromArgb(33, 101, 33),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                messagePopupPanel.Controls.Add(yesButton);

                yesButton.Click += (sender, e) => {
                    yesCallback?.Invoke();
                    HideMessagePopup(messagePopupPanel);
                };
            }

            if (noValue != "") {
                Button noButton = new Button() {
                    Width = 100,
                    Height = 30,
                    Location = new Point((this.Width - 100) / 2 + 60, messageLabel.Bottom + 20),
                    Text = noValue,
                    BackColor = Color.FromArgb(101, 33, 33),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                messagePopupPanel.Controls.Add(noButton);

                noButton.Click += (sender, e) => {
                    noCallback?.Invoke();
                    HideMessagePopup(messagePopupPanel);
                };
            }

            messagePopupPanel.BringToFront();
            BringMenuBarToFront();
        }

        public void HideMessagePopup(Panel messagePopupPanel) {
            if (messagePopupPanel != null && Controls.Contains(messagePopupPanel)) {
                Controls.Remove(messagePopupPanel);
            }
        }


        public void DisplayLoginOverlay() {
            if (loginOverlayPanel != null) return;

            loginOverlayPanel = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 33, 35, 38)
            };

            Label titleLabel = new Label() {
                Width = 400,
                Height = 20,
                Location = new Point((this.Width - 400) / 2, (this.Height - 10) / 2 - 50),
                Text = "Discord Authorisation Required!",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            Label infoLabel = new Label() {
                Width = 400,
                Height = 20,
                Location = new Point((this.Width - 400) / 2, (this.Height - 20) / 2 - 30),
                Text = "Authorise discord in your browser then copy the code below",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            TextBox loginOverlayTextbox = new TextBox() {
                Width = 400,
                Height = 20,
                Location = new Point((this.Width - 400) / 2, (this.Height - 20) / 2),
                Text = "",
                TextAlign = HorizontalAlignment.Center
            };

            Button loginButton = new Button() {
                Width = 100,
                Height = 30,
                Location = new Point((this.Width - 400) / 2 + 400 - 100, (this.Height - 20) / 2 + 30),
                Text = "Link to Discord",
                BackColor = Color.FromArgb(88, 101, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Button cancelButton = new Button() {
                Width = 100,
                Height = 30,
                Location = new Point((this.Width - 400) / 2, (this.Height - 20) / 2 + 30),
                Text = "Cancel",
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(loginOverlayPanel);
            loginOverlayPanel.Controls.Add(titleLabel);
            loginOverlayPanel.Controls.Add(infoLabel);
            loginOverlayPanel.Controls.Add(loginOverlayTextbox);
            loginOverlayPanel.Controls.Add(loginButton);
            loginOverlayPanel.Controls.Add(cancelButton);

            loginOverlayPanel.BringToFront();
            BringMenuBarToFront();

            loginButton.Click += (sender, e) => {
                DeveloperManager.instance.LoginLinkButtonConfirm(loginOverlayTextbox.Text);
            };

            cancelButton.Click += (sender, e) => {
                CloseLoginOverlay();
            };
        }

        public void BringMenuBarToFront() {
            windowDragPanel.BringToFront();
            menuStrip1.BringToFront();
            closeWindowButton.BringToFront();
            minimiseWindowButton.BringToFront();

        }

        public void CloseLoginOverlay() {
            Controls.Remove(loginOverlayPanel);
            loginOverlayPanel = null;
        }

        public void ShowSettingsOverlay() {
            if (settingsOverlayPanel != null) return;

            settingsOverlayPanel = new Panel() {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 33, 35, 38)
            };

            Label titleLabel = new Label() {
                Width = this.Width,
                Height = 25,
                Location = new Point(0, windowDragPanel.Height + 20),
                Text = "Addon Manager Settings",
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            Label pathLabel = new Label() {
                Width = this.Width - 40,
                Height = 20,
                Location = new Point(20, titleLabel.Bottom + 20),
                Text = "Game Installation Path",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            ComboBox installationPathComboBox = new ComboBox() {
                Width = this.Width - 40,
                Height = 30,
                Location = new Point(20, pathLabel.Bottom),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Fill the ComboBox with installation paths
            string[] installationPaths = AddonDataManager.instance.FindInstallationPaths();
            installationPathComboBox.Items.AddRange(installationPaths);
            installationPathComboBox.SelectedIndex = 0; // Select index 0 by default

            Label sourcesLabel = new Label() {
                Width = this.Width - 40,
                Height = 20,
                Location = new Point(20, installationPathComboBox.Bottom + 20),
                Text = "Addon Sources",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            TextBox sourcesTextBox = new TextBox() {
                Width = this.Width - 40,
                Height = 100,
                Multiline = true,
                Location = new Point(20, sourcesLabel.Bottom),
                Text = String.Join("\n", AddonDataManager.instance.GetAddonSourcesList())
            };

            Button closeButton = new Button() {
                Width = 200,
                Height = 30,
                Location = new Point((this.Width - 200) / 2, sourcesTextBox.Bottom + 20),
                Text = "Save and close",
                BackColor = Color.FromArgb(33, 33, 101),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Label versionLabel = new Label() {
                Width = 100,
                Height = 20,
                Location = new Point(this.Width - 120, this.Height - 40),
                Text = "Version: 1.00",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            Button checkUpdatesButton = new Button() {
                Width = 150,
                Height = 30,
                Location = new Point(versionLabel.Left - 160, this.Height - 45),
                Text = "Check for Updates",
                BackColor = Color.FromArgb(88, 101, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(settingsOverlayPanel);
            settingsOverlayPanel.Controls.Add(titleLabel);
            settingsOverlayPanel.Controls.Add(pathLabel);
            settingsOverlayPanel.Controls.Add(installationPathComboBox);
            settingsOverlayPanel.Controls.Add(sourcesLabel);
            settingsOverlayPanel.Controls.Add(sourcesTextBox);
            settingsOverlayPanel.Controls.Add(closeButton);
            settingsOverlayPanel.Controls.Add(versionLabel);
            settingsOverlayPanel.Controls.Add(checkUpdatesButton);

            settingsOverlayPanel.BringToFront();
            BringMenuBarToFront();

            closeButton.Click += (sender, e) => {
                CloseSettingsOverlay();
            };

            checkUpdatesButton.Click += (sender, e) => {
                CheckForUpdates();
            };
        }

        public void CloseSettingsOverlay() {
            Controls.Remove(settingsOverlayPanel);
            settingsOverlayPanel = null;
        }

        public void CheckForUpdates() {
            ShowMessagePopup("No Updates Found", "You're already using the latest version of the Archeage Addon Manager!", "Close");
        }

        public void UpdateDeveloperButtonState() {
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

        private void backupFilesButton_Click(object sender, EventArgs e) {
            DeveloperManager.instance.BackupGamePak(installationPathComboBox.Text);
        }

        private void restoreFilesButton_Click(object sender, EventArgs e) {
            DeveloperManager.instance.RestoreGamePak(installationPathComboBox.Text);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowSettingsOverlay();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e) {
            CheckForUpdates();
        }
    }
}
