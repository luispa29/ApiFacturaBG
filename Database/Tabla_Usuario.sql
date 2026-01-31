

CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    HashContrasena VARBINARY(MAX) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
);

CREATE INDEX IX_Usuario_NombreUsuario ON Usuarios(Username);
CREATE INDEX IX_Usuario_CorreoElectronico ON Usuarios(Email);
CREATE INDEX IX_Usuario_Activo ON Usuarios(Activo);

GO
