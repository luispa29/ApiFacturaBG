CREATE TABLE dbo.Clientes (
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,
    Identificacion NVARCHAR(20) NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1
);

CREATE INDEX IX_Cliente_Identificacion ON Clientes(Identificacion);
CREATE INDEX IX_Cliente_Nombre ON Clientes(Nombre);
CREATE INDEX IX_Cliente_Activo ON Clientes(Activo);

GO
