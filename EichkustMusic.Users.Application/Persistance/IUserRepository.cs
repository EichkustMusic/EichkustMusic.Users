using EichkustMusic.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace EichkustMusic.Users.Application.UserRepository
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByIdAsync(int id);

        Task<IEnumerable<IdentityError>?> RegisterUserAsync(ApplicationUser user, string password);

        // Description must be shortened!
        Task<ICollection<ApplicationUser>> ListUsersAsync(
            int pageNum, int pageSize, string? query);  

        // Document must be checked if it contains paths to S3 files. If it contains, patch should be rejected
        Task ApplyPatchDocumentAsyncTo(ApplicationUser user, JsonPatchDocument patchDocument);

        // Files must be deleted from S3
        Task DeleteUserAsync(ApplicationUser user);  

        Task<bool> AddSubscriptionAsync(ApplicationUser subscriber, ApplicationUser publisher);

        Task<bool> DeleteSubscriptionAsync(ApplicationUser subscriber, ApplicationUser publisher);
        Task SaveChangesAsync();
    }
}
