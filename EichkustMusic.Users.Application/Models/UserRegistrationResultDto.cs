using EichkustMusic.Users.Domain.Entities;

namespace EichkustMusic.Users.Application.Models
{
    public class UserRegistrationResultDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Description { get; set; }

        public bool EmailConfirmed { get; set; }

        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UrlToUploadPicture { get; set; } = null!;

        public static UserRegistrationResultDto MapFromApplicationUser(ApplicationUser user)
        {
            return new UserRegistrationResultDto()
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Description = user.Desciption,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber!,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
            };
        }
    }
}
