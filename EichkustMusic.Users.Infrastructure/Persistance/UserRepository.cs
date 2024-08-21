using EichkustMusic.S3;
using EichkustMusic.Users.Application.UserRepository;
using EichkustMusic.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace EichkustMusic.Users.Infrastructure.Persistance
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IS3Storage _s3;

        public UserRepository(
            UsersDbContext context, UserManager<ApplicationUser> userManager, IS3Storage s3)
        {
            _context = context;
            _userManager = userManager;
            _s3 = s3;
        }

        public void AddSubscription(ApplicationUser subscriber, ApplicationUser publisher)
        {
            var subscription = new PublisherSubscriber()
            {
                Subscriber = subscriber,
                Publisher = publisher
            };

            _context.Add(subscription);
        }

        public async Task ApplyPatchDocumentAsyncTo(ApplicationUser user, JsonPatchDocument patchDocument)
        {
            const string picturePath = "/pictureurl";

            var s3Operations = patchDocument.Operations.Where(
                operation => operation.path.Equals(picturePath, StringComparison.CurrentCultureIgnoreCase));

            // Check for S3 operation in document
            foreach (var operation in s3Operations)
            {
                var file = (string) operation.value;

                // Check if new file exists
                if (!await _s3.DoesFileExistAsync(file))
                {
                    throw new Exception("New picture file doesn't exist");
                }

                if (user.PictureUrl != null)
                {
                    // Delete old file from S3
                    await _s3.DeleteFileAsync(user.PictureUrl);
                } 
            }

            patchDocument.ApplyTo(user);
        }

        public async Task<bool> DeleteSubscriptionAsync(ApplicationUser subscriber, ApplicationUser publisher)
        {
            // Get subscription
            var subscription = await _context.PublisherSubscribers
                .FirstOrDefaultAsync(publisherSubscriber =>
                    publisherSubscriber.Subscriber == subscriber
                    && publisherSubscriber.Publisher == publisher);

            // Remove subscription
            if (subscription == null)
            {
                return false;
            }

            _context.Remove(subscription);

            return true;
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            // Delete user picture
            if (user.PictureUrl != null)
            {
                await _s3.DeleteFileAsync(user.PictureUrl);
            }

            // Delete user entity
            await _userManager.DeleteAsync(user);
        }

        public Task<ApplicationUser?> GetUserByIdAsync(int id)
        {
            return _userManager.Users
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<ICollection<ApplicationUser>> ListUsersAsync(int pageNum, int pageSize, string? query)
        {
            // Get users list
            var users = _userManager.Users;

            // Search users by query by description, username and display name
            if (query != null)
            {
                users = users.Where(user =>

                    user.Desciption != null
                        && user.Desciption.Contains(query, StringComparison.CurrentCultureIgnoreCase)

                    || user.UserName != null
                        && user.UserName.Contains(query, StringComparison.CurrentCultureIgnoreCase)

                    || user.DisplayName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            // Implement pagination
            users = users
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize);

            // Short descriptions
            users = users
                .Select(user => new ApplicationUser()
                {
                    #region Select other properties
                    Id = user.Id,
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    NormalizedUserName = user.NormalizedUserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    EmailConfirmed = user.EmailConfirmed,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    SubscriptionsM2M = user.SubscriptionsM2M,
                    SubsribersM2M = user.SubsribersM2M,
                    PictureUrl = user.PictureUrl,
                    #endregion

                    Desciption = 
                        user.Desciption != null
                        ? user.Desciption.Length <= 256

                        // If user has a description and its length is less or equal 256
                        ? user.Desciption

                        // If user has a description and its length is greater than 256
                        : user.Desciption.Substring(0, 256) + "..."

                        // If user hasn't description
                        : null
                });

            return await users.ToListAsync();
        }

        public async Task<IEnumerable<IdentityError>?> RegisterUserAsync(ApplicationUser user, string password)
        {
            var identityResult = await _userManager.CreateAsync(user, password);

            if (!identityResult.Succeeded)
            {
                return identityResult.Errors;
            }

            return null;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
