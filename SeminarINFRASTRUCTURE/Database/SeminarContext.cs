using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.DOMAIN.Entitys;

namespace Seminar.INFRASTRUCTURE.Database
{
    public class SeminarContext : DbContext
    {
        public SeminarContext(DbContextOptions<SeminarContext> options) : base(options)
        {

        }

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
    }
}
