using Microsoft.EntityFrameworkCore;
using AwarenessCampaign.Models;
using System.Reflection.Emit;

namespace AwarenessCampaign
{
    public class AwarenessCampaignDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Category { get; set; }

        public AwarenessCampaignDbContext(DbContextOptions<AwarenessCampaignDbContext> context) : base(context)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User[]
            {
        new User { Id = 1, UID = "user1", UserName = "John Doe" },
        new User { Id = 2, UID = "user2", UserName = "Jane Smith" }
            });

            modelBuilder.Entity<Category>().HasData(new Category[]
            {
        new Category { Id = 1, CategoryName = "Category A" },
        new Category { Id = 2, CategoryName = "Category B" }
            });

            modelBuilder.Entity<Post>().HasData(new Post[]
            {
        new Post { Id = 1, PostName = "Post 1", Description = "Description for Post 1", UserId = 1 },
        new Post { Id = 2, PostName = "Post 2", Description = "Description for Post 2", UserId = 2 }
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
