namespace Models.Utils
{
    public class DetalleCalculado
    {
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
