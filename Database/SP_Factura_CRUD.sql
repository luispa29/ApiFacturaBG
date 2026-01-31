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

CREATE OR ALTER PROCEDURE SP_Factura_ObtenerPorID
    @FacturaID INT
AS
BEGIN
    -- 1. Encabezado de la factura
    SELECT 
        f.FacturaID,
        f.NumeroFactura,
        f.ClienteID,
        c.Nombre AS ClienteNombre,
        c.Identificacion AS ClienteIdentificacion,
        f.VendedorID,
        u.Nombre AS VendedorNombre,
        f.FechaFactura,
        f.Subtotal,
        f.IVA,
        f.Total,
        f.Activo,
        f.FechaCreacion
    FROM dbo.Facturas f
    INNER JOIN dbo.Clientes c ON f.ClienteID = c.ClienteID
    INNER JOIN dbo.Usuarios u ON f.VendedorID = u.UsuarioID
    WHERE f.FacturaID = @FacturaID;

    -- 2. Detalles de la factura
    SELECT 
        fd.DetalleID,
        fd.FacturaID,
        fd.ProductoID,
        p.Nombre AS ProductoNombre,
        fd.Cantidad,
        fd.PrecioUnitario,
        fd.Subtotal,
        fd.IVA,
        fd.Total
    FROM dbo.FacturaDetalles fd
    INNER JOIN dbo.Productos p ON fd.ProductoID = p.ProductoID
    WHERE fd.FacturaID = @FacturaID;

    -- 3. Pagos de la factura
    SELECT 
        fp.PagoID,
        fp.FacturaID,
        fp.FormaPagoID,
        fmp.Nombre AS FormaPagoNombre,
        fp.Monto,
        fp.Referencia
    FROM dbo.FacturaPagos fp
    INNER JOIN dbo.FormasPago fmp ON fp.FormaPagoID = fmp.FormaPagoID
    WHERE fp.FacturaID = @FacturaID;
END
GO

CREATE OR ALTER PROCEDURE SP_Factura_Listar
    @NumeroFactura NVARCHAR(50) = NULL,
    @ClienteID INT = NULL,
    @VendedorID INT = NULL,
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL,
    @NumeroPagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    DECLARE @Offset INT = (@NumeroPagina - 1) * @TamanoPagina;

    SELECT 
        f.FacturaID,
        f.NumeroFactura,
        f.ClienteID,
        c.Nombre AS ClienteNombre,
        f.VendedorID,
        u.Nombre AS VendedorNombre,
        f.FechaFactura,
        f.Subtotal,
        f.IVA,
        f.Total,
        f.Activo,
        f.FechaCreacion
    FROM dbo.Facturas f
    INNER JOIN dbo.Clientes c ON f.ClienteID = c.ClienteID
    INNER JOIN dbo.Usuarios u ON f.VendedorID = u.UsuarioID
    WHERE (@NumeroFactura IS NULL OR f.NumeroFactura LIKE '%' + @NumeroFactura + '%')
      AND (@ClienteID IS NULL OR f.ClienteID = @ClienteID)
      AND (@VendedorID IS NULL OR f.VendedorID = @VendedorID)
      AND (@FechaDesde IS NULL OR f.FechaFactura >= @FechaDesde)
      AND (@FechaHasta IS NULL OR f.FechaFactura <= @FechaHasta)
    ORDER BY f.FechaFactura DESC, f.FacturaID DESC
    OFFSET @Offset ROWS FETCH NEXT @TamanoPagina ROWS ONLY;

    SELECT COUNT(*) AS Total
    FROM dbo.Facturas f
    WHERE (@NumeroFactura IS NULL OR f.NumeroFactura LIKE '%' + @NumeroFactura + '%')
      AND (@ClienteID IS NULL OR f.ClienteID = @ClienteID)
      AND (@VendedorID IS NULL OR f.VendedorID = @VendedorID)
      AND (@FechaDesde IS NULL OR f.FechaFactura >= @FechaDesde)
      AND (@FechaHasta IS NULL OR f.FechaFactura <= @FechaHasta);
END
GO
