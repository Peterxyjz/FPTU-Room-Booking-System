using FPTURoomBooking.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTURoomBooking.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
    }
}
