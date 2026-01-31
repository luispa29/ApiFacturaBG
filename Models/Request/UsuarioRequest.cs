

namespace Models.Request
{
    public class UsuarioRequest
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
    public class UsuarioUpdateRequest
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public bool ActualizarContrasena { get; set; } = true;
    }
}
