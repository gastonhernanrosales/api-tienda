namespace WebTonyWilly.Dtos
{
    public class AuthRequest
    {
        public string Token { get; set; }
        public string Usuario { get; set; }
        public string Rol { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
