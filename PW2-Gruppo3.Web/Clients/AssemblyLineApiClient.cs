using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;


public class AssemblyLineApiClient(HttpClient httpClient): IApiClient<AssemblyLine>
{
    public async Task<IEnumerable<AssemblyLine>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<AssemblyLine[]>("/api/v1/crud/customers/", cancellationToken);
        return response ?? Array.Empty<AssemblyLine>();
    }

    public Task<bool> CreateAsync(AssemblyLine assemblyLine, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}