using System;
using System.Drawing;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {
    public partial class MainWindow : Form {


        public static MainWindow instance;

        private Panel? loginOverlayPanel;
        private Panel? loadingOverlayPanel;

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
            MessageBox.Show("Not endorsed by XLGames or Kakao Games, we don't reflect views or opinion of anyone officially involved in Archeage.\n\nKakao strongly does not recommend addon usage, you accept the potential game breaking risks!\n\nArcheage Addon Manager created by Nidoran\n\nBig thanks to Ingram for all his work and help to make this possible and his work on AAPatcher.\n\nAdditional thanks to Tamaki & Strawberry", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeveloperActionButtonClick(object sender, EventArgs e) {
            if (DeveloperManager.instance.isLoggedIn) {
                if (DeveloperManager.instance.isDeveloper) {
                    DeveloperManager.instance.UploadAddonButtonClick(installationPathComboBox.Text);
                } else {
                    MessageBox.Show("Sorry but you don't have permission to upload addons!\nDM Nidoran on discord if you're interested in getting upload access.", "Developer access required!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                DeveloperManager.instance.LoginButtonClick();
            }
        }

        public void DisplayLoadingOverlay(string loadingText, string subLoadingText) {
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
        }

        public void DisplayLoginOverlay() {
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
