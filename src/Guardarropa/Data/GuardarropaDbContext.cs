using System.IO;
using Microsoft.EntityFrameworkCore;
using Guardarropa.Models;

namespace Guardarropa.Data;

public class GuardarropaDbContext : DbContext
{
    public DbSet<Configuracion> Configuraciones => Set<Configuracion>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<CierreZ> CierresZ => Set<CierreZ>();

    private readonly string _dbPath;

    public GuardarropaDbContext()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, "Guardarropa");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, "guardarropa.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Configuracion>(e =>
        {
            e.Property(c => c.PrecioPorPrenda).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Ticket>(e =>
        {
            e.Property(t => t.PrecioPorPrenda).HasColumnType("decimal(10,2)");
            e.Property(t => t.PrecioTotal).HasColumnType("decimal(10,2)");
            e.HasOne(t => t.Empresa).WithMany().HasForeignKey(t => t.EmpresaId);
            e.HasOne(t => t.CierreZ).WithMany(z => z.Tickets).HasForeignKey(t => t.CierreZId);
        });

        modelBuilder.Entity<CierreZ>(e =>
        {
            e.Property(z => z.TotalRecaudado).HasColumnType("decimal(10,2)");
            e.HasOne(z => z.Empresa).WithMany().HasForeignKey(z => z.EmpresaId);
        });
    }
}
