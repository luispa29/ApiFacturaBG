namespace Models.Response
{
    public class ProductoResponse
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int StockActual { get; set; }
    }
}
