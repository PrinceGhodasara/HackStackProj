using DocumentFormat.OpenXml.Wordprocessing;
using HackStack___Gemini.Models;
using HackStack___Gemini.Prompts;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using RestSharp;


namespace HackStack___Gemini.Services
{
    public class GeminiService
    {
        private readonly string _apiKey = "";
        private readonly string _geminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        public async Task<ApiResult> GetRecommendationsAsync(ExcelData data, string query)
        {

            var prompt = PromptsService.GetPrompt(data, string.IsNullOrEmpty(query) ? "Dashboard" : "query", query);
            var client = new RestClient($"{_geminiUrl}?key={_apiKey}");

            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                    }
                }
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Request failed:");
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Error Message: {response.ErrorMessage}");
                Console.WriteLine($"Raw Content: {response.Content}");

                return ApiErrors.GeminiError;
            }

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                Console.WriteLine("Empty response content received.");
                return ApiErrors.NoResFromGemini;
            }

            try
            {
                var result = JsonConvert.DeserializeObject<GeminiResponse>(response.Content);
                var innerJson = result?.candidates?[0]?.content?.parts?[0]?.text;

                if (innerJson.StartsWith("```html"))
                {
                    innerJson = innerJson.Replace("```html", "").Replace("```", "").Trim();
                }
                // Second layer: Parse that string into your model
                var finalData = innerJson;

                // Now you can return finalData to your frontend
                var res = new ApiResult(true, null, finalData);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse JSON:");
                Console.WriteLine(ex.Message);
                return ApiErrors.ParseError;
            }
        }
    }
}
