

CREATE TABLE Usuario (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    HashContrasena VARBINARY(MAX) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    CorreoElectronico NVARCHAR(100) NOT NULL UNIQUE,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME DEFAULT GETDATE()
);

CREATE INDEX IX_Usuario_NombreUsuario ON Usuario(NombreUsuario);
CREATE INDEX IX_Usuario_CorreoElectronico ON Usuario(CorreoElectronico);
CREATE INDEX IX_Usuario_Activo ON Usuario(Activo);

GO
