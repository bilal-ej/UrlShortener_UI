using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortenerApp.Models;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _apiBaseUrl;

    public IndexModel(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _apiBaseUrl = configuration["UrlShortenerApiBaseUrl"];
    }

    [BindProperty]
    public string LongUrl { get; set; }
    public string ShortenedUrl { get; private set; }
    public string ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/Url", LongUrl);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ShortenedUrlResponse>();
                ShortenedUrl = $"{_apiBaseUrl}/Url/shortUrl/{result.ShortUrl}";
                ErrorMessage = null;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ErrorMessage = errorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        return Page();
    }
}