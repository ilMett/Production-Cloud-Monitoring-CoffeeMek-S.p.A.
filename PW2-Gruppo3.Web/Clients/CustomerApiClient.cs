using PW2_Gruppo3.Models;

namespace PW2_Gruppo3.Web.Clients;

public class CustomerApiClient(HttpClient httpClient): IApiClient<Customer>
{
    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
     {
         var response = await httpClient.GetFromJsonAsync<Customer[]>("/api/v1/crud/customers/", cancellationToken);
         return response ?? Array.Empty<Customer>();
     }

    public async Task<bool> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/crud/customers/", customer, cancellationToken);
        return response.IsSuccessStatusCode;
    }


    
}