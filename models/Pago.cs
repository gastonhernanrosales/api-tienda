namespace WebTonyWilly.models
{
    public class Pago
    {
        public int Id { get; set; }
        public decimal Monto { get; set; } 
        public string Metodo { get; set; } = string.Empty;// "efectivo", "tarjeta", "qr"
        public string Email { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;// "pendiente", "approved", "completado"
        public DateTime Fecha { get; set; }

    }
}
