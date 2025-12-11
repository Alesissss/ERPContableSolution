using ERPContable.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPContable.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Propiedad DbSet por cada entidad/tabla
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<PersonalArea> PersonalAreas { get; set; }
        public DbSet<Personal> Personales { get; set; }
        public DbSet<ReqInterno> ReqInternos { get; set; }
        public DbSet<DReqInterno> DReqInternos { get; set; }
        public DbSet<OCompra> OCompras { get; set; }
        public DbSet<DCompra> DCompras { get; set; }
        public DbSet<IngresoSalidaAlm> IngresoSalidaAlms { get; set; }
        public DbSet<DIngresoSalidaAlm> DIngresoSalidaAlms { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Umedida> Umedidas { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DCompra>()
                .HasKey(dc => new { dc.ocompraId, dc.productoId });
            modelBuilder.Entity<DIngresoSalidaAlm>()
                .HasKey(dc => new { dc.ingresoSalidaAlmId, dc.productoId });
            modelBuilder.Entity<DReqInterno>()
                .HasKey(dc => new { dc.reqinternoId, dc.productoId });
            modelBuilder.Entity<PersonalArea>()
                .HasKey(dc => new { dc.personalId, dc.areaId });

            base.OnModelCreating(modelBuilder);
        }

    }
}