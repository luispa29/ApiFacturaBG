namespace Models.Request
{
    public class FacturaDetalleRequest
    {
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
