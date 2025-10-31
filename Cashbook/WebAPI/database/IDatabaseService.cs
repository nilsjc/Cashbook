using WebAPI.endpoints;

namespace WebAPI.database
{
    public interface IDatabaseService
    {
        Task<ServiceResult<string>> CreateAccountAsync(string name, AccountType type);
    }

    public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> Fail(string error) => new() { Success = false, Error = error };
}
}