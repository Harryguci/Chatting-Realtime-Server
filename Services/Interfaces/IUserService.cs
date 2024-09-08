using ChatingApp.Models;

namespace ChatingApp.Services.Interfaces
{
    public interface IUserService
    {
        AuthenticateResponse? Authenticate(AuthenticateRequest model);
        IEnumerable<Account> GetAll();
        Account? GetById(Guid id);
    }
}
