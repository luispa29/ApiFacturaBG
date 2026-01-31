namespace Models.Response
{
    public class FormaPagoInvalidaResponse
    {
        public int FormaPagoID { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
    }
}
