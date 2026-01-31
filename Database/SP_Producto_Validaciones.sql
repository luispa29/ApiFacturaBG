CREATE OR ALTER PROCEDURE SP_Producto_ExistePorNombre
    @Nombre NVARCHAR(200),
    @ProductoIDExcluir INT = NULL
AS
BEGIN
    
    IF @ProductoIDExcluir IS NULL
    BEGIN
        SELECT ProductoID
        FROM dbo.Productos 
        WHERE Nombre = @Nombre;
    END
    ELSE
    BEGIN
        SELECT ProductoID
        FROM dbo.Productos 
        WHERE Nombre = @Nombre AND ProductoID <> @ProductoIDExcluir;
    END
END
GO

CREATE OR ALTER PROCEDURE SP_Producto_ExistePorID
    @ProductoID INT
AS
BEGIN
    
    SELECT ProductoID
    FROM dbo.Productos 
    WHERE ProductoID = @ProductoID;
END
GO
