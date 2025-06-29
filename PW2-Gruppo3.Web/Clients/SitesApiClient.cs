using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class SitesApiClient(HttpClient httpClient): IApiClient<Site>
{
    public async Task<IEnumerable<Site>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<Site[]>("/api/v1/crud/sites/", cancellationToken);
        return response ?? Array.Empty<Site>();
    }

    public async Task<bool> CreateAsync(Site site, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/crud/sites/", site, cancellationToken);
        return response.IsSuccessStatusCode;
    }
    
}