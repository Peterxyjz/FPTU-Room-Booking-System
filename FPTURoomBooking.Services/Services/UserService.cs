using FPTURoomBooking.Repositories.Models;
using FPTURoomBooking.Repositories.Repositories;
using FPTURoomBooking.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTURoomBooking.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _unitOfWork.Repository<User>()
                .SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Repository<User>().GetAllAsync();
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            // Thêm business logic như validate, hash password, etc.
            _unitOfWork.Repository<User>().Add(user);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            // Thêm business logic
            _unitOfWork.Repository<User>().Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            _unitOfWork.Repository<User>().Remove(user);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            user.IsActive = true;
            _unitOfWork.Repository<User>().Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            _unitOfWork.Repository<User>().Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }
}
