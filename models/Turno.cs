namespace WebTonyWilly.models
{
    public class Turno
    {
        public int Id { get; set; }

        // FK a Usuario
        public int UsuarioId { get; set; }         // clave foránea obligatoria
        public Usuario Usuario { get; set; } = null!; // navegación

        // Fecha + hora de inicio del turno
        public DateTime Inicio { get; set; }

        // Fecha + hora de cierre (null si sigue abierto)
        public DateTime? Cierre { get; set; }

        // Fondo inicial (la plata de arranque)
        public decimal FondoInicial { get; set; }
        // Totales del turno
        public decimal? TotalEfectivo { get; set; }
        public decimal? TotalMP { get; set; }
        // Efectivo final contado por el cajero
        public decimal? EfectivoFinal { get; set; }

        // Calculado por el sistema al cerrar
        public decimal? EfectivoEsperado { get; set; }

        // Diferencia (expected - final)
        public decimal? Diferencia { get; set; }

        // Estado del turno
        public bool EstaCerrado { get; set; }
    }
}
