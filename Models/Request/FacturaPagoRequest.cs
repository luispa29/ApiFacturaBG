namespace Models.Request
{
    public class FacturaPagoRequest
    {
        public int FormaPagoID { get; set; }
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = string.Empty;
    }
}
