using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class TestLineApiClient(HttpClient httpClient): IApiClient<TestLine>
{
    public async Task<IEnumerable<TestLine>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<TestLine[]>("/api/v1/crud/customers/", cancellationToken);
        return response ?? Array.Empty<TestLine>();
    }

    public Task<bool> CreateAsync(TestLine testLine, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}