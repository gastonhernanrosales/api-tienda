
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.models;

namespace WebTonyWilly.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // Acá definís tus tablas, ejemplo:
        // public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Turno> Turnos { get; set; }



    }

}



