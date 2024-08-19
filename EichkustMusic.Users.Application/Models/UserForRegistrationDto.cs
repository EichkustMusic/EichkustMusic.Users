using EichkustMusic.Users.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EichkustMusic.Users.Application.Models
{
    public class UserForRegistrationDto
    {
        [Required]
        [MaxLength(32)]
        public string UserName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(1024)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(64)]
        public string DisplayName { get; set; } = null!;

        public ApplicationUser MapToApplicationUser()
        {
            return new ApplicationUser()
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Desciption = Description,
                DisplayName = DisplayName,
            };
        }
    }
}
