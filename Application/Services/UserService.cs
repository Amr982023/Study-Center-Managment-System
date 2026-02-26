using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<User>> CreateAsync(
            string firstName, string lastName, string phone, string gender,
            string userName, string email, string permission, string hashedPassword,
            string? midName = null)
        {
            if (await _uow.Users.IsUserNameTakenAsync(userName))
                return Result<User>.Failure("Username is already taken.");

            if (await _uow.Users.IsEmailTakenAsync(email))
                return Result<User>.Failure("Email is already in use.");

            if (await _uow.Users.AnyAsync(u => u.PersonalPhone == phone))
                return Result<User>.Failure("Phone already in use by a user.");

            if (await _uow.Students.AnyAsync(s => s.PersonalPhone == phone))
                return Result<User>.Failure("Phone already in use by a student.");

            var result = User.Create(firstName, lastName, phone, gender,
                                     userName, email, permission, hashedPassword, midName);
            if (!result.IsSuccess)
                return Result<User>.Failure(result.ErrorMessage!);

            await _uow.Users.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<User>.Success(result.Value!);
        }

        public async Task<Result<User>> GetByIdAsync(int id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            return user is null
                ? Result<User>.Failure("User not found.")
                : Result<User>.Success(user);
        }

        public async Task<Result<User>> GetWithPersonAsync(int id)
        {
            var user = await _uow.Users.GetWithPersonAsync(id);
            return user is null
                ? Result<User>.Failure("User not found.")
                : Result<User>.Success(user);
        }

        public async Task<Result<User>> GetByEmailAsync(string email)
        {
            var user = await _uow.Users.GetByEmailAsync(email);
            return user is null
                ? Result<User>.Failure("User not found.")
                : Result<User>.Success(user);
        }

        public async Task<Result<User>> GetByUserNameAsync(string userName)
        {
            var user = await _uow.Users.GetByUserNameAsync(userName);
            return user is null
                ? Result<User>.Failure("User not found.")
                : Result<User>.Success(user);
        }

        public async Task<Result<User>> GetByPhoneAsync(string phone)
        {
            var user = await _uow.Users.FirstOrDefaultAsync(u => u.PersonalPhone == phone);
            return user is null
                ? Result<User>.Failure("No user found with this phone number.")
                : Result<User>.Success(user);
        }

        public async Task<Result<IEnumerable<User>>> GetAllAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User>> AuthenticateAsync(string userName, string hashedPassword)
        {
            var user = await _uow.Users.GetByUserNameAsync(userName);
            if (user is null)
                return Result<User>.Failure("Invalid username or password.");

            if (user.HashedPassword != hashedPassword)
                return Result<User>.Failure("Invalid username or password.");

            return Result<User>.Success(user);
        }

        public async Task<Result<bool>> UpdatePermissionAsync(int userId, string permission)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null)
                return Result<bool>.Failure("User not found.");

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null)
                return Result<bool>.Failure("User not found.");

            await _uow.Users.DeleteAsync(user);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
