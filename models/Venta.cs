namespace WebTonyWilly.models
{
    public class Venta
    {

        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public int PagoId { get; set; }           // <-- Esta línea falta
        public Pago Pago { get; set; } = null!;

        public string Estado { get; set; } = "activa"; // Nuevo campo: "activa" o "cancelada"
        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
