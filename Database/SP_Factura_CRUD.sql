CREATE OR ALTER PROCEDURE SP_Factura_Crear
    @ClienteID INT,
    @VendedorID INT,
    @FechaFactura DATETIME,
    @Subtotal DECIMAL(18, 2),
    @IVA DECIMAL(18, 2),
    @Total DECIMAL(18, 2),
    @DetallesJSON NVARCHAR(MAX),
    @PagosJSON NVARCHAR(MAX)
AS
BEGIN
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @NumeroFactura NVARCHAR(50);
        DECLARE @FacturaID INT;
        DECLARE @TempNumero TABLE (NumeroFactura NVARCHAR(50));
        
        INSERT INTO @TempNumero
        EXEC SP_Factura_GenerarNumero;
        
        SELECT @NumeroFactura = NumeroFactura FROM @TempNumero;
        
        INSERT INTO dbo.Facturas (
            NumeroFactura,
            ClienteID,
            VendedorID,
            FechaFactura,
            Subtotal,
            IVA,
            Total
        )
        VALUES (
            @NumeroFactura,
            @ClienteID,
            @VendedorID,
            @FechaFactura,
            @Subtotal,
            @IVA,
            @Total
        );
        
        SET @FacturaID = SCOPE_IDENTITY();
        
        INSERT INTO dbo.FacturaDetalles (
            FacturaID,
            ProductoID,
            Cantidad,
            PrecioUnitario,
            Subtotal,
            IVA,
            Total
        )
        SELECT 
            @FacturaID,
            ProductoID,
            Cantidad,
            PrecioUnitario,
            Subtotal,
            IVA,
            Total
        FROM OPENJSON(@DetallesJSON)
        WITH (
            ProductoID INT '$.ProductoID',
            Cantidad INT '$.Cantidad',
            PrecioUnitario DECIMAL(18, 2) '$.PrecioUnitario',
            Subtotal DECIMAL(18, 2) '$.Subtotal',
            IVA DECIMAL(18, 2) '$.IVA',
            Total DECIMAL(18, 2) '$.Total'
        );
        
        INSERT INTO dbo.FacturaPagos (
            FacturaID,
            FormaPagoID,
            Monto,
            Referencia
        )
        SELECT 
            @FacturaID,
            FormaPagoID,
            Monto,
            Referencia
        FROM OPENJSON(@PagosJSON)
        WITH (
            FormaPagoID INT '$.FormaPagoID',
            Monto DECIMAL(18, 2) '$.Monto',
            Referencia NVARCHAR(100) '$.Referencia'
        );
        
        COMMIT TRANSACTION;
        
        SELECT @FacturaID AS FacturaID, @NumeroFactura AS NumeroFactura;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
