using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EichkustMusic.Users.Domain.Entities
{
    public class PublisherSubscriber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        public int SubscriberId { get; set; }

        [ForeignKey(nameof(PublisherId))]
        public ApplicationUser Publisher { get; set; } = null!;

        [ForeignKey(nameof(SubscriberId))]
        public ApplicationUser Subscriber { get; set; } = null!;
    }
}
