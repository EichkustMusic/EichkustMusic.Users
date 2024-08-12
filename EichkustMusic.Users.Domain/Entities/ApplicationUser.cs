using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [InverseProperty(nameof(PublisherSubscriber.Publisher))]
        public IEnumerable<PublisherSubscriber> SubscriptionsM2M { get; set; }
            = new List<PublisherSubscriber>();

        [InverseProperty(nameof(PublisherSubscriber.Subscriber))]
        public IEnumerable<PublisherSubscriber> SubsribersM2M { get; set; }
            = new List<PublisherSubscriber>();

        public IEnumerable<ApplicationUser> Subscriptions
        {
            get => SubscriptionsM2M.Select(m2m => m2m.Publisher);
        }

        public IEnumerable<ApplicationUser> Subscribers
        {
            get => SubsribersM2M.Select(m2m => m2m.Subscriber);
        }
    }
}
