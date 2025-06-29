using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class MillingApiClient(HttpClient httpClient): IApiClient<Milling>
{
    public async Task<IEnumerable<Milling>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<Milling[]>("/api/v1/crud/customers/", cancellationToken);
        return response ?? Array.Empty<Milling>();
    }

    public Task<bool> CreateAsync(Milling milling, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}