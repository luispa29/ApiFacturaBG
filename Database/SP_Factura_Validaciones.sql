CREATE OR ALTER PROCEDURE SP_Factura_ExistePorNumero
    @NumeroFactura NVARCHAR(50),
    @FacturaIDExcluir INT = NULL
AS
BEGIN
    
    IF @FacturaIDExcluir IS NULL
    BEGIN
        SELECT FacturaID
        FROM dbo.Facturas 
        WHERE NumeroFactura = @NumeroFactura;
    END
    ELSE
    BEGIN
        SELECT FacturaID
        FROM dbo.Facturas 
        WHERE NumeroFactura = @NumeroFactura AND FacturaID <> @FacturaIDExcluir;
    END
END
GO

CREATE OR ALTER PROCEDURE SP_Factura_ExistePorID
    @FacturaID INT
AS
BEGIN
    
    SELECT FacturaID
    FROM dbo.Facturas 
    WHERE FacturaID = @FacturaID;
END
GO

CREATE OR ALTER PROCEDURE SP_Factura_GenerarNumero
AS
BEGIN
    
    DECLARE @Fecha NVARCHAR(8) = CONVERT(NVARCHAR(8), GETDATE(), 112);
    DECLARE @Prefijo NVARCHAR(20) = 'FAC-' + @Fecha + '-';
    DECLARE @UltimoNumero INT;
    DECLARE @NuevoNumero NVARCHAR(50);
    
    SELECT @UltimoNumero = ISNULL(MAX(CAST(RIGHT(NumeroFactura, 4) AS INT)), 0)
    FROM dbo.Facturas
    WHERE NumeroFactura LIKE @Prefijo + '%';
    
    SET @NuevoNumero = @Prefijo + RIGHT('0000' + CAST(@UltimoNumero + 1 AS NVARCHAR(4)), 4);
    
    SELECT @NuevoNumero AS NumeroFactura;
END
GO

CREATE OR ALTER PROCEDURE SP_Factura_ValidarProductos
    @ProductosJSON NVARCHAR(MAX)
AS
BEGIN
    
    SELECT p.ProductoID, p.Nombre, p.Activo
    FROM OPENJSON(@ProductosJSON)
    WITH (ProductoID INT '$') AS ids
    LEFT JOIN dbo.Productos p ON ids.ProductoID = p.ProductoID
    WHERE p.ProductoID IS NULL OR p.Activo = 0;
END
GO
