CREATE TABLE dbo.Productos (
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    PrecioUnitario DECIMAL(18, 2) NOT NULL CHECK (PrecioUnitario >= 0),
    StockActual INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE INDEX IX_Producto_Nombre ON Productos(Nombre);
CREATE INDEX IX_Producto_Activo ON Productos(Activo);

GO
