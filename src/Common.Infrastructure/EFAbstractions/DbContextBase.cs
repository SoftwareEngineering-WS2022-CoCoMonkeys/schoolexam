using System;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EFAbstractions
{
    public class DbContextBase : DbContext
    {
        private readonly IDbConnectionConfiguration _configuration;
        
        public DbContextBase(IDbConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_configuration.Technology)
            {
                case DbTechnology.PostgresSql:
                    optionsBuilder.UseNpgsql(_configuration.ConnectionString);
                    break;
                default:
                    throw new ArgumentException("Configured database technology is not supported.");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}