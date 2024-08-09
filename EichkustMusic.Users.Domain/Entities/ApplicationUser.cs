using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EichkustMusic.Users.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Url]
        public string? PictureUrl {  get; set; }

        [MaxLength(1024)]
        public string? Desciption { get; set; }

        [Required]
        [MaxLength(64)]
        public string DisplayName { get; set; } = null!;

        public IEnumerable<PublisherSubscriber> PublisherSubscribers { get; set; }
            = new List<PublisherSubscriber>();

        public IEnumerable<ApplicationUser> Subscriptions
        {
            get
            {
                return PublisherSubscribers.Select(publisherSubscriber => publisherSubscriber.Publisher);
            }
        }
    }
}
