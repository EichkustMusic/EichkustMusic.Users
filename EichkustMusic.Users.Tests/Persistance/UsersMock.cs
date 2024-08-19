using EichkustMusic.Users.Domain.Entities;

namespace EichkustMusic.Users.Tests.Persistance
{
    public static class UsersMock
    {
        public static List<ApplicationUser> Users
            = new List<ApplicationUser>
        {
            new()
            {
                Id = 1,
                PictureUrl = "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/01.jpg",
                Desciption = "test description 1",
                DisplayName = "first user",
                UserName = "firstuser",
                NormalizedUserName = "FIRSTUSER",
                Email = "first@user.com",
                NormalizedEmail = "FIRST@USER.COM",
                EmailConfirmed = true,
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                SecurityStamp = null,
                ConcurrencyStamp = null,
                PhoneNumber = null,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            },
            new()
            {
                Id = 2,
                PictureUrl = "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/02.jpg",
                Desciption = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem",
                DisplayName = "second user",
                UserName = "seconduser",
                NormalizedUserName = "SECONDUSER",
                Email = "second@user.com",
                NormalizedEmail = "SECOND@USER.COM",
                EmailConfirmed = true,
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                SecurityStamp = null,
                ConcurrencyStamp = null,
                PhoneNumber = null,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            },
        };
    }
}
