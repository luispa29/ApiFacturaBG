CREATE OR ALTER PROCEDURE SP_Cliente_ExistePorIdentificacion
    @Identificacion NVARCHAR(20),
    @ClienteIDExcluir INT = NULL
AS
BEGIN
    
    IF @ClienteIDExcluir IS NULL
    BEGIN
        SELECT ClienteID
        FROM dbo.Clientes 
        WHERE Identificacion = @Identificacion;
    END
    ELSE
    BEGIN
        SELECT ClienteID
        FROM dbo.Clientes 
        WHERE Identificacion = @Identificacion AND ClienteID <> @ClienteIDExcluir;
    END
END
GO

CREATE OR ALTER PROCEDURE SP_Cliente_ExistePorEmail
    @Email NVARCHAR(100),
    @ClienteIDExcluir INT = NULL
AS
BEGIN
    
    IF @ClienteIDExcluir IS NULL
    BEGIN
        SELECT ClienteID
        FROM dbo.Clientes 
        WHERE Email = @Email;
    END
    ELSE
    BEGIN
        SELECT ClienteID
        FROM dbo.Clientes 
        WHERE Email = @Email AND ClienteID <> @ClienteIDExcluir;
    END
END
GO

CREATE OR ALTER PROCEDURE SP_Cliente_ExistePorID
    @ClienteID INT
AS
BEGIN
    
    SELECT ClienteID,
        Identificacion,
        Nombre,
        Telefono,
        Email,
        FechaRegistro,
        Activo
    FROM dbo.Clientes 
    WHERE ClienteID = @ClienteID;
END
GO
