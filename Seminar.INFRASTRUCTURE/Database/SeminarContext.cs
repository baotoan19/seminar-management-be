using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.DOMAIN.Entitys;

namespace Seminar.INFRASTRUCTURE.Database
{
    public class SeminarContext : DbContext
    {
        public SeminarContext()
        {

        }
        public SeminarContext(DbContextOptions<SeminarContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(60),
                        errorNumbersToAdd: null);
                });
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Artical> Articals { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<RegistrationForm> RegistrationForms { get; set; }
        public DbSet<Review_Assignment> Review_Assignments { get; set; }
        public DbSet<Review_Form> Review_Forms { get; set; }
        public DbSet<Review_Committee> Review_Committees { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Proceeding> Proceedings { get; set; }
        public DbSet<Author_Artical> Author_Articlas { get; set; }
        public DbSet<History_Update_Artical> History_Update_Articals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Organizer>()
                .HasIndex(o => o.AccountId)
                .IsUnique();

            modelBuilder.Entity<Author>()
                .HasIndex(a => a.AccountId)
                .IsUnique();

            modelBuilder.Entity<Reviewer>()
                .HasIndex(a => a.AccountId)
                .IsUnique();
        }

    }
}