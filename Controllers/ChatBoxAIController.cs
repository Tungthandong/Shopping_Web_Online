using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shopping_Web.Models; // Model "GenerateImageRequest" của bạn
using System;
using System.Collections.Generic; // Thêm
using System.IO;
using System.Linq; // Thêm
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shopping_Web.Controllers
{
    public class TranslationResponse
    {
        [JsonPropertyName("translation_text")]
        public string TranslationText { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxAIController : ControllerBase
    {
        private const string IMAGE_API_URL = "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-xl-base-1.0";
        private const string TRANSLATE_API_URL = "https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-vi-en";
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string _apiToken;

        public ChatBoxAIController(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _hostEnvironment = hostEnvironment;
            _apiToken = configuration["HuggingFaceApiToken"];
        }


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request)
        {
            if (string.IsNullOrEmpty(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required." });
            }

            try
            {
                Console.WriteLine($"Received Viet prompt: {request.Prompt}");
                string englishPrompt = await TranslateVietnameseToEnglish(request.Prompt);

                if (string.IsNullOrEmpty(englishPrompt))
                {
                    return StatusCode(500, new { error = "Không thể dịch prompt sang Tiếng Anh." });
                }
                Console.WriteLine($"Translated English prompt: {englishPrompt}");

                string fullPrompt = $"A high-quality, centered vector design for a t-shirt print, minimalist style, " +
                                    $"on a solid white background, 8k, sharp details: {englishPrompt}";

                var imagePayload = new { inputs = fullPrompt };
                var imageJsonPayload = JsonSerializer.Serialize(imagePayload);

                var imageRequest = new HttpRequestMessage(HttpMethod.Post, IMAGE_API_URL);
                imageRequest.Content = new StringContent(imageJsonPayload, Encoding.UTF8, "application/json");
                imageRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

                Console.WriteLine("Calling Image Generation API...");
                HttpResponseMessage imageResponse = await _httpClient.SendAsync(imageRequest);

                if (!imageResponse.IsSuccessStatusCode)
                {
                    string errorContent = await imageResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Image API Error: {errorContent}");
                    if (errorContent.Contains("model is loading"))
                    {
                        return StatusCode(503, new { error = "AI tạo ảnh đang khởi động (cold start). Vui lòng thử lại sau 1-2 phút." });
                    }
                    return StatusCode((int)imageResponse.StatusCode, new { error = $"Lỗi API tạo ảnh: {errorContent}" });
                }
                byte[] imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();

                string uniqueFileName = $"{Guid.NewGuid()}.jpg";
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "generated-images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                string imageUrl = $"/generated-images/{uniqueFileName}";
                Console.WriteLine($"Image saved to: {imageUrl}");
                return Ok(new { imageUrl = imageUrl });
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("API timed out (cold start?).");
                return StatusCode(504, new { error = "Yêu cầu mất quá nhiều thời gian (AI khởi động quá 2 phút). Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating image: {ex.Message}");
                return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
            }
        }

        private async Task<string> TranslateVietnameseToEnglish(string vietnameseText)
        {
            try
            {
                var payload = new { inputs = vietnameseText };
                var jsonPayload = JsonSerializer.Serialize(payload);

                var translateRequest = new HttpRequestMessage(HttpMethod.Post, TRANSLATE_API_URL);
                translateRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                translateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

                Console.WriteLine("Calling Translation API...");
                HttpResponseMessage response = await _httpClient.SendAsync(translateRequest);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Translate API Error: {errorContent}");
                    if (errorContent.Contains("model is loading"))
                    {
                        throw new Exception("AI dịch đang khởi động, vui lòng thử lại.");
                    }
                    return null;
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var translationResult = JsonSerializer.Deserialize<List<TranslationResponse>>(jsonResponse);

                if (translationResult != null && translationResult.Count > 0)
                {
                    return translationResult.First().TranslationText;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Translation Error: {ex.Message}");
                return null;
            }
        }
    }
}