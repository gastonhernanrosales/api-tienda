namespace WebTonyWilly.Dtos
{
    public class ConfirmarVentaDto
    {
        public int PagoId { get; set; }

        public int UsuarioId { get; set; }    // <-- nuevo
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public List<VentaDetalleRequest> Detalles { get; set; }
    }
}
