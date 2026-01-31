namespace Models.Response
{
    public class FacturaListadoResponse
    {
        public int FacturaID { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string VendedorNombre { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; }
        public decimal Total { get; set; }
        public bool Activo { get; set; }
    }
}
