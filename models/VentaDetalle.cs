using System.Text.Json.Serialization;

namespace WebTonyWilly.models
{
    public class VentaDetalle
    {
        public int Id { get; set; }


        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int VentaId { get; set; }

        [JsonIgnore]  // Evita la referencia circular
        public Venta Venta { get; set; } = null!;


    }
}
