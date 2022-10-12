using System;
using HomeSearch.Shared.Models;

namespace HomeSearch.Client.Repositories
{
    public interface IAuthService
    {
        Task Login(LoginRequest loginRequest);
        Task Register(RegisterRequest registerRequest);
        Task Logout();
        Task<CurrentUser> CurrentUserInfo();
    }
}

