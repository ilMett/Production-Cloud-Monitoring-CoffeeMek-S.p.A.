using Microsoft.EntityFrameworkCore;
using PW2_Gruppo3.Models;
using System.Reflection.PortableExecutable;

namespace PW2_Gruppo3.ApiService.Data;

public class ProductionMonitoringContext : DbContext
{
    public ProductionMonitoringContext(DbContextOptions<ProductionMonitoringContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<Site> Sites { get; set; }

    // Dati produzione per i diversi tipi di macchina (telemetria)
    public DbSet<Milling> Millings { get; set; }
    public DbSet<Lathe> Lathes { get; set; }
    public DbSet<AssemblyLine> AssemblyLines { get; set; }
    public DbSet<TestLine> TestLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //// Informa EF che AssemblyLine eredita da ProductionData
        //modelBuilder.Entity<AssemblyLine>().HasBaseType<ProductionData>();

        //// Configura la chiave primaria sulla classe base
        modelBuilder.Entity<Batch>()
            .ToTable("Batches")
            .HasOne<Customer>(x => x.Customer)
            .WithMany(s => s.Batches)
            .HasForeignKey(b => b.CustomerId);

        // FK verso Site
        modelBuilder.Entity<Batch>()
            .HasOne<Site>(x => x.Site)
            .WithMany(s => s.Batches)
            .HasForeignKey(b => b.SiteId);

        //modelBuilder.Entity<Customer>()
        //    .ToTable("Customers")
        //    .HasMany(c => c.Batches);

        modelBuilder.Entity<AssemblyLine>().ToTable("AssemblyLines");

        modelBuilder.Entity<Milling>().Property(m => m.CuttingDepth).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<Milling>().Property(m => m.Vibration).HasColumnType("decimal(18, 4)");

        modelBuilder.Entity<Lathe>().Property(l => l.SpindleTemperature).HasColumnType("decimal(18, 4)");

        modelBuilder.Entity<TestLine>().Property(t => t.BoilerPressure).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<TestLine>().Property(t => t.BoilerTemperature).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<TestLine>().Property(t => t.EnergyConsumption).HasColumnType("decimal(18, 4)");

    }
}

