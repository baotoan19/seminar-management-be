using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace Seminar.INFRASTRUCTURE.Database
{
    public class SeminarContextFactory : IDesignTimeDbContextFactory<SeminarContext>
    {
        public SeminarContext CreateDbContext(string[] args)
        {
            //Load env
            var optionsBuilder = new DbContextOptionsBuilder<SeminarContext>();
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            Console.WriteLine("connectionString: " + connectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set. 111");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new SeminarContext(optionsBuilder.Options);
        }
    }
}