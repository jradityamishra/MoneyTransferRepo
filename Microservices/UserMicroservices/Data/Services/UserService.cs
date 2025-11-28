
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UserMicroservices.Data;
using UserMicroservices.Data.Model.Entity;
using UserMicroservices.Data.ViewModel.Authentication;
using UserMicroservices.Data.ViewModel.UserVM;

using UserMicroservices.Data.ViewModel.UserVM;
namespace Banking.Data.Services
{
    public class UserService
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(DatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        private async Task<UserMicroservices.Data.ViewModel.UserVM.UserDto> ToDtoAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserMicroservices.Data.ViewModel.UserVM.UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Status = user.Status.ToString(),
                Roles = roles
            };
        }
        public async Task<IEnumerable<UserMicroservices.Data.ViewModel.UserVM.UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            var list = new List<UserMicroservices.Data.ViewModel.UserVM.UserDto>();
            foreach (var u in users)
                list.Add(await ToDtoAsync(u));
            return list;
        }
        public async Task<UserMicroservices.Data.ViewModel.UserVM.UserDto> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return await ToDtoAsync(user);
        }
        public async Task<UserMicroservices.Data.ViewModel.UserVM.UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return await ToDtoAsync(user);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, UserMicroservices.Data.ViewModel.UserVM.UserDto User)> CreateUserAsync(UserMicroservices.Data.ViewModel.UserVM.CreateUserVM model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName,
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.Active
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description), null);

            // assign roles if provided
            if (model.Roles != null && model.Roles.Any())
            {
                foreach (var r in model.Roles.Distinct())
                {
                    if (!await _roleManager.RoleExistsAsync(r))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(r));
                    }
                }
                await _userManager.AddToRolesAsync(user, model.Roles.Distinct());
            }

            var dto = await ToDtoAsync(user);
            return (true, Array.Empty<string>(), dto);
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(string id, UserMicroservices.Data.ViewModel.UserVM.UpdateUserVM model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return (false, new[] { "User not found" });

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                    return (false, setEmailResult.Errors.Select(e => e.Description).ToArray());
            }

            if (!string.IsNullOrEmpty(model.UserName) && model.UserName != user.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded)
                    return (false, setUserNameResult.Errors.Select(e => e.Description).ToArray());
            }

            if (model.IsActive.HasValue)
                user.Status = model.IsActive.Value ? UserStatus.Active : UserStatus.Inactive;

            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(UserMicroservices.Data.ViewModel.UserVM.ChangePasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(UserMicroservices.Data.ViewModel.UserVM.ResetPasswordVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Succeeded, string[] Errors)> AssignRoleAsync(UserMicroservices.Data.ViewModel.UserVM.AssignRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return (false, new[] { "User not found" });

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Succeeded, string[] Errors)> RemoveRoleAsync(UserMicroservices.Data.ViewModel.UserVM.AssignRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, Array.Empty<string>());
        }



    }
}
