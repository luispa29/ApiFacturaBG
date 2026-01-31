namespace Models.Request
{
    public class ProductoRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int StockActual { get; set; }
    }
}
