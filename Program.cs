using Newtonsoft.Json.Linq;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        string authorizationToken = GetAuthorizationToken();
        int maxRetries = 12;
        int retryDelay = 2000;
        int attempt = 0;
        while (true)
        {
            try
            {
                try
                {
                    Console.WriteLine("W8 To get Balance Value");
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0");
                        client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                        client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
                        client.DefaultRequestHeaders.Add("authorization", authorizationToken);
                        client.DefaultRequestHeaders.Add("dnt", "1");
                        client.DefaultRequestHeaders.Add("origin", "https://telegram.blum.codes");
                        client.DefaultRequestHeaders.Add("priority", "u=1, i");
                        client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"125\", \"Chromium\";v=\"125\", \"Not.A/Brand\";v=\"24\", \"Microsoft Edge WebView2\";v=\"125\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                        client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                        client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                        client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");

                        HttpResponseMessage response = await client.GetAsync("https://game-domain.blum.codes/api/v1/user/balance");

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            var json = JObject.Parse(responseContent);

                            string availableBalance = json["availableBalance"].ToString();
                            int playPasses = (int)json["playPasses"];

                            Console.WriteLine($"Available Balance: {availableBalance}");
                            Console.WriteLine($"Play Passes: {playPasses}");
                      
                        }
                        else
                        {
                            Console.WriteLine("Failed to fetch balance data.");
                            attempt++;
                            if (attempt < maxRetries)
                            {
                                Console.WriteLine($"Retrying in {retryDelay / 1000} seconds...");
                                await Task.Delay(retryDelay);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");

                    if (attempt < maxRetries)
                    {
                        Console.WriteLine($"Retrying in {retryDelay / 1000} seconds...");
                        await Task.Delay(retryDelay); 
                    }
                    else
                    {
                        Console.WriteLine("Failed to fetch balance data after multiple attempts.");
                        return; 
                    }
                }


                Console.Write(
                    "Enter number of repetitions(Warning: Do not enter a repeat count greater than the number of games!): ");
                if (!int.TryParse(Console.ReadLine(), out int repetitions))
                {
                    Console.WriteLine("Invalid number of repetitions.");
                    continue;
                }

                Console.Write("Enter points (suggested between 250 and 270): ");
                string pointsInput = Console.ReadLine();
                int points;

                if (string.IsNullOrEmpty(pointsInput))
                {
                    Random random = new Random();
                    points = random.Next(250, 270);
                    Console.WriteLine($"No points entered. Using random points: {points}");
                }
                else if (!int.TryParse(pointsInput, out points) || points < 10 || points > 2000)
                {
                    Console.WriteLine("Invalid points value. Please enter a number between 10 and 300.");
                    continue;
                }

                Console.Write("Enter number of requests to process at the same time (batch size | use 1 or max 3): ");
                if (!int.TryParse(Console.ReadLine(), out int batchSize) || batchSize <= 0)
                {
                    Console.WriteLine("Invalid batch size. Please enter a valid number greater than 0.");
                    continue;
                }

                for (int i = 0; i < repetitions; i += batchSize)
                {
                    List<Task<string>> taskBatch = new List<Task<string>>();

                    for (int j = 0; j < batchSize && (i + j) < repetitions; j++)
                    {
                        Random random = new Random();
                        points = random.Next(250, 270); 
                        Console.WriteLine($"Iteration {i + j + 1}: Random points = {points}");
                        Thread.Sleep(3000);
                        taskBatch.Add(MakeRequestsAsync(authorizationToken, points, i + j + 1));
                    }

                    string[] results = await Task.WhenAll(taskBatch);

                    for (int k = 0; k < results.Length; k++)
                    {
                        Console.WriteLine($"Result of iteration {i + k + 1}:\n{results[k]}\n");
                    }

                    Thread.Sleep(1000);
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

            if (!token.StartsWith("Bearer "))
            {
                token = "Bearer " + token;
            }

            return token;
        }
    }

    static async Task<string> MakeRequestsAsync(string authorizationToken, int points, int iteration)
    {
        int maxRetries = 4;
        int retryDelay = 2000;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                Thread.Sleep(500);
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

                    HttpResponseMessage response = await client.PostAsync("https://game-domain.blum.codes/api/v1/game/play", null);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine("Number of tokens has expired. Please enter a valid authorization token.");
                            return $"Iteration {iteration} failed.";
                        }
                        else if (responseContent.Contains("cannot start game"))
                        {
                            Console.WriteLine($"Iteration {iteration}: Cannot start game. Retrying in 13 seconds...");
                            await Task.Delay(10000); 
                            Thread.Sleep(5000);
                            attempt++;
                            continue; 
                        }
                        else
                        {
                            Console.WriteLine($"Error: {responseContent}");
                            return $"Iteration {iteration} failed.";
                        }
                    }

                    Console.WriteLine(responseContent);

                    var json = JObject.Parse(responseContent);
                    string gameId = json["gameId"].ToString();

                    // Wait for 32 seconds
                    Console.WriteLine($"Iteration {iteration}: Wait for 32 seconds...");
                    await Task.Delay(32000);

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

                    response = await client.PostAsync("https://game-domain.blum.codes/api/v1/game/claim", content);
                    responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error from /game/claim (Iteration {iteration}): {responseContent}");
                        return $"Iteration {iteration} failed.";
                    }
                    return $"Response==> {responseContent}";
                }
            }
            catch (HttpRequestException ex)
            {
                attempt++;
                Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");

                if (ex.Message.Contains("An error occurred while sending the request") && attempt < maxRetries)
                {
                    Console.WriteLine($"Retrying in {retryDelay / 1000} seconds...");
                    await Task.Delay(retryDelay);
                }
                else
                {
                    return $"Iteration {iteration} failed after {attempt} attempts.";
                }
            }
        }

        return $"Iteration {iteration} failed after {maxRetries} attempts.";
    }


}
