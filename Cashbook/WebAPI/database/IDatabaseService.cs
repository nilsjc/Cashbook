using WebAPI.endpoints;

namespace WebAPI.database
{
    public interface IDatabaseService
    {
        Task CreateAccountAsync(string name, AccountType type);
    }
}