namespace WebTonyWilly.Dtos
{
    public class PagoRequest
    {
        public decimal Monto { get; set; }
        public string Metodo { get; set; } // "efectivo", "tarjeta", "qr"
        public string Email { get; set; }
    }

}
