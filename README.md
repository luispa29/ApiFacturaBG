# ApiFacturaBG - Sistema de Facturación Backend

Este es el proyecto de API de backend para el sistema de facturación BG, desarrollado con .NET 8.0.

## Tecnologías Utilizadas

- .NET 8.0
- Web API de ASP.NET Core
- MapSql (Librería para manejo de base de datos)
- Autenticación JWT y Cifrado AES
- Swagger (Documentación de API)

## Prerrequisitos

- SDK de .NET 8.0 o superior
- SQL Server (Local o Remoto)
- Visual Studio 2022 o VS Code

## Configuración de la Base de Datos

Los scripts de base de datos se encuentran en la carpeta `Database/`. Para levantar la base de datos correctamente, siga este orden de ejecución:

1. Ejecute primero los scripts de creación de tablas:
   - `Tabla_Usuario.sql`
   - `Tabla_Cliente.sql`
   - `Tabla_Producto.sql`
   - `Tabla_FormaPago.sql`
   - `Tabla_Factura.sql`
   - `Tabla_FacturaDetalle.sql`
   - `Tabla_FacturaPago.sql`

2. Luego ejecute los scripts de validaciones:
   - `SP_Usuario_Validaciones.sql`
   - `SP_Cliente_Validaciones.sql`
   - `SP_Producto_Validaciones.sql`
   - `SP_FormaPago_Validaciones.sql`
   - `SP_Factura_Validaciones.sql`

3. Finalmente, ejecute los scripts de CRUD (Procedimientos Almacenados):
   - `SP_Usuario_CRUD.sql`
   - `SP_Cliente_CRUD.sql`
   - `SP_Producto_CRUD.sql`
   - `SP_FormaPago_CRUD.sql`
   - `SP_Factura_CRUD.sql`

## Configuración del Proyecto

1. Abra el archivo `appsettings.json` en el proyecto `ApiFacturaBG`.
2. Actualice la cadena de conexión en `AppSettings.DatabaseConnection` con sus credenciales de SQL Server si es necesario.
3. Asegúrese de que las claves de seguridad JWT y AES en la sección `Security` sean correctas para su entorno.

## Ejecución del Proyecto

Para ejecutar la API localmente, puede usar la terminal desde la raíz de la carpeta `ApiFacturaBG/ApiFacturaBG`:

```bash
dotnet run
```

O presione F5 en Visual Studio.

## Documentación de la API

Una vez iniciada la aplicación, puede acceder a la interfaz de Swagger para probar los endpoints en:

- URL: https://localhost:7092/swagger/index.html
- Alternativa: http://localhost:5127/swagger/index.html

## Postman

Se incluye una colección de Postman en la raíz del proyecto para facilitar las pruebas: `ApiFacturaBG.postman_collection.json`.
