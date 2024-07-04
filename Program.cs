using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        string authorizationToken = GetAuthorizationToken();

        while (true)
        {
            try
            {
                
                Console.Write("Enter number of repetitions(Warning: Do not enter a repeat count greater than the number of games!): ");
                if (!int.TryParse(Console.ReadLine(), out int repetitions))
                {
                    Console.WriteLine("Invalid number of repetitions.");
                    continue;
                }

                Console.Write("Enter points (suggested between 250 and 300): ");
                string pointsInput = Console.ReadLine();
                int points;

                if (string.IsNullOrEmpty(pointsInput))
                {
                    Random random = new Random();
                    points = random.Next(260, 300);
                    Console.WriteLine($"No points entered. Using random points: {points}");
                }
                else if (!int.TryParse(pointsInput, out points) || points < 10 || points > 2000)
                {
                    Console.WriteLine("Invalid points value. Please enter a number between 10 and 300.");
                    continue;
                }

                List<Task<string>> tasks = new List<Task<string>>();

                for (int i = 0; i < repetitions; i++)
                {
                    tasks.Add(MakeRequestsAsync(authorizationToken, points, i + 1));
                }

                // Wait for all tasks to complete
                string[] results = await Task.WhenAll(tasks);

                // Display all results
                for (int i = 0; i < results.Length; i++)
                {
                    Console.WriteLine($"Result of iteration {i + 1}:\n{results[i]}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Do you want to run again? (yes/no): ");
            string runAgain = Console.ReadLine().ToLower();
            if (runAgain != "yes" && runAgain != "y")
            {
                break;
            }
        }
    }

    static string GetAuthorizationToken()
    { 
        Console.WriteLine("Development by omidRR");
        while (true)
        {
           
            Console.Write("Enter authorization token: ");
            string token = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token cannot be empty. Please enter a valid authorization token.");
                continue;
            }

            // Check if the token starts with "Bearer " and add it if not
            if (!token.StartsWith("Bearer "))
            {
                token = "Bearer " + token;
            }

            return token;
        }
    }

    static async Task<string> MakeRequestsAsync(string authorizationToken, int points, int iteration)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.0.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("authorization", authorizationToken);
            client.DefaultRequestHeaders.Add("origin", "https://telegram.blum.codes");
            client.DefaultRequestHeaders.Add("priority", "u=1, i");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"125\", \"Chromium\";v=\"125\", \"Not.A/Brand\";v=\"24\", \"Microsoft Edge WebView2\";v=\"125\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");

            // First POST request
            HttpResponseMessage response = await client.PostAsync("https://game-domain.blum.codes/api/v1/game/play", null);

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Number of tokens has expired. Please enter a valid authorization token.");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorMessage}");
                }
                return $"Iteration {iteration} failed.";
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response from /game/play (Iteration {iteration}):");
            Console.WriteLine(responseContent);

            // Parse gameId from the response
            var json = JObject.Parse(responseContent);
            string gameId = json["gameId"].ToString();

            // Wait for 32 seconds
            Console.WriteLine($"Iteration {iteration}: Wait for 32 seconds...");
            await Task.Delay(32000);

            // Second POST request
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.0.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("authorization", authorizationToken);
            client.DefaultRequestHeaders.Add("origin", "https://telegram.blum.codes");
            client.DefaultRequestHeaders.Add("priority", "u=1, i");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"125\", \"Chromium\";v=\"125\", \"Not.A/Brand\";v=\"24\", \"Microsoft Edge WebView2\";v=\"125\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");

            var payload = new
            {
                gameId = gameId,
                points = points
            };

            StringContent content = new StringContent(JObject.FromObject(payload).ToString(), Encoding.UTF8, "application/json");

            // Sending the request with content
            response = await client.PostAsync("https://game-domain.blum.codes/api/v1/game/claim", content);
            responseContent = await response.Content.ReadAsStringAsync();

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error from /game/claim (Iteration {iteration}): {errorMessage}");
                return $"Iteration {iteration} failed.";
            }

            Console.WriteLine($"Response from /game/claim (Iteration {iteration}):");
            Console.WriteLine(responseContent);

            return $"Iteration {iteration} completed successfully.";
        }
    }
}
