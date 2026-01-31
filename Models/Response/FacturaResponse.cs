namespace Models.Response
{
    public class FacturaResponse
    {
        public int FacturaID { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        
        public int ClienteID { get; set; }
        public string ClienteIdentificacion { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        
        public int VendedorID { get; set; }
        public string VendedorNombre { get; set; } = string.Empty;
        
        public DateTime FechaFactura { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        public List<FacturaDetalleResponse> Detalles { get; set; } = new List<FacturaDetalleResponse>();
        public List<FacturaPagoResponse> Pagos { get; set; } = new List<FacturaPagoResponse>();
    }
}
