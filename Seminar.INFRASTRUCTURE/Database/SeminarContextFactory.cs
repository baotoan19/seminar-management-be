// using dotenv.net;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Seminar.CORE.ExceptionCustom;
// namespace Seminar.INFRASTRUCTURE.Database
// {
//     public class SeminarContextFactory : IDesignTimeDbContextFactory<SeminarContext>
//     {
//         public SeminarContext CreateDbContext(string[] args)
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<SeminarContext>();
//             var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
//             if (string.IsNullOrEmpty(connectionString))
//             {
//                 throw new CoreException("Connection string is not set", "DB_CONNECTION_STRING");
//             }

//             optionsBuilder.UseSqlServer(connectionString);

//             return new SeminarContext(optionsBuilder.Options);
//         }
//     }
// }