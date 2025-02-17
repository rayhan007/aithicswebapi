using aithics.data.Data;
using aithics.data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime;

namespace aithics.data
{
    public class AithicsDbContext : DbContext
    {
        private readonly string _connectionString;
        public AithicsDbContext(DbContextOptions<AithicsDbContext> options) : base(options) {
            _connectionString = DbSettings.ConnectionString;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<APIListing> APIlistings { get; set; }
        public DbSet<RoleToAPI> RoleToAPIs { get; set; }
        public DbSet<SleepTracker> SleepTrackers { get; set; }
    }
}
