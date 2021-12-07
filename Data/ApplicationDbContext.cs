using ITicketSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITicketSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                   .HasIndex(u => u.EmployeeNumber)
                   .IsUnique();
            modelBuilder.Entity<ApplicationUser>()
                   .HasIndex(u => u.Email)
                   .IsUnique();
            modelBuilder.Entity<ApplicationUser>()
                   .HasIndex(u => u.UserName)
                   .IsUnique();
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        

        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
