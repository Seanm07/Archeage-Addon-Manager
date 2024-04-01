using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Archeage_Addon_Manager {
    internal class DeveloperManager {
        public static DeveloperManager instance;

        public bool isLoggedIn;
        public bool isDeveloper;

        public DeveloperManager() {
            instance ??= this;
        }        

        public void CheckLoggedInState() {
            if (!string.IsNullOrWhiteSpace(ProgramManager.ReadFromConfigFile("discord_access_token", ""))) {
                long expiryTimestamp = long.Parse(ProgramManager.ReadFromConfigFile("discord_token_expiry", "0"));

                // Check if the access token expires in the next 48 hours
                if (DateTimeOffset.Now.ToUnixTimeSeconds() + 172800 > expiryTimestamp) {
                    RefreshDiscordAccessToken();
                } else {
                    isLoggedIn = true;
                    isDeveloper = ProgramManager.ReadFromConfigFile("is_developer", "false") == "true";

                    // Send a request to update the users developer status
                    SendDeveloperCheckRequest();
                }
            } else {
                Logout();
            }

            MainWindow.instance.UpdateDeveloperButtonState();
        }

        public async void RefreshDiscordAccessToken() {
            string refreshToken = ProgramManager.ReadFromConfigFile("discord_refresh_token", "");

            if (!string.IsNullOrWhiteSpace(refreshToken)) {
                await new WebRequest().DoRequest("https://www.spacemeat.space/aamods/api/request.php?action=refresh_auth&refresh_token=" + refreshToken, (string response) => {
                    ProcessDiscordOAuthResponse(response);
                });
            } else {
                Logout();
            }

            MainWindow.instance.UpdateDeveloperButtonState();
        }

        public void Logout(bool userAction = false) {
            ProgramManager.WriteToConfigFile("discord_access_token", "");
            ProgramManager.WriteToConfigFile("discord_token_expiry", "");
            ProgramManager.WriteToConfigFile("discord_refresh_token", "");
            ProgramManager.WriteToConfigFile("is_developer", "false");

            isLoggedIn = false;
            isDeveloper = false;

            MainWindow.instance.UpdateDeveloperButtonState();

            if (userAction) {
                MainWindow.instance.ShowMessagePopup("Logged out!", "Your discord has been unlinked and you are now logged out.", "Ok");
            }
        }

        private void ProcessDiscordOAuthResponse(string response) {
            Console.WriteLine(response);

            JObject jsonResponse = JObject.Parse(response);

            try {
                if (jsonResponse.TryGetValue("access_token", out JToken? accessToken) && accessToken != null) {
                    string accessTokenString = accessToken.ToString();

                    if (!string.IsNullOrWhiteSpace(accessTokenString))
                        ProgramManager.WriteToConfigFile("discord_access_token", accessTokenString);
                } else {
                    throw new IOException("Invalid discord access token!");
                }

                if (jsonResponse.TryGetValue("expires_in", out JToken? expiresIn) && expiresIn != null) {
                    // Current timestamp + expires_in = expiry timestamp
                    string accessTokenExpiry = (DateTimeOffset.Now.ToUnixTimeSeconds() + long.Parse(expiresIn.ToString())).ToString();

                    if (!string.IsNullOrWhiteSpace(accessTokenExpiry))
                        ProgramManager.WriteToConfigFile("discord_token_expiry", accessTokenExpiry);
                } else {
                    throw new IOException("Invalid discord access token!");
                }

                if (jsonResponse.TryGetValue("refresh_token", out JToken? refreshToken) && refreshToken != null) {
                    string refreshTokenString = refreshToken.ToString();

                    if (!string.IsNullOrWhiteSpace(refreshTokenString))
                        ProgramManager.WriteToConfigFile("discord_refresh_token", refreshTokenString);
                } else {
                    throw new IOException("Invalid discord refresh token!");
                }

                isLoggedIn = true;

                // Send a request to check if the user is a developer
                SendDeveloperCheckRequest();
            } catch (IOException e) {
                MainWindow.instance.ShowMessagePopup("Failed to authenticate with Discord!", e.Message, "Retry", () => LoginButtonClick(), "Cancel");

                Logout();
            }
        }

        public void LoginButtonClick() {
            // We open in an external browser because it would be pretty sketchy needing to login to discord within the program
            WebRequest.ExternalOpenURL("https://discord.com/api/oauth2/authorize?client_id=1203510156529369178&response_type=code&redirect_uri=https%3A%2F%2Fwww.spacemeat.space%2Faamods%2Fapi%2Fauth.php&scope=guilds");

            MainWindow.instance.DisplayLoginOverlay();
        }

        private async void SendDeveloperCheckRequest() {
            await new WebRequest().DoRequest("https://www.spacemeat.space/aamods/api/request.php?action=check_developer&access_token=" + ProgramManager.ReadFromConfigFile("discord_access_token", ""), (string response) => {
                JObject jsonResponse = JObject.Parse(response);

                if (jsonResponse.TryGetValue("is_developer", out JToken? isDeveloperToken) && isDeveloperToken != null) {
                    isDeveloper = isDeveloperToken.ToString() == "1";
                    ProgramManager.WriteToConfigFile("is_developer", isDeveloper ? "true" : "false");
                } else {
                    throw new IOException("Failed to check if user is a developer!");
                }

                MainWindow.instance.UpdateDeveloperButtonState();
            });
        }

        public async void LoginLinkButtonConfirm(string discordLinkCode) {
            MainWindow.instance.CloseLoginOverlay();

            MainWindow.instance.DisplayLoadingOverlay("Logging in..", "Authenticating with Discord..");

            await new WebRequest().DoRequest("https://www.spacemeat.space/aamods/api/request.php?action=new_auth&code=" + discordLinkCode, (string response) => {
                ProcessDiscordOAuthResponse(response);

                MainWindow.instance.CloseLoadingOverlay();
                MainWindow.instance.UpdateDeveloperButtonState();
            });
        }

        public bool BuildAddon(string installationPath, string addonFolderPath, out AddonDataManager.AddonData addonInfo, out string jsonPath) {
            AddonDataManager.instance.GetAddonSrcInfo(addonFolderPath, out addonInfo, out List<string> filePaths);

            // TODO: Make a nicer looking form for the user to enter all the info at once while the addon packs in the background
            addonInfo.name = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for your addon", "Addon Name", Path.GetFileName(addonFolderPath));
            addonInfo.description = Microsoft.VisualBasic.Interaction.InputBox("Enter a description for your addon", "Addon Description", "No description provided");
            addonInfo.version = float.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter a version number for your addon", "Addon Version", "1.00"));
            addonInfo.author = Environment.UserName;
            addonInfo.packagedFileName = addonInfo.name.Replace(" ", "_").ToLower();

            string jsonOutput = AddonDataManager.instance.CreateJsonForFolder(addonInfo);

            // Create a text file named addon.json at FileUtil.TempFilePath() + "addon.json" and write the jsonOutput to it
            jsonPath = FileUtil.TempFilePath() + "addon.json";
            File.WriteAllText(jsonPath, jsonOutput);

            //MessageBox.Show("These scripts were found in your addon!\nThey'll be extracted from your game_pak as a backup.\n\n" + string.Join("\n", filePaths));

            try {
                if (!PakManager.GeneratePakFile(addonFolderPath, "mod"))
                    throw new IOException("Failed to generate addon pak file!");

                if (!PakManager.GenerateUninstallPakFile(installationPath + @"\game_pak", filePaths.ToArray(), "default"))
                    throw new IOException("Failed to generate uninstall pak file!");

                string[] addonFiles = [
                    FileUtil.TempFilePath() + "addon.json",
                    FileUtil.TempFilePath() + "mod.pak",
                    FileUtil.TempFilePath() + "default.pak"
                ];

                // Move the addon files into a zip
                if (!FileUtil.CreateZipFile(addonFiles, FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip"))
                    throw new IOException("Failed to build addon zip file!");
            } catch (IOException ex) {
                MainWindow.instance.ShowMessagePopup("Failed to package addon!", ex.Message, "Retry", () => UploadAddonButtonClick(installationPath), "Cancel");
                return false;
            }

            return true;
        }

        public bool UnpackAddon(string outputFolder, string addonFilePath) {
            try {
                if(!PakManager.ExtractPakFile(outputFolder, addonFilePath))
                    throw new IOException("Failed to extract addon pak file!");
            } catch (IOException ex) {
                MainWindow.instance.ShowMessagePopup("Failed to unpack addon!", ex.Message, "Retry", () => UnpackAddon(outputFolder, addonFilePath), "Cancel");
                return false;
            }

            return true;
        }

        private void CleanupTempFiles(string jsonPath, AddonDataManager.AddonData addonInfo) {
            // Cleanup the json file
            File.Delete(jsonPath);

            // Cleanup the pak files
            File.Delete(FileUtil.TempFilePath() + "mod.pak");
            File.Delete(FileUtil.TempFilePath() + "default.pak");

            // Cleanup the zip file
            File.Delete(FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip");
        }

        public async void UploadAddonButtonClick(string installationPath) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() {
                Description = "Select root directory of addon source"
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                string selectedFolder = folderBrowserDialog.SelectedPath;

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Building addon data");
                await Task.Delay(1); // Wait a frame to allow the loading overlay to display before the pak generation begins

                if (BuildAddon(installationPath, selectedFolder, out AddonDataManager.AddonData addonInfo, out string jsonPath)) {
                    MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Uploading " + addonInfo.packagedFileName + " to cloud");

                    WebRequest webRequest = new WebRequest();
                    await webRequest.UploadZipFile(FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip");
                }

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Cleaning up temporary files");
                await Task.Delay(500); // half a second delay so the loading text is readable

                // Cleanup the temp files
                CleanupTempFiles(jsonPath, addonInfo);

                // Reload addons
                AddonDataManager.instance.ReloadAddonsFromDataSources();

                MainWindow.instance.DisplayLoadingOverlay("Uploading addon..", "Done! " + addonInfo.packagedFileName + " has successfully been published.");
                await Task.Delay(500); // half a second delay so the loading text is readable

                MainWindow.instance.CloseLoadingOverlay();
            }
        }

        public async void PackAddonButtonClick(string installationPath) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() {
                Description = "Select root directory of addon source (Parent folder of game/)"
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                string selectedFolder = folderBrowserDialog.SelectedPath;

                // Make sure the child folder of selectedFolder is named "game"
                if (!Directory.Exists(selectedFolder + "/game")) {
                    // Show a message box error allowing the user to retry or cancel, retry will rerun this function
                    MainWindow.instance.ShowMessagePopup("Invalid addon source!", "Your addon must match the game_pak folder structure! Your root folder must be named 'game'.", "Retry", () => PackAddonButtonClick(installationPath), "Cancel");
                    return;
                }

                MainWindow.instance.DisplayLoadingOverlay("Packing local addon..", "Building addon data");
                await Task.Delay(1); // Wait a frame to allow the loading overlay to display before the pak generation begins

                if (BuildAddon(installationPath, selectedFolder, out AddonDataManager.AddonData addonInfo, out string jsonPath)) {
                    MainWindow.instance.DisplayLoadingOverlay("Packing local addon..", "Moving " + addonInfo.packagedFileName + " to addon directory");

                    File.Move(FileUtil.TempFilePath() + addonInfo.packagedFileName + ".zip", Directory.GetParent(selectedFolder).FullName + "/" + addonInfo.packagedFileName + ".zip");
                }

                MainWindow.instance.DisplayLoadingOverlay("Packing local addon..", "Cleaning up temporary files");
                await Task.Delay(500); // half a second delay so the loading text is readable

                // Cleanup the temp files
                CleanupTempFiles(jsonPath, addonInfo);

                MainWindow.instance.DisplayLoadingOverlay("Packing local addon..", "Done! " + addonInfo.packagedFileName + " has successfully been built.");
                await Task.Delay(500); // half a second delay so the loading text is readable

                MainWindow.instance.CloseLoadingOverlay();
            }
        }

        public async void UnpackAddonButtonClick(string installationPath) {
            OpenFileDialog fileDialog = new OpenFileDialog() {
                Title = "Select .pak or .zip containing mod.pak to unpack",
                Filter = "Packaged Addon (*.pak;*.zip)|*.pak;*.zip",
                Multiselect = false,
                ValidateNames = true,
                CheckFileExists = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK) {
                string selectedFilePath = fileDialog.FileName;

                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() {
                    Description = "Select root directory of addon source (Parent folder of game/)"
                };

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                    MainWindow.instance.DisplayLoadingOverlay("Unpacking local addon..", "Building addon data");
                    await Task.Delay(1); // Wait a frame to allow the loading overlay to display before the pak generation begins

                    if(UnpackAddon(folderBrowserDialog.SelectedPath, selectedFilePath)) {
                        MainWindow.instance.DisplayLoadingOverlay("Unpacking local addon..", "Successfully unpacked addon");
                        await Task.Delay(500); // half a second delay so the loading text is readable

                        MainWindow.instance.CloseLoadingOverlay();
                    }
                }
            }
        }

        public async void UnpackGamePakButtonClick(string installationPath) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() {
                Description = "Select folder to unpack game pak to"
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                MainWindow.instance.DisplayLoadingOverlay("Unpacking game pak..", "Unpacking");
                await Task.Delay(1); // Wait a frame to allow the loading overlay to display before the pak generation begins

                if (UnpackAddon(folderBrowserDialog.SelectedPath, installationPath + "/game_pak")) {
                    MainWindow.instance.DisplayLoadingOverlay("Unpacking game pak..", "Successfully unpacked");
                    await Task.Delay(500); // half a second delay so the loading text is readable

                    MainWindow.instance.CloseLoadingOverlay();
                }
            }
        }

    }
}
