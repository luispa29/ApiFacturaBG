
CREATE OR ALTER PROCEDURE SP_Usuario_ExistePorUsername
    @Username NVARCHAR(50),
    @UsuarioIDExcluir INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @UsuarioIDExcluir IS NULL
    BEGIN
        SELECT UsuarioID
        FROM dbo.Usuarios 
        WHERE Username = @Username;
    END
    ELSE
    BEGIN
        SELECT UsuarioID
        FROM dbo.Usuarios 
        WHERE Username = @Username AND UsuarioID <> @UsuarioIDExcluir;
    END
END
GO

-- =============================================
-- SP: Verificar si existe un usuario por correo electrónico
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_ExistePorEmail
    @Email NVARCHAR(100),
    @UsuarioIDExcluir INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @UsuarioIDExcluir IS NULL
    BEGIN
        SELECT UsuarioID
        FROM dbo.Usuarios 
        WHERE Email = @Email;
    END
    ELSE
    BEGIN
        SELECT UsuarioID
        FROM dbo.Usuarios 
        WHERE Email = @Email AND UsuarioID <> @UsuarioIDExcluir;
    END
END
GO

-- =============================================
-- SP: Verificar si existe un usuario por ID
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_ExistePorID
    @UsuarioID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT UsuarioID
    FROM dbo.Usuarios 
    WHERE UsuarioID = @UsuarioID;
END
GO

-- =============================================
-- SP: Validar si existe un usuario y retornar su ID
-- =============================================
CREATE OR ALTER PROCEDURE SP_Usuario_ValidarExistencia
    @Username NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UsuarioID,
        Username,
        Email,
        Nombre,
        Activo
    FROM dbo.Usuarios
    WHERE (@Username IS NOT NULL AND Username = @Username)
       OR (@Email IS NOT NULL AND Email = @Email);
END
GO
