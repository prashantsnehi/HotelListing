using System;
using System.Threading.Tasks;
using MyWebAPI.Models;

namespace MyWebAPI.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginDTO model);
        Task<string> CreateToken();
    }
}
