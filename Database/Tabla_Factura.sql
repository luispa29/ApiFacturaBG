CREATE TABLE dbo.Facturas (
    FacturaID INT IDENTITY(1,1) PRIMARY KEY,
    NumeroFactura NVARCHAR(50) NOT NULL UNIQUE,
    ClienteID INT NOT NULL,
    VendedorID INT NOT NULL,
    FechaFactura DATETIME NOT NULL DEFAULT GETDATE(),
    Subtotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IVA DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_Factura_Cliente FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
    CONSTRAINT FK_Factura_Vendedor FOREIGN KEY (VendedorID) REFERENCES Usuarios(UsuarioID)
);

CREATE INDEX IX_Factura_NumeroFactura ON Facturas(NumeroFactura);
CREATE INDEX IX_Factura_ClienteID ON Facturas(ClienteID);
CREATE INDEX IX_Factura_VendedorID ON Facturas(VendedorID);
CREATE INDEX IX_Factura_FechaFactura ON Facturas(FechaFactura);
CREATE INDEX IX_Factura_Total ON Facturas(Total);
CREATE INDEX IX_Factura_Activo ON Facturas(Activo);

GO
