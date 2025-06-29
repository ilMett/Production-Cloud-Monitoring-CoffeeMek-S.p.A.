using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class LatheApiClient(HttpClient httpClient): IApiClient<Lathe>
{
    public async Task<IEnumerable<Lathe>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<Lathe[]>("/api/v1/crud/customers/", cancellationToken);
        return response ?? Array.Empty<Lathe>();
    }

    public Task<bool> CreateAsync(Lathe lathe, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}