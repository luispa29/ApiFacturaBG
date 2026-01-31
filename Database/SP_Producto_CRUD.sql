CREATE OR ALTER PROCEDURE SP_Producto_Crear
    @Nombre NVARCHAR(200),
    @PrecioUnitario DECIMAL(18, 2),
    @StockActual INT = 0,
    @Id INT = NULL
AS
BEGIN
    
    INSERT INTO dbo.Productos (Nombre, PrecioUnitario, StockActual)
    VALUES (@Nombre, @PrecioUnitario, @StockActual);
    
    SELECT SCOPE_IDENTITY() AS ProductoID;
END
GO

CREATE OR ALTER PROCEDURE SP_Producto_ObtenerPorID
    @ProductoID INT
AS
BEGIN
    
    SELECT 
        ProductoID,
        Nombre,
        PrecioUnitario,
        StockActual
    FROM dbo.Productos
    WHERE ProductoID = @ProductoID;
END
GO

CREATE OR ALTER PROCEDURE SP_Producto_Listar
    @NumeroPagina INT = 1,
    @TamanoPagina INT = 10,
    @Filtro NVARCHAR(200) = NULL,
    @SoloActivos BIT = NULL
AS
BEGIN
    
    DECLARE @Offset INT = (@NumeroPagina - 1) * @TamanoPagina;
    
    SELECT 
        ProductoID,
        Nombre,
        PrecioUnitario,
        StockActual,
        Activo
    FROM dbo.Productos
    WHERE 
        (@Filtro IS NULL OR 
         Nombre LIKE '%' + @Filtro + '%')
        AND (@SoloActivos IS NULL OR Activo = @SoloActivos)
    ORDER BY ProductoID DESC
    OFFSET @Offset ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;

   
    SELECT COUNT(*) as Total
    FROM dbo.Productos
    WHERE 
        (@Filtro IS NULL OR 
         Nombre LIKE '%' + @Filtro + '%')
        AND (@SoloActivos IS NULL OR Activo = @SoloActivos);
END
GO

CREATE OR ALTER PROCEDURE SP_Producto_Actualizar
    @Id INT,
    @Nombre NVARCHAR(200),
    @PrecioUnitario DECIMAL(18, 2),
    @StockActual INT
AS
BEGIN
    
    UPDATE dbo.Productos
    SET 
        Nombre = @Nombre,
        PrecioUnitario = @PrecioUnitario,
        StockActual = @StockActual
    WHERE ProductoID = @Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

CREATE OR ALTER PROCEDURE SP_Producto_Eliminar
    @ProductoID INT,
    @EliminacionFisica BIT = 0
AS
BEGIN
    
    IF @EliminacionFisica = 1
    BEGIN
        DELETE FROM dbo.Productos WHERE ProductoID = @ProductoID;
    END
    ELSE
    BEGIN
        UPDATE dbo.Productos
        SET Activo = 0
        WHERE ProductoID = @ProductoID;
    END
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO
