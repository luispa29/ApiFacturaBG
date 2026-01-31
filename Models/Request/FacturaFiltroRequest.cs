namespace Models.Request
{
    public class FacturaFiltroRequest
    {
        public string? NumeroFactura { get; set; }
        public int? ClienteID { get; set; }
        public int? VendedorID { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
    }
}
