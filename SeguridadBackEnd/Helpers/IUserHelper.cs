using Microsoft.AspNetCore.Identity;
using SeguridadBackEnd.Data.Entities;
using SeguridadBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguridadBackEnd.Helpers
{
    public interface IUserHelper
    {
        Task<UserEntity> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(UserEntity user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(UserEntity user, string roleName);

        Task<bool> IsUserInRoleAsync(UserEntity user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<bool> DeleteUserAsync(string email);

        Task<IdentityResult> UpdateUserAsync(UserEntity user);

        Task<SignInResult> ValidatePasswordAsync(UserEntity user, string password);

        Task<UserEntity> AddUser(AddUserViewModel view, string role);

        Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);

        Task<string> GenerateEmailConfirmationTokenAsync(UserEntity user);

        Task<IdentityResult> ConfirmEmailAsync(UserEntity user, string token);

        Task<UserEntity> GetUserByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(UserEntity user);

        Task<IdentityResult> ResetPasswordAsync(UserEntity user, string token, string password);
    }
}
