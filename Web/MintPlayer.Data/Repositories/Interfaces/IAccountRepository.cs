using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Tuple<User, string>> Register(User user, string password);
        Task<LoginResult> LocalLogin(string email, string password, bool createCookie);
        Task<IEnumerable<AuthenticationScheme>> GetExternalLoginProviders();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<LoginResult> PerfromExternalLogin();
        Task<IEnumerable<UserLoginInfo>> GetExternalLogins(ClaimsPrincipal userProperty);
        Task AddExternalLogin(ClaimsPrincipal userProperty);
        Task RemoveExternalLogin(ClaimsPrincipal userProperty, string provider);
        Task<User> GetCurrentUser(ClaimsPrincipal userProperty);
        Task Logout();
    }
}
