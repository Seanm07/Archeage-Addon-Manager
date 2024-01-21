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
            panel2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // InstallButton
            // 
            InstallButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            InstallButton.Location = new System.Drawing.Point(458, 8);
            InstallButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new System.Drawing.Size(137, 60);
            InstallButton.TabIndex = 0;
            InstallButton.Text = "Apply Patch";
            InstallButton.UseVisualStyleBackColor = true;
            InstallButton.Click += InstallButtonClick;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Location = new System.Drawing.Point(0, 25);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(600, 344);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.SystemColors.Window;
            panel2.Controls.Add(label1);
            panel2.Controls.Add(installationPathComboBox);
            panel2.Controls.Add(InstallButton);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 367);
            panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(600, 73);
            panel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 16);
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
            installationPathComboBox.Location = new System.Drawing.Point(8, 43);
            installationPathComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            installationPathComboBox.Name = "installationPathComboBox";
            installationPathComboBox.Size = new System.Drawing.Size(446, 23);
            installationPathComboBox.TabIndex = 2;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, developersToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(600, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { manageAddonSourcesToolStripMenuItem, propertiesToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // manageAddonSourcesToolStripMenuItem
            // 
            manageAddonSourcesToolStripMenuItem.Name = "manageAddonSourcesToolStripMenuItem";
            manageAddonSourcesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            manageAddonSourcesToolStripMenuItem.Text = "Check For Updates";
            // 
            // propertiesToolStripMenuItem
            // 
            propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            propertiesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            propertiesToolStripMenuItem.Text = "Properties";
            // 
            // developersToolStripMenuItem
            // 
            developersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { uploadAddonToolStripMenuItem });
            developersToolStripMenuItem.Name = "developersToolStripMenuItem";
            developersToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            developersToolStripMenuItem.Text = "Developers";
            // 
            // uploadAddonToolStripMenuItem
            // 
            uploadAddonToolStripMenuItem.Name = "uploadAddonToolStripMenuItem";
            uploadAddonToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            uploadAddonToolStripMenuItem.Text = "Upload Addon";
            uploadAddonToolStripMenuItem.Click += UploadAddonButtonClick;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { creditsToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // creditsToolStripMenuItem
            // 
            creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            creditsToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            creditsToolStripMenuItem.Text = "About";
            creditsToolStripMenuItem.Click += ShowAboutDialog;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(600, 440);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(menuStrip1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(616, 479);
            MinimumSize = new System.Drawing.Size(616, 479);
            Name = "MainWindow";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "Archeage Addon Manager";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
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
    }
}

