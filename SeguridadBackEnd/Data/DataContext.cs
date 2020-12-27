using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeguridadBackEnd.Data.Entities;


namespace SeguridadBackEnd.Data
{
    public class DataContext : IdentityDbContext<UserEntity>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ProductEntity>()
            //.HasIndex(t => t.CodeProduct)
            //.IsUnique();

            //modelBuilder.Entity<ProductEntity>()
            //.HasIndex(t => t.NameProduct)
            //.IsUnique();

            //modelBuilder.Entity<ClientEntity>()
            //.HasIndex(t => t.IdentificationCLient)
            //.IsUnique();

            //modelBuilder.Entity<IncomeEntity>()
            //.HasOne(a => a.objIncomePayOfDebtEntity)
            //.WithOne(a => a.IncomeEntity)
            //.HasForeignKey<IncomePayOfDebtEntity>(c => c.IncomeId);

            //modelBuilder.Entity<EgressEntity>()
            //.HasOne(a => a.objEgressPurchaseEntity)
            //.WithOne(a => a.EgressEntity)
            //.HasForeignKey<EgressPurchaseEntity>(c => c.EgressId);

        }
    }
}
