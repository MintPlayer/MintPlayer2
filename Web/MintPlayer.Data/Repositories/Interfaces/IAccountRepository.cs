using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Tuple<User, string>> Register(User user, string password);
        Task<LoginResult> LocalLogin(string email, string password, bool createCookie);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<LoginResult> PerfromExternalLogin(); 
        Task<User> GetCurrentUser(ClaimsPrincipal userProperty);
        Task Logout();
    }
}
