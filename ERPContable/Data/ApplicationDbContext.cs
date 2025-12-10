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
    }
}
