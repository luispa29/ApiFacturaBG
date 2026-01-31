namespace Models.Request
{
    public class FacturaRequest
    {
        public int Id { get; set; }
        public int ClienteID { get; set; }
        public int VendedorID { get; set; }
        public DateTime FechaFactura { get; set; }
        public List<FacturaDetalleRequest> Detalles { get; set; } = new List<FacturaDetalleRequest>();
        public List<FacturaPagoRequest> Pagos { get; set; } = new List<FacturaPagoRequest>();
    }
}
