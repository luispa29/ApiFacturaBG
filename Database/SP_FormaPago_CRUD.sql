CREATE OR ALTER PROCEDURE SP_FormaPago_ListarActivos
AS
BEGIN
    SELECT 
        FormaPagoID,
        Nombre,
        Activo
    FROM dbo.FormasPago
    WHERE Activo = 1
    ORDER BY Nombre ASC;
END
GO
