
using System;
using System.Collections.Generic; // Required for Dictionary
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class Program
{
    private const string ApiUrl = "https://mempool.space/api/v1/prices";

    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        try
        {
            var rate = await GetBtcToCurrencyRate(httpClient, "USD");
            Console.WriteLine($"BTC to USD rate: {rate}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task<decimal> GetBtcToCurrencyRate(HttpClient httpClient, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code must not be null or empty.", nameof(currencyCode));

        // Fetch the latest rate from API
        var latestRate = await FetchRateFromApi(httpClient, currencyCode);
        return latestRate;
    }

    private static async Task<decimal> FetchRateFromApi(HttpClient httpClient, string currencyCode)
    {
        try
        {
            var response = await httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {jsonString}");

            var data = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonString);
            if (data != null && data.TryGetValue(currencyCode, out var rate))
            {
                return rate;
            }

            throw new KeyNotFoundException($"Currency '{currencyCode}' not found in the API response.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Error while fetching data from API.");
            throw new Exception("Failed to fetch BTC rate from the API.", ex);
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Error parsing the API response.");
            throw new Exception("Invalid response format from the API.", ex);
        }
    }
}
