CREATE TABLE dbo.FacturaPagos (
    PagoID INT IDENTITY(1,1) PRIMARY KEY,
    FacturaID INT NOT NULL,
    FormaPagoID INT NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL CHECK (Monto > 0),
    Referencia NVARCHAR(100) NULL,
    
    CONSTRAINT FK_FacturaPago_Factura FOREIGN KEY (FacturaID) REFERENCES Facturas(FacturaID) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaPago_FormaPago FOREIGN KEY (FormaPagoID) REFERENCES FormasPago(FormaPagoID)
);

CREATE INDEX IX_FacturaPago_FacturaID ON FacturaPagos(FacturaID);
CREATE INDEX IX_FacturaPago_FormaPagoID ON FacturaPagos(FormaPagoID);

GO
