using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace Archeage_Addon_Manager {
    public class WebRequest {
        public async Task DownloadFile(string url, string filePath) {
            using var httpClient = new HttpClient();

            // Send the request and get the response stream asynchronously
            using var response = await httpClient.GetAsync(url);
            using var stream = await response.Content.ReadAsStreamAsync();

            // Save the stream to the specified file path
            using FileStream fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);
        }

        public async Task UploadZipFile(string zipFilePath) {
            using var httpClient = new HttpClient();

            string phpScriptUrl = "https://www.spacemeat.space/aamods/data/upload.php";

            MultipartFormDataContent formData = new MultipartFormDataContent();

            // Add the ZIP file to the form data
            byte[] fileBytes = File.ReadAllBytes(zipFilePath);
            formData.Add(new ByteArrayContent(fileBytes), "zip_file", Path.GetFileName(zipFilePath));

            // Make the POST request
            HttpResponseMessage response = await httpClient.PostAsync(phpScriptUrl, formData);

            // Check the response status
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Error uploading file: " + response.ReasonPhrase);

            //await response.Content.ReadAsStringAsync();
        }
    }
}
