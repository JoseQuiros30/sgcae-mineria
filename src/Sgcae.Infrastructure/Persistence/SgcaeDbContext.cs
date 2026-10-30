using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Infrastructure.Persistence;

public sealed class SgcaeDbContext(DbContextOptions<SgcaeDbContext> options)
    : DbContext(options), IUnidadTrabajo
{
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Credencial> Credenciales => Set<Credencial>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<PuntoAcceso> PuntosAcceso => Set<PuntoAcceso>();
    public DbSet<MovimientoAcceso> MovimientosAcceso => Set<MovimientoAcceso>();

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarEmpleado(modelBuilder);
        ConfigurarCredencial(modelBuilder);
        ConfigurarSede(modelBuilder);
        ConfigurarPuntoAcceso(modelBuilder);
        ConfigurarMovimiento(modelBuilder);
        AgregarDatosIniciales(modelBuilder);
    }

    private static void ConfigurarEmpleado(ModelBuilder builder)
    {
        var entidad = builder.Entity<Empleado>();
        entidad.ToTable("Empleados");
        entidad.HasKey(x => x.Id);
        entidad.Property(x => x.Identificacion).HasMaxLength(30).IsRequired();
        entidad.HasIndex(x => x.Identificacion).IsUnique();
        entidad.Property(x => x.Nombres).HasMaxLength(100).IsRequired();
        entidad.Property(x => x.Apellidos).HasMaxLength(100).IsRequired();
        entidad.Property(x => x.Cargo).HasMaxLength(100).IsRequired();
        entidad.Property(x => x.AreaOrganizacional).HasMaxLength(100).IsRequired();
        entidad.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
        entidad.OwnsOne(x => x.Correo, correo =>
        {
            correo.Property(x => x.Valor).HasColumnName("Correo").HasMaxLength(150).IsRequired();
            correo.HasIndex(x => x.Valor).IsUnique();
        });
    }

    private static void ConfigurarCredencial(ModelBuilder builder)
    {
        var entidad = builder.Entity<Credencial>();
        entidad.ToTable("Credenciales");
        entidad.HasKey(x => x.Id);
        entidad.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        entidad.HasIndex(x => x.Codigo).IsUnique();
        entidad.HasIndex(x => x.EmpleadoId).IsUnique().HasFilter("[Estado] = 'Vigente'");
        entidad.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
        entidad.HasOne(x => x.Empleado).WithMany().HasForeignKey(x => x.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigurarSede(ModelBuilder builder)
    {
        var entidad = builder.Entity<Sede>();
        entidad.ToTable("Sedes");
        entidad.HasKey(x => x.Id);
        entidad.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
        entidad.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        entidad.HasIndex(x => x.Codigo).IsUnique();
    }

    private static void ConfigurarPuntoAcceso(ModelBuilder builder)
    {
        var entidad = builder.Entity<PuntoAcceso>();
        entidad.ToTable("PuntosAcceso");
        entidad.HasKey(x => x.Id);
        entidad.Property(x => x.Codigo).HasMaxLength(30).IsRequired();
        entidad.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        entidad.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(30);
        entidad.HasIndex(x => x.Codigo).IsUnique();
        entidad.HasOne(x => x.Sede).WithMany().HasForeignKey(x => x.SedeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigurarMovimiento(ModelBuilder builder)
    {
        var entidad = builder.Entity<MovimientoAcceso>();
        entidad.ToTable("MovimientosAcceso");
        entidad.HasKey(x => x.Id);
        entidad.Property(x => x.CodigoCredencial).HasMaxLength(50).IsRequired();
        entidad.Property(x => x.CodigoPuntoAcceso).HasMaxLength(30).IsRequired();
        entidad.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(20);
        entidad.Property(x => x.Resultado).HasConversion<string>().HasMaxLength(20);
        entidad.Property(x => x.Motivo).HasMaxLength(250).IsRequired();
        entidad.HasIndex(x => x.RegistradoEnUtc);
        entidad.HasIndex(x => new { x.EmpleadoId, x.RegistradoEnUtc });
        entidad.HasOne(x => x.Credencial).WithMany().HasForeignKey(x => x.CredencialId)
            .OnDelete(DeleteBehavior.NoAction);
        entidad.HasOne(x => x.Empleado).WithMany().HasForeignKey(x => x.EmpleadoId)
            .OnDelete(DeleteBehavior.NoAction);
        entidad.HasOne(x => x.PuntoAcceso).WithMany().HasForeignKey(x => x.PuntoAccesoId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void AgregarDatosIniciales(ModelBuilder builder)
    {
        var guayabito = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var guaico = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var relavera = Guid.Parse("10000000-0000-0000-0000-000000000003");

        builder.Entity<Sede>().HasData(
            new Sede(guayabito, "GUAYABITO", "Mina Guayabito"),
            new Sede(guaico, "GUAICO", "Mina Guaico"),
            new Sede(relavera, "RELAVERA", "Relavera"));

        builder.Entity<PuntoAcceso>().HasData(
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Codigo = "GYA-PORTERIA", Nombre = "Portería Mina Guayabito", SedeId = guayabito, Tipo = TipoPuntoAcceso.Porteria, Habilitado = true },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Codigo = "GUA-PORTERIA", Nombre = "Portería Mina Guaico", SedeId = guaico, Tipo = TipoPuntoAcceso.Porteria, Habilitado = true },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Codigo = "REL-INGRESO", Nombre = "Ingreso Relavera", SedeId = relavera, Tipo = TipoPuntoAcceso.Relavera, Habilitado = true });
    }
}
