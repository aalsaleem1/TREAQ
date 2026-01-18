using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.Models;
using Traeq.DTO;

namespace Traeq.Repositories
{
    public class UserRepository : IUserRepository<User>
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(
            AppDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
        {
            var user = new User
            {
                FullName = model.FullName,

                UserName = model.Username,

                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                District = model.District,
                AccountType = model.AccountType,
                ImageUrl = model.ImageUrl,

                IsActive = model.AccountType != "Pharmacy",
                IsDelete = false,
                CreateDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await CreateRolesIfNotExist();

                string role = GetRoleByAccountType(model.AccountType);
                await _userManager.AddToRoleAsync(user, role);

                if (model.AccountType == "Pharmacy")
                {
                    var pharmacyInfo = new PharmacyLegalInfo
                    {
                        UserId = user.Id,
                        PharmacyName = model.PharmacyName,

                        LicenseNumber = model.LicenseNumber,

                        OwnerName = model.OwnerName,
                        PharmacyLogoUrl = model.PharmacyLogoUrl,
                        CreateDate = DateTime.Now,
                        IsActive = false, 
                        IsDelete = false
                    };

                    _context.PharmacyLegalInfos.Add(pharmacyInfo);
                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username) ??
                       await _userManager.FindByEmailAsync(model.Username);

            if (user == null || !user.IsActive || user.IsDelete)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task UpdateAsync(string id, UserDTO model, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("المستخدم غير موجود");

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.District = model.District;
            user.AccountType = model.AccountType;
            user.IsActive = model.IsActive;
            user.EditId = currentUserId;
            user.EditDate = DateTime.Now;
            if (!string.IsNullOrEmpty(model.ImageUrl))
                user.ImageUrl = model.ImageUrl;

            await _userManager.UpdateAsync(user);

            if (model.AccountType == "Pharmacy")
            {
                var pharmacyInfo = await _context.PharmacyLegalInfos
                    .FirstOrDefaultAsync(p => p.UserId == id);

                if (pharmacyInfo != null)
                {
                    pharmacyInfo.PharmacyName = model.PharmacyName;
                    pharmacyInfo.LicenseNumber = model.PharmacyLicense;
                    pharmacyInfo.OwnerName = model.OwnerName;
                    if (!string.IsNullOrEmpty(model.PharmacyLogoUrl))
                        pharmacyInfo.PharmacyLogoUrl = model.PharmacyLogoUrl;
                    pharmacyInfo.EditDate = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newPharmacyInfo = new PharmacyLegalInfo
                    {
                        UserId = id,
                        PharmacyName = model.PharmacyName,
                        LicenseNumber = model.PharmacyLicense,
                        OwnerName = model.OwnerName,
                        PharmacyLogoUrl = model.PharmacyLogoUrl,
                        CreateDate = DateTime.Now,
                        IsActive = user.IsActive,
                        IsDelete = false,
                    };

                    _context.PharmacyLegalInfos.Add(newPharmacyInfo);
                    await _context.SaveChangesAsync();
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            string newRole = GetRoleByAccountType(model.AccountType);
            await _userManager.AddToRoleAsync(user, newRole);
        }

        public async Task DeleteAsync(string id, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsDelete = true;
                user.EditId = currentUserId;
                user.EditDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                var pharmacyInfo = await _context.PharmacyLegalInfos
                    .FirstOrDefaultAsync(p => p.UserId == id);

                if (pharmacyInfo != null)
                {
                    pharmacyInfo.IsDelete = true;
                    pharmacyInfo.EditDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ToggleActiveAsync(string id, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                user.EditId = currentUserId;
                user.EditDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                var pharmacyInfo = await _context.PharmacyLegalInfos
                    .FirstOrDefaultAsync(p => p.UserId == id);

                if (pharmacyInfo != null)
                {
                    pharmacyInfo.IsActive = user.IsActive;
                    pharmacyInfo.EditDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public List<UserDTO> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.PharmacyLegalInfo)
                .Where(u => !u.IsDelete)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    City = u.City,
                    District = u.District,
                    AccountType = u.AccountType,
                    IsActive = u.IsActive,
                    ImageUrl = u.ImageUrl,
                    PharmacyName = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().PharmacyName : null,
                    PharmacyLicense = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().LicenseNumber : null,
                    OwnerName = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().OwnerName : null,
                    PharmacyLogoUrl = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().PharmacyLogoUrl : null
                })
                .ToList();
        }

        public UserDTO GetUserById(string id)
        {
            var user = _context.Users
                .Include(u => u.PharmacyLegalInfo)
                .FirstOrDefault(u => u.Id == id && !u.IsDelete);

            if (user == null) return null;

            var pharmacy = user.PharmacyLegalInfo?.FirstOrDefault();

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                District = user.District,
                AccountType = user.AccountType,
                IsActive = user.IsActive,
                ImageUrl = user.ImageUrl,

                PharmacyName = pharmacy?.PharmacyName,
                PharmacyLicense = pharmacy?.LicenseNumber,
                OwnerName = pharmacy?.OwnerName,
                PharmacyLogoUrl = pharmacy?.PharmacyLogoUrl
            };
        }

        public UserDTO GetUserByUsername(string username)
        {
            var user = _context.Users
                .Include(u => u.PharmacyLegalInfo)
                .FirstOrDefault(u => (u.UserName == username || u.Email == username) && !u.IsDelete);

            if (user == null) return null;

            var pharmacy = user.PharmacyLegalInfo?.FirstOrDefault();

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                District = user.District,
                AccountType = user.AccountType,
                IsActive = user.IsActive,
                PharmacyName = pharmacy?.PharmacyName,
                PharmacyLicense = pharmacy?.LicenseNumber,
                OwnerName = pharmacy?.OwnerName,
                PharmacyLogoUrl = pharmacy?.PharmacyLogoUrl
            };
        }

        public List<UserDTO> GetUsersByAccountType(string accountType)
        {
            return _context.Users
                .Include(u => u.PharmacyLegalInfo)
                .Where(u => u.AccountType == accountType && !u.IsDelete)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    City = u.City,
                    District = u.District,
                    AccountType = u.AccountType,
                    IsActive = u.IsActive,
                    PharmacyName = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().PharmacyName : null,
                    PharmacyLicense = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().LicenseNumber : null,
                    OwnerName = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().OwnerName : null,
                    PharmacyLogoUrl = u.PharmacyLegalInfo != null ? u.PharmacyLegalInfo.FirstOrDefault().PharmacyLogoUrl : null
                })
                .ToList();
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<User> FindUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> FindUserByUsernameAsync(string username)
        {
            var data = await _userManager.FindByEmailAsync(username);
            if (data == null)
                data = await _userManager.FindByNameAsync(username);
            return data;
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task AddUserToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task RemoveUserFromRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        private async Task CreateRolesIfNotExist()
        {
            string[] roles = { "Admin", "Pharmacy", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private string GetRoleByAccountType(string accountType)
        {
            return accountType switch
            {
                "Admin" => "Admin",
                "Pharmacy" => "Pharmacy",
                _ => "User"
            };
        }
    }
}