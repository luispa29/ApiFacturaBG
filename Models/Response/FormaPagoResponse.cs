namespace Models.Response
{
    public class FormaPagoResponse
    {
        public int FormaPagoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
