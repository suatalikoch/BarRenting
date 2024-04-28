using BarRating.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public virtual DbSet<Bar> Bars { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Review>()
                .HasOne(review => review.User)
                .WithMany(user => user.Reviews)
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Review>()
                .HasOne(review => review.Bar)
                .WithMany(bar => bar.Reviews)
                .HasForeignKey(review => review.BarId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Bar>()
                .HasMany(bar => bar.Reviews)
                .WithOne(review => review.Bar)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(user => user.Reviews)
                .WithOne(review => review.User)
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            base.OnModelCreating(builder);
        }
    }
}
