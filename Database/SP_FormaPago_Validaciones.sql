CREATE OR ALTER PROCEDURE SP_FormaPago_ValidarMultiples
    @FormasPagoJSON NVARCHAR(MAX)
AS
BEGIN
    
    SELECT fp.FormaPagoID, fp.Nombre, fp.Activo
    FROM OPENJSON(@FormasPagoJSON)
    WITH (FormaPagoID INT '$') AS ids
    LEFT JOIN dbo.FormasPago fp ON ids.FormaPagoID = fp.FormaPagoID
    WHERE fp.FormaPagoID IS NULL OR fp.Activo = 0;
END
GO
