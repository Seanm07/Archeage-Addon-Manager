using Archeage_Addon_Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

public class AddonDataManager
{
    public static AddonDataManager instance;

    public class AddonData
    {
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public float version { get; set; }
        public string dataPath { get; set; }
    }

    public class FolderInfo
    {
        public string foldername { get; set; }
        public string folderpath { get; set; }
        public List<FileInfo> files { get; set; } = new List<FileInfo>();
        public List<FolderInfo> subFolders { get; set; } = new List<FolderInfo>();
    }

    public class FileInfo
    {
        public string filename { get; set; }
        public string filepath { get; set; }
        public long filesize { get; set; }
        public string checksum { get; set; }
    }

    List<AddonData> addons = new List<AddonData>();

    public AddonDataManager()
    {
        instance = instance ?? this;
    }

    public void LoadAddonsFromDataSources()
    {
        AddAddonsFromURL("https://www.spacemeat.space/aamods/data.json");
    }

    // Load addons from a URL containing a JSON array of AddonData objects
    public void AddAddonsFromURL(string url)
    {
        try
        {
            // Send a web request to the specified URL which should return a JSON array of AddonData objects
            string jsonInput = new System.Net.WebClient().DownloadString(url);

            // Deserialize the JSON array into a list of AddonData objects
            List<AddonData> addonList = JsonConvert.DeserializeObject<List<AddonData>>(jsonInput);

            // Iterate through each addon and call AddAddon
            foreach (AddonData addon in addonList)
            {
                // Store the addon data in a list for later referencing
                addons.Add(addon);

                // Create a widget in the main window for this addon
                MainWindow.instance.AddAddonWidget(addon);
            }
        }
        catch (Exception e)
        {
            DialogResult selectedOption = MessageBox.Show(url + " failed to load! " + e.Message, "Failed to load addon source!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);

            switch (selectedOption) {
                case DialogResult.Abort: Environment.Exit(0); break;
                case DialogResult.Retry: AddAddonsFromURL(url); break;
            }
        }
    }

    private string CalculateChecksum(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

    private long GetFileSize(string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            return stream.Length;
        }
    }

    private FileInfo GetFileInfo(string filePath, string basePath)
    {
        return new FileInfo
        {
            filename = Path.GetFileName(filePath),
            filepath = filePath.Replace(basePath, "").Replace("\\", "/"),
            filesize = GetFileSize(filePath),
            checksum = CalculateChecksum(filePath)
        };
    }

    private FolderInfo GetFolderInfo(string folderPath, string basePath)
    {
        bool curFolderIsBase = folderPath == basePath;

        // Get the folder info for the current folder
        var folderInfo = new FolderInfo
        {
            foldername = Path.GetFileName(folderPath),
            folderpath = folderPath,
            files = new List<FileInfo>()
        };

        // Add all files in this folder to the list of files
        foreach (var filePath in Directory.GetFiles(folderPath))
        {
            var fileInfo = GetFileInfo(filePath, basePath);
            folderInfo.files.Add(fileInfo);
        }

        // Add all subfolders in this folder to the list of files
        foreach (var subFolderPath in Directory.GetDirectories(folderPath))
        {
            var subFolderInfo = GetFolderInfo(subFolderPath, basePath);
            folderInfo.subFolders.Add(subFolderInfo);
        }

        // If we're currently in the base folder, strip folder information about the base folder
        if (curFolderIsBase) {
            folderInfo.foldername = folderInfo.folderpath = null;
        } else {
            // Strip the base path from the folder path and replace backslashes with forward slashes
            folderInfo.folderpath = folderInfo.folderpath.Replace(basePath, "").Replace("\\", "/");
        }

        return folderInfo;
    }

    public string CreateJsonForFolder(string folderPath)
    {
        var folderInfo = GetFolderInfo(folderPath, folderPath);
        var jsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(folderInfo, jsonSettings);
    }

    // Search for ArcheAge installation paths on all drives
    public string[] FindInstallationPaths() {
        List<string> validPaths = new List<string>();

        foreach (DriveInfo drive in DriveInfo.GetDrives()) {
            string[] possiblePaths =
            {
                Path.Combine(drive.Name, "KakaoGames", "ArcheAge"),
                Path.Combine(drive.Name, "Steam", "steamapps", "common", "ArcheAge"),
                Path.Combine(drive.Name, "SteamLibrary", "steamapps", "common", "ArcheAge"),
                Path.Combine(drive.Name, "Program Files (x86)", "steamapps", "common", "ArcheAge")
            };

            // Under certain installs the ArcheAge folder gets duplicated and appended with a number, scan for these too
            for (int i = 0; i < 10; i++) {
                possiblePaths = possiblePaths.Concat(new string[] {
                    possiblePaths[0] + i,
                    possiblePaths[1] + i,
                    possiblePaths[2] + i,
                    possiblePaths[3] + i,
                }).ToArray();
            }

            validPaths.AddRange(possiblePaths.Where(path => Directory.Exists(path)));
        }

        validPaths.Add("Browse..");

        return validPaths.ToArray();
    }
}