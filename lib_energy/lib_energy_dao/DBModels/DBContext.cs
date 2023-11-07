using Microsoft.EntityFrameworkCore;

namespace lib_energy_dao.DBModels
{
    public class Energy_DBContext : DbContext
    {
        public Energy_DBContext(DbContextOptions<Energy_DBContext> options) : base(options) { }

        public DbSet<DBElectricityConsumption> DBElectricityConsumption { get; set; }
        public DbSet<DBRenewableElectricity> DBRenewableElectricity { get; set; }
        public DbSet<DBElectricityProduction> DBElectricityProduction{ get; set; }
        public DbSet<DBCity> DBCity { get; set; }
        public DbSet<DBFuel> DBFuel { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBElectricityConsumption>(p =>
            {
                p.Property(p => p.ElectricityConsumptionID).HasColumnName("e_consumptionID");
                p.Property(p => p.CityID).HasColumnName("fk_cityID");
                p.HasKey(c => new { c.ElectricityConsumptionID });
                p.ToTable("electricity_consumption");
            });

            modelBuilder.Entity<DBRenewableElectricity>(p =>
            {
                p.Property(p => p.RenewableElectricityID).HasColumnName("renewable_electricity_id");
                p.HasKey(c => new { c.RenewableElectricityID });
                p.ToTable("renewable_electricity");
            });

            modelBuilder.Entity<DBElectricityProduction>(p =>
            {
                p.Property(p => p.ElectricityProductionID).HasColumnName("electricity_productionID");
                p.Property(p => p.FuelID).HasColumnName("fk_fuelID");
                p.HasKey(c => new { c.ElectricityProductionID });
                p.ToTable("electricity_production");
            });

            modelBuilder.Entity<DBCity>(p =>
            {
                p.HasKey(c => new { c.CityID });
                p.ToTable("city");
            });
            modelBuilder.Entity<DBFuel>(p =>
            {
                p.HasKey(c => new { c.FuelID});
                p.ToTable("fuel");
            });


        }
    }
}