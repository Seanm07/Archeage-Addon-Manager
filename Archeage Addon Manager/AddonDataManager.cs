using Archeage_Addon_Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

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
        // Send a web request to the specified URL which should return a JSON array of AddonData objects
        string jsonInput = new System.Net.WebClient().DownloadString(url);

        try
        {
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
            Console.WriteLine("Error parsing JSON result from URL " + url + " - " + e.Message);
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

    private FileInfo GetFileInfo(string filePath)
    {
        return new FileInfo
        {
            filename = Path.GetFileName(filePath),
            filepath = filePath,
            filesize = GetFileSize(filePath),
            checksum = CalculateChecksum(filePath)
        };
    }

    private FolderInfo GetFolderInfo(string folderPath)
    {
        var folderInfo = new FolderInfo
        {
            foldername = Path.GetFileName(folderPath),
            folderpath = folderPath,
            files = new List<FileInfo>()
        };

        foreach (var filePath in Directory.GetFiles(folderPath))
        {
            var fileInfo = GetFileInfo(filePath);
            folderInfo.files.Add(fileInfo);
        }

        foreach (var subFolderPath in Directory.GetDirectories(folderPath))
        {
            var subFolderInfo = GetFolderInfo(subFolderPath);
            folderInfo.subFolders.Add(subFolderInfo);
        }

        return folderInfo;
    }

    public string CreateJsonForFolder(string folderPath)
    {
        var folderInfo = GetFolderInfo(folderPath);
        return JsonConvert.SerializeObject(folderInfo, Formatting.Indented);
    }
}