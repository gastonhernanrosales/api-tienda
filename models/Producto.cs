namespace WebTonyWilly.models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; } 
        public int Stock { get; set; } 
        public string ImageUrl { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;

        // 🔹 Relación con Categoría
        public int CategoriaId { get; set; }          // Clave foránea
        public Categoria? Categoria { get; set; }      // Navegación
    }
}
