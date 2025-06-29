using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class BatchApiClient(HttpClient httpClient):IApiClient<Batch>
{
    public async Task<IEnumerable<Batch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<Batch[]>("/api/v1/crud/customers/", cancellationToken);
        return response ?? Array.Empty<Batch>();
    }

    public Task<bool> CreateAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}