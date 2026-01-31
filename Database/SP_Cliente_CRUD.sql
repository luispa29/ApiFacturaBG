CREATE OR ALTER PROCEDURE SP_Cliente_Crear
    @Identificacion NVARCHAR(20),
    @Nombre NVARCHAR(150),
    @Telefono NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Id INT = NULL
AS
BEGIN
    
    INSERT INTO dbo.Clientes (Identificacion, Nombre, Telefono, Email)
    VALUES (@Identificacion, @Nombre, @Telefono, @Email);
    
    SELECT SCOPE_IDENTITY() AS ClienteID;
END
GO

CREATE OR ALTER PROCEDURE SP_Cliente_Actualizar
    @Id INT,
    @Identificacion NVARCHAR(20),
    @Nombre NVARCHAR(150),
    @Telefono NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL
AS
BEGIN
    
    UPDATE dbo.Clientes
    SET 
        Identificacion = @Identificacion,
        Nombre = @Nombre,
        Telefono = @Telefono,
        Email = @Email
    WHERE ClienteID = @Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

CREATE OR ALTER PROCEDURE SP_Cliente_Eliminar
    @ClienteID INT,
    @EliminacionFisica BIT = 0
AS
BEGIN
    
    IF @EliminacionFisica = 1
    BEGIN
        DELETE FROM dbo.Clientes WHERE ClienteID = @ClienteID;
    END
    ELSE
    BEGIN
        UPDATE dbo.Clientes
        SET Activo = 0
        WHERE ClienteID = @ClienteID;
    END
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO
