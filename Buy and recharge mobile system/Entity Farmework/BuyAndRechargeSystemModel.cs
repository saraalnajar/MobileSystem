using System.Data.Entity;

namespace EntityFarmework
{
    public partial class BuyAndRechargeSystemModel : DbContext
    {
        public BuyAndRechargeSystemModel(DatabaseConfiguration config)
            : base(config.ConnectionString)
        {
        }

        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Packages> Packages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Customers>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Customers>()
                .Property(e => e.BirthDate)
                .IsUnicode(false);

            modelBuilder.Entity<Customers>()
                .Property(e => e.Balance)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Packages>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Packages>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Packages>()
                .HasMany(e => e.Customers)
                .WithRequired(e => e.Packages)
                .HasForeignKey(e => e.PackageID)
                .WillCascadeOnDelete(false);
        }
    }
}
