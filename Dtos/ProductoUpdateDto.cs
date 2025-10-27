namespace WebTonyWilly.Dtos
{
    public class ProductoUpdateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public string CodigoBarras { get; set; }
        // 👇 Agregá esta línea
        public int? CategoriaId { get; set; }
    }
}
