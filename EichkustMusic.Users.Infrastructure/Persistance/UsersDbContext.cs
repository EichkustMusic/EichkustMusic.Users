using EichkustMusic.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EichkustMusic.Users.Infrastructure.Persistance
{
    public class UsersDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        { }

        public DbSet<PublisherSubscriber> PublisherSubscribers { get; set; } = null!;
    }   
}
