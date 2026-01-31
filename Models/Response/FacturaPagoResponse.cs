namespace Models.Response
{
    public class FacturaPagoResponse
    {
        public int PagoID { get; set; }
        public int FormaPagoID { get; set; }
        public string FormaPagoNombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = string.Empty;
    }
}
