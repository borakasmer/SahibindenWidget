namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Banner : DbContext
    {
        public Banner()
            : base("name=Banner")
        {
        }

        public virtual DbSet<SahibindenCars> SahibindenCars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SahibindenCars>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<SahibindenCars>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<SahibindenCars>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<SahibindenCars>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<SahibindenCars>()
                .Property(e => e.Place)
                .IsUnicode(false);
        }
    }
}
