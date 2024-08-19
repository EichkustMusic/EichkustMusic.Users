using EichkustMusic.Users.Domain.Entities;

namespace EichkustMusic.Users.Application.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string? Description { get; set; }

        public string DisplayName { get; set; } = null!;

        public int SubscriptionsCount { get; set; }

        public int SubscribersCount { get; set; }

        public static UserDto MapFromApplicationUser(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Description = user.Desciption,
                DisplayName = user.DisplayName,
                SubscriptionsCount = user.SubscriptionsCount,
                SubscribersCount = user.SubscribersCount,
            };
        }
    }
}
