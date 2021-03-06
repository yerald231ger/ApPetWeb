﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppWebApp.Models;

namespace AppWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pet> Pets { get; set; }
        public virtual DbSet<PetType> PetTypes { get; set; }
        public virtual DbSet<Veterinary> Veterinaries { get; set; }
        public virtual DbSet<VetService> VetServices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);     
            builder.Entity<ApplicationUser>().ToTable("tblUser");
            builder.Entity<IdentityRole>().ToTable("tblRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("tblUserClaim");
            builder.Entity<IdentityUserRole<string>>().ToTable("tblUserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("tblUserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("tblRoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("tblUserToken");

            builder.Entity<Pet>(build =>
            {
                build.ToTable("tblPets");
                build.HasOne(p => p.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserId);

            });

            builder.Entity<PetType>(build =>
            {
                build.ToTable("tblPetTypes");
                build.HasMany(pt => pt.Pets)
                .WithOne(p => p.PetType)
                .HasForeignKey(p => p.PetTypeId);
            });

            builder.Entity<VeterinaryVetService>().ToTable("tblVeterinaryVetServices");

            builder.Entity<Veterinary>().ToTable("tblVeterinaries");

            builder.Entity<VetService>().ToTable("tblVetServices");

            builder.Entity<VeterinaryVetService>().HasKey(vvs => new { vvs.VeterinaryId , vvs.VetServiceId });

            builder.Entity<VeterinaryVetService>()
                .HasOne(vvs => vvs.Veterinary)
                .WithMany(v => v.VeterinaryVetServices)
                .HasForeignKey(vvs => vvs.VeterinaryId);

            builder.Entity<VeterinaryVetService>()
                .HasOne(vvs => vvs.VetService)
                .WithMany(vs => vs.VeterinaryVetServices)
                .HasForeignKey(vvs => vvs.VetServiceId);
        }
    }
}
