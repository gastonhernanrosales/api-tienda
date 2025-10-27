namespace WebTonyWilly.Dtos
{
    public class VentaRequest
    {
        public int PagoId { get; set; }
        public int? UsuarioId { get; set; }
        public string MetodoPago { get; set; }
        public List<VentaDetalleRequest> Detalles { get; set; }
        public decimal Total { get; set; }
    }

}
