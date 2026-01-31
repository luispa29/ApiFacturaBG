CREATE TABLE dbo.FacturaDetalles (
    DetalleID INT IDENTITY(1,1) PRIMARY KEY,
    FacturaID INT NOT NULL,
    ProductoID INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18, 2) NOT NULL CHECK (PrecioUnitario >= 0),
    Subtotal DECIMAL(18, 2) NOT NULL,
    IVA DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL,
    
    CONSTRAINT FK_FacturaDetalle_Factura FOREIGN KEY (FacturaID) REFERENCES Facturas(FacturaID) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaDetalle_Producto FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID)
);

CREATE INDEX IX_FacturaDetalle_FacturaID ON FacturaDetalles(FacturaID);
CREATE INDEX IX_FacturaDetalle_ProductoID ON FacturaDetalles(ProductoID);

GO
