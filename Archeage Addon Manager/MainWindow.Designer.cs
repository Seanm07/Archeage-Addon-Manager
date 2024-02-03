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
            panel2 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            installationPathComboBox = new System.Windows.Forms.ComboBox();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            manageAddonSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            developersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            uploadAddonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            windowDragPanel = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            panel2.SuspendLayout();
            menuStrip1.SuspendLayout();
            windowDragPanel.SuspendLayout();
            SuspendLayout();
            // 
            // InstallButton
            // 
            InstallButton.BackColor = System.Drawing.Color.FromArgb(53, 57, 62);
            InstallButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            InstallButton.FlatAppearance.BorderSize = 0;
            InstallButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            InstallButton.Font = new System.Drawing.Font("Philosopher", 14.2499981F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            InstallButton.ForeColor = System.Drawing.Color.FromArgb(154, 161, 175);
            InstallButton.Location = new System.Drawing.Point(466, 11);
            InstallButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new System.Drawing.Size(137, 60);
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
            panel1.Location = new System.Drawing.Point(227, 45);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(376, 332);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.Transparent;
            panel2.Controls.Add(label1);
            panel2.Controls.Add(installationPathComboBox);
            panel2.Controls.Add(InstallButton);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 397);
            panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(620, 83);
            panel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(30, 16);
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
            installationPathComboBox.Location = new System.Drawing.Point(13, 43);
            installationPathComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            installationPathComboBox.Name = "installationPathComboBox";
            installationPathComboBox.Size = new System.Drawing.Size(441, 23);
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
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            menuStrip1.Size = new System.Drawing.Size(287, 24);
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
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
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
            developersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { uploadAddonToolStripMenuItem });
            developersToolStripMenuItem.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            developersToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            developersToolStripMenuItem.Name = "developersToolStripMenuItem";
            developersToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            developersToolStripMenuItem.Text = "Developers";
            // 
            // uploadAddonToolStripMenuItem
            // 
            uploadAddonToolStripMenuItem.Image = Properties.Resources.cloud_upload;
            uploadAddonToolStripMenuItem.Name = "uploadAddonToolStripMenuItem";
            uploadAddonToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            uploadAddonToolStripMenuItem.Text = "Upload Addon";
            uploadAddonToolStripMenuItem.Click += UploadAddonButtonClick;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { creditsToolStripMenuItem });
            helpToolStripMenuItem.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // creditsToolStripMenuItem
            // 
            creditsToolStripMenuItem.Image = Properties.Resources.about;
            creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            creditsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            creditsToolStripMenuItem.Text = "About";
            creditsToolStripMenuItem.Click += ShowAboutDialog;
            // 
            // windowDragPanel
            // 
            windowDragPanel.BackColor = System.Drawing.Color.Transparent;
            windowDragPanel.Controls.Add(button1);
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
            // button1
            // 
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            button1.ForeColor = System.Drawing.Color.White;
            button1.Location = new System.Drawing.Point(559, 0);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(60, 30);
            button1.TabIndex = 5;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(62, 273);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(104, 30);
            label2.TabIndex = 6;
            label2.Text = "Current loading\r\naction shown here";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new System.Drawing.Size(620, 480);
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(windowDragPanel);
            Controls.Add(panel1);
            Controls.Add(panel2);
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
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            windowDragPanel.ResumeLayout(false);
            windowDragPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAddonSourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem developersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadAddonToolStripMenuItem;
        private System.Windows.Forms.ComboBox installationPathComboBox;
        private System.Windows.Forms.Panel windowDragPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
    }
}

