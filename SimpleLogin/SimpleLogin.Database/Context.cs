using Microsoft.EntityFrameworkCore;
using SimpleLogin.Database.Models;
using System;

namespace SimpleLogin.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base (options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
