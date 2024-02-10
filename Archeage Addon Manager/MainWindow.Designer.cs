namespace Archeage_Addon_Manager
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            InstallButton = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            bottomPanel = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            installationPathComboBox = new System.Windows.Forms.ComboBox();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            manageAddonSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            developersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            developerItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            windowDragPanel = new System.Windows.Forms.Panel();
            minimiseWindowButton = new System.Windows.Forms.Button();
            closeWindowButton = new System.Windows.Forms.Button();
            containerPanel = new System.Windows.Forms.Panel();
            statusPanel = new System.Windows.Forms.Panel();
            backupFilesButton = new System.Windows.Forms.Button();
            restoreFilesButton = new System.Windows.Forms.Button();
            bottomPanel.SuspendLayout();
            menuStrip1.SuspendLayout();
            windowDragPanel.SuspendLayout();
            containerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // InstallButton
            // 
            InstallButton.BackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            InstallButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            InstallButton.FlatAppearance.BorderSize = 0;
            InstallButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            InstallButton.Font = new System.Drawing.Font("Ebrima", 13F, System.Drawing.FontStyle.Bold);
            InstallButton.ForeColor = System.Drawing.Color.White;
            InstallButton.Location = new System.Drawing.Point(479, 0);
            InstallButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new System.Drawing.Size(141, 80);
            InstallButton.TabIndex = 0;
            InstallButton.Text = "APPLY\r\nPATCHES";
            InstallButton.UseVisualStyleBackColor = false;
            InstallButton.Click += InstallButtonClick;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.BackColor = System.Drawing.Color.Transparent;
            panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            panel1.Location = new System.Drawing.Point(10, 10);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(370, 328);
            panel1.TabIndex = 2;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            bottomPanel.Controls.Add(label1);
            bottomPanel.Controls.Add(installationPathComboBox);
            bottomPanel.Controls.Add(InstallButton);
            bottomPanel.Location = new System.Drawing.Point(0, 397);
            bottomPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(620, 83);
            bottomPanel.TabIndex = 3;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(13, 16);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(101, 15);
            label1.TabIndex = 1;
            label1.Text = "0 addons selected";
            // 
            // installationPathComboBox
            // 
            installationPathComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            installationPathComboBox.FormattingEnabled = true;
            installationPathComboBox.Location = new System.Drawing.Point(10, 48);
            installationPathComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            installationPathComboBox.Name = "installationPathComboBox";
            installationPathComboBox.Size = new System.Drawing.Size(458, 23);
            installationPathComboBox.TabIndex = 2;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = System.Drawing.Color.Transparent;
            menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            menuStrip1.GripMargin = new System.Windows.Forms.Padding(2);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, developersToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(-1, 2);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(5, 3, 0, 2);
            menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            menuStrip1.Size = new System.Drawing.Size(165, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { manageAddonSourcesToolStripMenuItem, propertiesToolStripMenuItem });
            fileToolStripMenuItem.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            fileToolStripMenuItem.Text = "File";
            // 
            // manageAddonSourcesToolStripMenuItem
            // 
            manageAddonSourcesToolStripMenuItem.Image = Properties.Resources.update;
            manageAddonSourcesToolStripMenuItem.Name = "manageAddonSourcesToolStripMenuItem";
            manageAddonSourcesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            manageAddonSourcesToolStripMenuItem.Text = "Check For Updates";
            // 
            // propertiesToolStripMenuItem
            // 
            propertiesToolStripMenuItem.Image = Properties.Resources.settings;
            propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            propertiesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            propertiesToolStripMenuItem.Text = "Settings";
            // 
            // developersToolStripMenuItem
            // 
            developersToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            developersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { developerItemToolStripMenuItem });
            developersToolStripMenuItem.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            developersToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            developersToolStripMenuItem.Name = "developersToolStripMenuItem";
            developersToolStripMenuItem.Size = new System.Drawing.Size(77, 19);
            developersToolStripMenuItem.Text = "Developers";
            // 
            // developerItemToolStripMenuItem
            // 
            developerItemToolStripMenuItem.Image = Properties.Resources._lock;
            developerItemToolStripMenuItem.Name = "developerItemToolStripMenuItem";
            developerItemToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            developerItemToolStripMenuItem.Text = "Developer Login";
            developerItemToolStripMenuItem.Click += DeveloperActionButtonClick;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { creditsToolStripMenuItem });
            helpToolStripMenuItem.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 19);
            helpToolStripMenuItem.Text = "Help";
            // 
            // creditsToolStripMenuItem
            // 
            creditsToolStripMenuItem.Image = Properties.Resources.about;
            creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            creditsToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            creditsToolStripMenuItem.Text = "About";
            creditsToolStripMenuItem.Click += ShowAboutDialog;
            // 
            // windowDragPanel
            // 
            windowDragPanel.BackColor = System.Drawing.Color.FromArgb(100, 0, 0, 0);
            windowDragPanel.Controls.Add(minimiseWindowButton);
            windowDragPanel.Controls.Add(closeWindowButton);
            windowDragPanel.Controls.Add(menuStrip1);
            windowDragPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            windowDragPanel.Location = new System.Drawing.Point(0, 0);
            windowDragPanel.MaximumSize = new System.Drawing.Size(0, 30);
            windowDragPanel.Name = "windowDragPanel";
            windowDragPanel.Size = new System.Drawing.Size(620, 30);
            windowDragPanel.TabIndex = 5;
            windowDragPanel.MouseDown += windowDragPanel_MouseDown;
            windowDragPanel.MouseMove += windowDragPanel_MouseMove;
            windowDragPanel.MouseUp += windowDragPanel_MouseUp;
            // 
            // minimiseWindowButton
            // 
            minimiseWindowButton.BackColor = System.Drawing.Color.Transparent;
            minimiseWindowButton.FlatAppearance.BorderSize = 0;
            minimiseWindowButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(33, 35, 38);
            minimiseWindowButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            minimiseWindowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            minimiseWindowButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            minimiseWindowButton.ForeColor = System.Drawing.Color.White;
            minimiseWindowButton.Location = new System.Drawing.Point(500, 0);
            minimiseWindowButton.Name = "minimiseWindowButton";
            minimiseWindowButton.Size = new System.Drawing.Size(60, 30);
            minimiseWindowButton.TabIndex = 6;
            minimiseWindowButton.Text = "-";
            minimiseWindowButton.UseVisualStyleBackColor = false;
            minimiseWindowButton.Click += MinimiseButtonClick;
            // 
            // closeWindowButton
            // 
            closeWindowButton.BackColor = System.Drawing.Color.Transparent;
            closeWindowButton.FlatAppearance.BorderSize = 0;
            closeWindowButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(33, 35, 38);
            closeWindowButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            closeWindowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            closeWindowButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            closeWindowButton.ForeColor = System.Drawing.Color.White;
            closeWindowButton.Location = new System.Drawing.Point(560, 0);
            closeWindowButton.Name = "closeWindowButton";
            closeWindowButton.Size = new System.Drawing.Size(60, 30);
            closeWindowButton.TabIndex = 5;
            closeWindowButton.Text = "X";
            closeWindowButton.UseVisualStyleBackColor = false;
            closeWindowButton.Click += CloseButtonClick;
            // 
            // containerPanel
            // 
            containerPanel.BackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            containerPanel.Controls.Add(panel1);
            containerPanel.Location = new System.Drawing.Point(220, 40);
            containerPanel.Name = "containerPanel";
            containerPanel.Size = new System.Drawing.Size(390, 348);
            containerPanel.TabIndex = 8;
            // 
            // statusPanel
            // 
            statusPanel.BackColor = System.Drawing.Color.Transparent;
            statusPanel.BackgroundImage = Properties.Resources.daru_green;
            statusPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            statusPanel.Location = new System.Drawing.Point(2, 296);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new System.Drawing.Size(140, 113);
            statusPanel.TabIndex = 7;
            // 
            // backupFilesButton
            // 
            backupFilesButton.BackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            backupFilesButton.FlatAppearance.BorderSize = 0;
            backupFilesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            backupFilesButton.Font = new System.Drawing.Font("Ebrima", 11F);
            backupFilesButton.ForeColor = System.Drawing.Color.White;
            backupFilesButton.Location = new System.Drawing.Point(10, 40);
            backupFilesButton.Name = "backupFilesButton";
            backupFilesButton.Size = new System.Drawing.Size(203, 60);
            backupFilesButton.TabIndex = 9;
            backupFilesButton.Text = "Backup Game Files";
            backupFilesButton.UseVisualStyleBackColor = false;
            backupFilesButton.Click += backupFilesButton_Click;
            // 
            // restoreFilesButton
            // 
            restoreFilesButton.BackColor = System.Drawing.Color.FromArgb(200, 33, 35, 38);
            restoreFilesButton.FlatAppearance.BorderSize = 0;
            restoreFilesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            restoreFilesButton.Font = new System.Drawing.Font("Ebrima", 11F);
            restoreFilesButton.ForeColor = System.Drawing.Color.White;
            restoreFilesButton.Location = new System.Drawing.Point(10, 109);
            restoreFilesButton.Name = "restoreFilesButton";
            restoreFilesButton.Size = new System.Drawing.Size(203, 60);
            restoreFilesButton.TabIndex = 10;
            restoreFilesButton.Text = "Restore Game Files\r\nFrom Backup File";
            restoreFilesButton.UseVisualStyleBackColor = false;
            restoreFilesButton.Click += restoreFilesButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new System.Drawing.Size(620, 480);
            ControlBox = false;
            Controls.Add(restoreFilesButton);
            Controls.Add(backupFilesButton);
            Controls.Add(containerPanel);
            Controls.Add(windowDragPanel);
            Controls.Add(bottomPanel);
            Controls.Add(statusPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(620, 480);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(620, 480);
            Name = "MainWindow";
            ShowIcon = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "Archeage Addon Manager";
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            windowDragPanel.ResumeLayout(false);
            windowDragPanel.PerformLayout();
            containerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAddonSourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem developersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem developerItemToolStripMenuItem;
        private System.Windows.Forms.ComboBox installationPathComboBox;
        private System.Windows.Forms.Panel windowDragPanel;
        private System.Windows.Forms.Button closeWindowButton;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Button minimiseWindowButton;
        private System.Windows.Forms.Panel containerPanel;
        private System.Windows.Forms.Button backupFilesButton;
        private System.Windows.Forms.Button restoreFilesButton;
    }
}

