using Microsoft.EntityFrameworkCore;
using Ads.Web.Models;

namespace Ads.Web.Data
{
    public class AdsContext : DbContext
    {
        public AdsContext(DbContextOptions<AdsContext> options) : base(options) { }

        public DbSet<Annonsor> Annonsorer => Set<Annonsor>();
        public DbSet<Annons> Annonser => Set<Annons>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Annons>()
                .HasOne(a => a.Annonsor)
                .WithMany(o => o.Annonser)
                .HasForeignKey(a => a.AnnonsorId)
                .HasConstraintName("FK_tbl_ads_tbl_annonsorer");
        }
    }
}
