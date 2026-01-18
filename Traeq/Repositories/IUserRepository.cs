using Microsoft.AspNetCore.Identity;
using Traeq.Models;
using Traeq.DTO;

namespace Traeq.Repositories
{
    public interface IUserRepository<T>
    {
        Task<IdentityResult> RegisterAsync(RegisterDTO model);
        Task<SignInResult> LoginAsync(LoginDTO model);
        Task LogoutAsync();
        Task UpdateAsync(string id, UserDTO model, string currentUserId);
        Task DeleteAsync(string id, string currentUserId);
        Task ToggleActiveAsync(string id, string currentUserId);
        List<UserDTO> GetAllUsers();
        UserDTO GetUserById(string id);
        UserDTO GetUserByUsername(string username);
        List<UserDTO> GetUsersByAccountType(string accountType);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<User> FindUserByIdAsync(string id);
        Task<User> FindUserByUsernameAsync(string username);
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task AddUserToRoleAsync(string userId, string role);
        Task RemoveUserFromRoleAsync(string userId, string role);
    }
}