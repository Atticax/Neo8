using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PatchManager.Model.Entity;

namespace PatchManager.Model
{
    class PatchDbContext : DbContext
    {
        public DbSet<ManagedProgramEntity> ManagedPrograms { get; set; }
        public DbSet<PatchEntity> Patches { get; set; }
        public DbSet<ProgramVersionEntity> ProgramVersions { get; set; }

        public PatchDbContext(DbContextOptions<PatchDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ManagedProgramEntity>()
                .HasIndex(x => x.Name).IsUnique();
        }

    }
}
