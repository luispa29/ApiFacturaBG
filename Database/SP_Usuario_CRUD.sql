

-- =============================================
-- SP: Crear Usuario
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_Crear
    @Username NVARCHAR(50),
    @Contrasena NVARCHAR(255),
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Activo BIT = 1,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Convertir la contraseña a VARBINARY 
    DECLARE @PasswordHash VARBINARY(MAX);
    SET @PasswordHash = CONVERT(VARBINARY(MAX), HASHBYTES('SHA2_256', @Contrasena));
    
    INSERT INTO dbo.Usuarios (Username, PasswordHash, Nombre, Email, Activo)
    VALUES (@Username, @PasswordHash, @Nombre, @Email, @Activo);
    
    SELECT SCOPE_IDENTITY() AS UsuarioID;
END
GO

-- =============================================
-- SP: Obtener Usuario por ID
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_ObtenerPorID
    @UsuarioID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UsuarioID,
        Username,
        Nombre,
        Email,
        Activo,
        FechaCreacion
    FROM dbo.Usuarios
    WHERE UsuarioID = @UsuarioID;
END
GO

-- =============================================
-- SP: Listar Usuarios
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_Listar
    @NumeroPagina INT = 1,
    @TamanoPagina INT = 10,
    @Filtro NVARCHAR(100) = NULL,
    @SoloActivos BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@NumeroPagina - 1) * @TamanoPagina;
    
    SELECT 
        UsuarioID,
        Username,
        Nombre,
        Email,
        Activo,
        FechaCreacion
    FROM dbo.Usuarios
    WHERE 
        (@Filtro IS NULL OR 
         Username LIKE '%' + @Filtro + '%' OR 
         Nombre LIKE '%' + @Filtro + '%' OR 
         Email LIKE '%' + @Filtro + '%')
        AND (@SoloActivos IS NULL OR Activo = @SoloActivos)
    ORDER BY UsuarioID DESC
    OFFSET @Offset ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;

    SELECT 
        COUNT(*) as TotalUsuarios,
        COUNT(CASE WHEN Activo = 1 THEN 1 END) as UsuariosActivos,
        COUNT(CASE WHEN Activo = 0 THEN 1 END) as UsuariosInactivos
    FROM dbo.Usuarios
    WHERE 
        (@Filtro IS NULL OR 
         Username LIKE '%' + @Filtro + '%' OR 
         Nombre LIKE '%' + @Filtro + '%' OR 
         Email LIKE '%' + @Filtro + '%')
        AND (@SoloActivos IS NULL OR Activo = @SoloActivos);

    SELECT COUNT(*) as TotalRegistros
    FROM dbo.Usuarios
    WHERE 
        (@Filtro IS NULL OR 
         Username LIKE '%' + @Filtro + '%' OR 
         Nombre LIKE '%' + @Filtro + '%' OR 
         Email LIKE '%' + @Filtro + '%')
        AND (@SoloActivos IS NULL OR Activo = @SoloActivos);
END
GO

-- =============================================
-- SP: Actualizar Usuario
-- =============================================

CREATE OR ALTER PROCEDURE SP_Usuario_Actualizar
    @Id INT,
    @Username NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Activo BIT,
    @ActualizarContrasena BIT = 0,
    @NuevaContrasena NVARCHAR(255) = NULL
AS
BEGIN
    
    IF @ActualizarContrasena = 1 AND @NuevaContrasena IS NOT NULL
    BEGIN
        DECLARE @PasswordHash VARBINARY(MAX);
        SET @PasswordHash = CONVERT(VARBINARY(MAX), HASHBYTES('SHA2_256', @NuevaContrasena));
        
        UPDATE dbo.Usuarios
        SET 
            Username = @Username,
            PasswordHash = @PasswordHash,
            Nombre = @Nombre,
            Email = @Email,
            Activo = @Activo
        WHERE UsuarioID = @Id;
    END
    ELSE
    BEGIN
        UPDATE dbo.Usuarios
        SET 
            Username = @Username,
            Nombre = @Nombre,
            Email = @Email,
            Activo = @Activo
        WHERE UsuarioID = @Id;
    END
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- =============================================
-- SP: Eliminar Usuario (Soft Delete)
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_Eliminar
    @UsuarioID INT,
    @EliminacionFisica BIT = 0
AS
BEGIN
    
    IF @EliminacionFisica = 1
    BEGIN
        DELETE FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID;
    END
    ELSE
    BEGIN
        UPDATE dbo.Usuarios
        SET Activo = 0
        WHERE UsuarioID = @UsuarioID;
    END
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

CREATE OR ALTER PROCEDURE SP_Usuario_ValidarCredenciales
    @Username NVARCHAR(50),
    @Contrasena NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PasswordHash VARBINARY(MAX);
    SET @PasswordHash = CONVERT(VARBINARY(MAX), HASHBYTES('SHA2_256', @Contrasena));
    
    SELECT 
        UsuarioID,
        Username,
        Nombre,
        Email,
        Activo,
        FechaCreacion
    FROM dbo.Usuarios
    WHERE Username = @Username 
      AND PasswordHash = @PasswordHash
      AND Activo = 1;
END
GO
