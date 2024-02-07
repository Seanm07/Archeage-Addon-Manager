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
            using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);
        }
    }
}
