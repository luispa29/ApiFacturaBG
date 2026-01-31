CREATE TABLE dbo.FormasPago (
    FormaPagoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE INDEX IX_FormaPago_Nombre ON FormasPago(Nombre);
CREATE INDEX IX_FormaPago_Activo ON FormasPago(Activo);

INSERT INTO FormasPago (Nombre, Descripcion) VALUES 
('Efectivo', 'Pago en efectivo'),
('Tarjeta Crédito', 'Pago con tarjeta de crédito'),
('Tarjeta Débito', 'Pago con tarjeta de débito'),
('Transferencia', 'Transferencia bancaria'),
('Cheque', 'Pago con cheque');

GO
