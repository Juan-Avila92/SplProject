using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrasctructure.Persistence
{

    public class DashboardDbContext : DbContext
    {
        public DashboardDbContext(
            DbContextOptions<DashboardDbContext> options)
            : base(options)
        {
        }

        public DbSet<DashboardOverview> DashboardOverviews =>
            Set<DashboardOverview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DashboardOverview>(entity =>
            {
                entity.ToTable("DashboardOverviews");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId)
                    .IsRequired();

                entity.Property(x => x.TotalProjects)
                    .IsRequired();

                entity.Property(x => x.LastUpdated)
                    .IsRequired();

                entity.HasIndex(x => x.UserId)
                    .IsUnique();
            });
        }
    }
}
