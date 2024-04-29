using Catalog.Domain.StoredProcedure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }
        public DbSet<MemoryPerson> MemoryPerson { get; set; }
        public DbSet<MemoryPersonDetail> MemoryPersonDetail { get; set; }
        public DbSet<MemoryPersonByUserId> MemoryPersonByUserId { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Memories");
            ModelConfig(builder);
        }

        public void ModelConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemoryPerson>().HasNoKey();
            modelBuilder.Entity<MemoryPersonDetail>().HasNoKey();
            modelBuilder.Entity<MemoryPersonByUserId>().HasNoKey();
        }
    }
}
