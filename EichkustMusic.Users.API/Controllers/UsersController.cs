using EichkustMusic.S3;
using EichkustMusic.Users.Application.Models;
using EichkustMusic.Users.Application.S3;
using EichkustMusic.Users.Application.UserRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace EichkustMusic.Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IS3Storage _s3;

        public UsersController(IUserRepository userRepository, IS3Storage s3)
        {
            _userRepository = userRepository;
            _s3 = s3;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsDto?>> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(nameof(id));
            }

            var userDto = UserDetailsDto.MapFromApplicationUser(user);

            return userDto;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> List(
            string? query, int pageNum = 1, int pageSize = 5)
        {
            var users = await _userRepository.ListUsersAsync(pageNum, pageSize, query);

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = UserDto.MapFromApplicationUser(user);

                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        [HttpPost]
        public async Task<ActionResult<UserRegistrationResultDto?>> Create(
            UserForRegistrationDto userForRegistrationDto)
        {
            var user = userForRegistrationDto.MapToApplicationUser();

            var errors = await _userRepository.RegisterUserAsync(
                user, userForRegistrationDto.Password);

            if (errors != null)
            {
                return BadRequest(errors);
            }

            await _userRepository.SaveChangesAsync();

            var registrationResultDto = UserRegistrationResultDto.MapFromApplicationUser(user);

            registrationResultDto.UrlToUploadPicture = _s3
                .GetPreSignedUploadUrl(BucketNames.UserAvatars);

            return CreatedAtAction(nameof(GetById),
                new
                {
                    user.Id,

                },
                registrationResultDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(int id, JsonPatchDocument patchDocument)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(nameof(id));
            }

            await _userRepository.ApplyPatchDocumentAsyncTo(user, patchDocument);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("get_picture_upload_url")]
        public ActionResult<string> GetPictureUploadUrl()
        {
            return _s3.GetPreSignedUploadUrl(BucketNames.UserAvatars);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(nameof(id));
            }

            await _userRepository.DeleteUserAsync(user);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{subscriberId}/subscribe_to/{publisherId}")]
        public async Task<ActionResult> SubscribeToUser(int subscriberId, int publisherId)
        {
            var subscriber = await _userRepository.GetUserByIdAsync(subscriberId);

            if (subscriber == null)
            {
                return NotFound(nameof(subscriberId));
            }

            var publisher = await _userRepository.GetUserByIdAsync(publisherId);

            if (publisher == null)
            {
                return NotFound(nameof(publisherId));
            }

            var isSuccessful = await _userRepository.AddSubscriptionAsync(subscriber, publisher);

            if (isSuccessful == false)
            {
                return BadRequest("User is already subscribed");
            }

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{subscriberId}/unsubscribe_from/{publisherId}")]
        public async Task<ActionResult> UnsubscribeFrom(int subscriberId, int publisherId)
        {
            var subscriber = await _userRepository.GetUserByIdAsync(subscriberId);

            if (subscriber == null)
            {
                return NotFound(nameof(subscriberId));
            }

            var publisher = await _userRepository.GetUserByIdAsync(publisherId);

            if (publisher == null)
            {
                return NotFound(nameof(publisherId));
            }

            await _userRepository.DeleteSubscriptionAsync(subscriber, publisher);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/subscribers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSubscribers(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(nameof(id));
            }

            var subscriberDtos = new List<UserDto>();

            foreach (var subscriber in user.Subscribers)
            {
                var subscriberDto = UserDto.MapFromApplicationUser(subscriber);

                subscriberDtos.Add(subscriberDto);
            }

            return Ok(subscriberDtos);
        }

        [HttpGet("{id}/subscriptions")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSubscriptions(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(nameof(id));
            }

            var subscriptionDtos = new List<UserDto>();

            foreach (var subscription in user.Subscriptions)
            {
                var subscriberDto = UserDto.MapFromApplicationUser(subscription);

                subscriptionDtos.Add(subscriberDto);
            }

            return Ok(subscriptionDtos);
        }
    }
}
