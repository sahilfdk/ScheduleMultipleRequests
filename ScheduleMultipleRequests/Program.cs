using ScheduleMultipleRequests;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Hardcoded path to the directory containing JSON request files
        string jsonRequestsDirectory = @"C:\Users\kn\source\github\ScheduleMultipleRequests\ScheduleMultipleRequests\jsonRequests";

        // Hardcoded path to the fingerprints file
        string fingerprintsFilePath = @"C:\Users\kn\source\github\ScheduleMultipleRequests\ScheduleMultipleRequests\fingerprints.json";

        // Read fingerprints from the file
        string fingerprintsContent = await File.ReadAllTextAsync(fingerprintsFilePath);
        Dictionary<string, string> fingerprints = JsonSerializer.Deserialize<Dictionary<string, string>>(fingerprintsContent);

        // Get list of JSON files
        string[] jsonFiles = Directory.GetFiles(jsonRequestsDirectory, "*.json");

        // List to store requests
        List<(JsonDocument, string)> requests = new List<(JsonDocument, string)>();

        // Create requests
        foreach (var jsonFile in jsonFiles)
        {
            string jsonContent = await File.ReadAllTextAsync(jsonFile);
            JsonDocument requestData = JsonDocument.Parse(jsonContent);

            // Get the fingerprint corresponding to the current JSON file
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFile);
            if (fingerprints.TryGetValue(fileNameWithoutExtension, out string fingerprint))
            {
                requests.Add((requestData, fingerprint));
            }
            else
            {
                Console.WriteLine($"Fingerprint not found for file: {jsonFile}");
            }
        }

        ConfigurableItems.PrintWaitingMessage();
        await WaitUntilConfiguredTimeAsync();

        // Start the requests
        Console.WriteLine("It's time! Starting the requests...");

        // Start the requests
        List<Task> tasks = new List<Task>();
        foreach (var (requestData, fingerprint) in requests)
        {
            for (int i = 0; i < 500; i++)
            {
                tasks.Add(MakeApiCallsAsync(requestData, fingerprint));
            }
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        Console.WriteLine("All requests completed.");

        Console.ReadLine();
    }

    static async Task WaitUntilConfiguredTimeAsync()
    {
        while (true)
        {
            
            if (ConfigurableItems.IsTimeToStart())
            {
                break;
            }

            // Wait for a short duration before checking again
            await Task.Delay(1000);
            ConfigurableItems.PrintWaitingMessage();
        }
    }    

    static async Task MakeApiCallsAsync(JsonDocument requestData, string fingerprint)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            handler.UseCookies = false;

            using (HttpClient client = new HttpClient(handler))
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://csc.fcappservices.in/ShoppingCart/ShoppingCart.svc/json/LoadCart");

                // Adding headers
                request.Headers.Add("Host", "csc.fcappservices.in");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"125\", \"Chromium\";v=\"125\", \"Not.A/Brand\";v=\"24\"");
                request.Headers.Add("fingerprint", fingerprint);
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
                request.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                request.Headers.Add("fcdid", "");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("Origin", "https://checkout.firstcry.com");
                request.Headers.Add("Sec-Fetch-Site", "cross-site");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Referer", "https://checkout.firstcry.com/");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");

                // Adding content
                var content = new StringContent(requestData.RootElement.GetRawText(), Encoding.UTF8, "application/json");
                request.Content = content;

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    Console.WriteLine($"Response status code : {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with request {request.RequestUri}: {ex.Message}");
                }
            }
        }
    }
}
