using EichkustMusic.S3;
using EichkustMusic.Users.Application.S3;
using EichkustMusic.Users.Domain.Entities;
using EichkustMusic.Users.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics;
using System.Text.Json;

namespace EichkustMusic.Users.Tests.Persistance
{
    public class UserRepositoryTests
    {
        private UsersDbContext DbContext { get; set; }
        private UserRepository UserRepository { get; set; }
        private S3Storage S3 { get; set; }

        public UserRepositoryTests()
        {
            // Configure database context
            var dbContextOptions = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase("TracksDb")
                .Options;

            var dbContext = new UsersDbContext(dbContextOptions);

            DbContext = dbContext;

            // Mock user manager
            var userStore = new Microsoft.AspNetCore.Identity.EntityFrameworkCore
                .UserStore<ApplicationUser, IdentityRole<int>, UsersDbContext, int>(dbContext);

            var options = new Mock<IOptions<IdentityOptions>>();

            var idOptions = new IdentityOptions 
            {
                Lockout = 
                {
                    AllowedForNewUsers = false
                } 
            };

            options
                .Setup(o => o.Value)
                .Returns(idOptions);

            var userValidators = new List<IUserValidator<ApplicationUser>>();

            var validator = new Mock<IUserValidator<ApplicationUser>>();

            userValidators.Add(validator.Object);

            var pwdValidators = new List<PasswordValidator<ApplicationUser>> 
            {
                new()
            };

            var userManager = new UserManager<ApplicationUser>(
                userStore,
                options.Object, 
                new PasswordHasher<ApplicationUser>(), 
                userValidators,
                pwdValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // .env file should be created at bin/Env/net8.0/
            var directory = $"{Directory.GetCurrentDirectory()}" + "/.env";

            Debug.WriteLine(directory);

            DotNetEnv.Env.Load(directory);

            // Get S3 configuration
            var accessKey = Environment.GetEnvironmentVariable("S3_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("S3_SECRET_KEY");
            var serviceUrl = Environment.GetEnvironmentVariable("S3_SERVICE_URL");

            // Mock S3
            var configurationManagerMock = new Mock<IConfigurationManager>();

            configurationManagerMock
                .Setup(cm => cm["S3:AccessKey"])
                .Returns(accessKey);

            configurationManagerMock
                .Setup(cm => cm["S3:SecretKey"])
                .Returns(secretKey);

            configurationManagerMock
                .Setup(cm => cm["S3:ServiceUrl"])
                .Returns(serviceUrl);

            var s3 = new S3Storage(configurationManagerMock.Object);

            S3 = s3;

            // Set up repository
            UserRepository = new UserRepository(dbContext, userManager, s3);
        }

        [SetUp]
        public async Task Setup()
        {
            // Add new users to the repository
            DbContext.Users.AddRange(UsersMock.Users);

            await DbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task Cleanup()
        {
            // Return missing files to the S3
            var firstAvatarImagePath = Environment.GetEnvironmentVariable("S3_01jpg_PATH");
            var secondAvatarImagePath = Environment.GetEnvironmentVariable("S3_02jpg_PATH");

            if (firstAvatarImagePath == null || secondAvatarImagePath == null)
            {
                throw new Exception("Test pictures paths not found");
            }

            if (!await S3.DoesFileExistAsync(
                "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/01.jpg"))
            {
                var isSuccess = await S3.UploadFileAsync(BucketNames.UserAvatars, firstAvatarImagePath, "01.jpg");

                if (isSuccess == false)
                {
                    Console.WriteLine("First file not found and returned");

                    throw new Exception("Cannot upload image");
                }
            }

            if (!await S3.DoesFileExistAsync(
                "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/02.jpg"))
            {
                var isSuccess = await S3.UploadFileAsync(BucketNames.UserAvatars, firstAvatarImagePath, "02.jpg");

                if (isSuccess == false)
                {
                    Console.WriteLine("Second file not found and returned");

                    throw new Exception("Cannot upload image");
                }
            }

            // Clean up database context
            DbContext.Users.RemoveRange(DbContext.Users);
            DbContext.PublisherSubscribers.RemoveRange(DbContext.PublisherSubscribers);
            DbContext.UserTokens.RemoveRange(DbContext.UserTokens);
            DbContext.UserRoles.RemoveRange(DbContext.UserRoles);
            DbContext.UserLogins.RemoveRange(DbContext.UserLogins);
            DbContext.UserClaims.RemoveRange(DbContext.UserClaims);
            DbContext.Roles.RemoveRange(DbContext.Roles);
            DbContext.RoleClaims.RemoveRange(DbContext.RoleClaims);

            await DbContext.SaveChangesAsync(); 
        }

        [Test]
        public async Task UserRespository_DeleteUserAsync_DeletesFileFromS3()
        {
            // Delete user
            var user = await UserRepository.GetUserByIdAsync(1)
                ?? throw new Exception("User not found");

            var pictureUrl = user.PictureUrl!;

            await UserRepository.DeleteUserAsync(user);

            await DbContext.SaveChangesAsync();

            // Assertion
            var actual = await S3.DoesFileExistAsync(pictureUrl);

            Assert.That(actual, Is.False);
        }

        [Test]
        public async Task UserRepository_ListUsersAsync_ReturnsShortenedDescriptionIfLengthIsGreaterThan256()
        {
            // Get users
            var users = await UserRepository.ListUsersAsync(1, 10, null);

            // The user with long description is with ID 2
            var longDescriptionUser = users.FirstOrDefault(user => user.Id == 2)
                ?? throw new Exception("User not found");

            // Assert that consumed description length is 259 (because there are three dots after a text (256+3))
            Assert.That(longDescriptionUser.Desciption, Has.Length.EqualTo(259));
        }

        [Test]
        public async Task UserRepository_ListUsersAsync_IgnoresCase()
        {
            var actual = await UserRepository.ListUsersAsync(1, 10, "first");
            var expected = UsersMock.Users.Where(user => user.Id == 1);

            Assert.That(
                JsonSerializer.Serialize(actual),
                Is.EqualTo(JsonSerializer.Serialize(expected)));
        }

        [Test]
        public async Task UserRepository_ApplyPatchDocumentAsyncTo_DeletesOldFileIfNewFileExists()
        {
            // Upload new image to S3
            var avatarImagePath = Environment.GetEnvironmentVariable("S3_01jpg_PATH");

            if (avatarImagePath == null)
            {
                throw new Exception("Test pictures paths not found");
            }

            var isSuccess = await S3.UploadFileAsync(BucketNames.UserAvatars, avatarImagePath, "03.jpg");

            if (isSuccess == false)
            {
                Console.WriteLine("File not found and returned");

                throw new Exception("Cannot upload image");
            }

            var newPictureUrl = "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/03.jpg";

            // Assert that method deletes old file and set value to new the new file path
            var user = await UserRepository.GetUserByIdAsync(1);

            if (user == null)
            {
                throw new Exception("A user was not found");
            }

            var oldPictureUrl = user.PictureUrl!;

            var patchDocument = new JsonPatchDocument();

            patchDocument.Operations.Add(new Operation
            {
                op = "add",
                path = "/PictureUrl",
                value = newPictureUrl
            });

            await UserRepository.ApplyPatchDocumentAsyncTo(user, patchDocument);

            await UserRepository.SaveChangesAsync();

            Assert.That(user.PictureUrl, Is.EqualTo(newPictureUrl));

            var doesOldPictureExist = await S3.DoesFileExistAsync(oldPictureUrl);

            Assert.That(doesOldPictureExist, Is.False);

            // Delete new image from S3
            var doesNewFileDeletionSuccessful = await S3.DeleteFileAsync(newPictureUrl);

            if (doesNewFileDeletionSuccessful == false)
            {
                throw new Exception("Cannot delete new picture from S3");
            }
        }

        [Test]
        public async Task UserRepository_ApplyPatchDocumentAsyncTo_ThrowsExceptionIfNewFileDoesNotExists()
        {
            // Get user
            var user = await UserRepository.GetUserByIdAsync(1);

            if (user == null)
            {
                throw new Exception("A user was not found");
            }

            // Create and apply invalid patch document, assert that exception was thrown
            var patchDocument = new JsonPatchDocument();

            patchDocument.Operations.Add(new Operation
            {
                op = "add",
                path = "/PictureUrl",
                value = "https://s3.eu-west-2.wasabisys.com/eichkust-user-avatars/04.jpg"
            });

            Assert.ThrowsAsync<Exception>(async () =>
               await UserRepository.ApplyPatchDocumentAsyncTo(user, patchDocument),
               message: "New picture file doesn't exist");
        }

        [Test]
        public async Task UserRepository_RegisterUserAsync_SuccessfulOnValidData()
        {
            // Create valid test user
            var user = new ApplicationUser()
            {
                UserName = "testusername",
                Email = "test@user.com",
                PhoneNumber = "88005553535",
                Desciption = "test description",
                DisplayName = "Test username"
            };

            // Assert that registration was successful
            var registrationResult = await UserRepository
                .RegisterUserAsync(user, "Test1_password");

            Console.WriteLine(JsonSerializer.Serialize(registrationResult));

            Assert.That(registrationResult, Is.Null);
        }

        [Test]
        public async Task UserRepository_RegisterUserAsync_NotSuccessfulOnInvalidData()
        {
            // Create invalid test user
            var user = new ApplicationUser()
            {
                UserName = "thirduser",
                Email = "third@user.com",
                PhoneNumber = "88005553535",
                Desciption = "test description",
                DisplayName = "Test username"
            };

            // Assert that registration was not successful
            var registrationResult = await UserRepository
                .RegisterUserAsync(user, "testpassword"); // Password is invalid

            Console.WriteLine(JsonSerializer.Serialize(registrationResult));

            Assert.That(registrationResult, Is.Not.Null);
        }
    }
}
