using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Lead.Api.DataModel.Models;
using Models = Lead.Api.DataModel.Models;

namespace Lead.Api.DataModel
{
    public class LeadContext : DbContext
    {
        public LeadContext(DbContextOptions<LeadContext> options) : base(options)
        {

        }

        public virtual DbSet<Models.Lead> Leads { get; set; }
    
        public virtual DbSet<Contractor> Contractors { get; set; }

        public virtual DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 1, CompanyName = "Test Contractor" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 2, CompanyName = "Test Contractor 2" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 3, CompanyName = "Test Contractor 3" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 4, CompanyName = "Test Contractor 4" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 5, CompanyName = "Test Contractor 5" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 6, CompanyName = "Test Contractor 6" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 7, CompanyName = "Test Contractor 7" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 8, CompanyName = "Test Contractor 8" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 9, CompanyName = "Test Contractor 9" });
            modelBuilder.Entity<Contractor>().HasData(new Contractor { ContractorID = 10, CompanyName = "Test Contractor 10" });
            
            base.OnModelCreating(modelBuilder);
        }


    }
}
