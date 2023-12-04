using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using InterviewProject.Models;

namespace InterviewProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Individual_Customer_Tbl> Individual_Customer_Tbl { get; set; }
        public DbSet<Corporate_Customer_Tbl> Corporate_Customer_Tbl { get; set; }
        public DbSet<Products_Service_Tbl> Products_Service_Tbl { get; set; }
    }
}
