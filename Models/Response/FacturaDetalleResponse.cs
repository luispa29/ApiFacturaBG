namespace Models.Response
{
    public class FacturaDetalleResponse
    {
        public int DetalleID { get; set; }
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
