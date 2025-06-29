namespace PW2_Gruppo3.Web.Clients;

public interface IApiClient<T>
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(T item, CancellationToken cancellationToken = default);
}