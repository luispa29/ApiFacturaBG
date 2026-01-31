using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Request;
using Models.Response;
using Models.Utils;
using System.Text.Json;

namespace Domain.UseCases
{
    public class FacturaUseCase(ISqlPort _sqlPort) : IFacturaService
    {
        private const decimal IVA_PORCENTAJE = 0;

        public async Task<(int id, string mensaje)> CrearFactura(FacturaRequest factura)
        {
            try
            {
                string validacionErrores = ValidarFacturaRequest(factura);

                if (validacionErrores.Length > 0)
                {
                    return (0, validacionErrores);
                }

                var existeCliente = await ValidarClienteExistente(factura.ClienteID);
                var existeVendedor = await ValidarVendedorExistente(factura.VendedorID);
                var productosInvalidos = await ValidarProductosExistentes(factura.Detalles);
                var formasPagoInvalidas = await ValidarFormasPagoExistentes(factura.Pagos);

                if (existeCliente.Length > 0 || existeVendedor.Length > 0 || productosInvalidos.Length > 0 || formasPagoInvalidas.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeCliente, existeVendedor, productosInvalidos, formasPagoInvalidas }.Where(e => e.Length > 0)));
                }

                var (subtotal, iva, total, detallesCalculados) = CalcularTotalesFactura(factura.Detalles);

                var validacionPagos = ValidarSumaPagos(factura.Pagos, total);
                if (validacionPagos.Length > 0)
                {
                    return (0, validacionPagos);
                }

                var detallesJSON = JsonSerializer.Serialize(detallesCalculados);
                var pagosJSON = JsonSerializer.Serialize(factura.Pagos);

                var parametros = new
                {
                    factura.ClienteID,
                    factura.VendedorID,
                    factura.FechaFactura,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total,
                    DetallesJSON = detallesJSON,
                    PagosJSON = pagosJSON
                };

                var resultTypes = new Type[] { typeof(FacturaCreadaResponse) };
                var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync("SP_Factura_Crear", resultTypes, parametros);
                var facturaCreada = results[0].Cast<FacturaCreadaResponse>().FirstOrDefault();

                if (facturaCreada != null && facturaCreada.FacturaID > 0)
                {
                    return (facturaCreada.FacturaID, $"Factura {facturaCreada.NumeroFactura} creada exitosamente.");
                }
                else
                {
                    return (0, "No se pudo crear la factura.");
                }
            }
            catch (Exception ex)
            {
                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }

        public async Task<FacturaResponse?> ObtenerPorID(int facturaID)
        {
            try
            {
                var parameter = new { FacturaID = facturaID };
                var resultTypes = new[] { typeof(FacturaResponse), typeof(FacturaDetalleResponse), typeof(FacturaPagoResponse) };

                var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync("SP_Factura_ObtenerPorID", resultTypes, parameter);

                var factura = results[0].Cast<FacturaResponse>().FirstOrDefault();

                if (factura != null)
                {
                    factura.Detalles = results[1].Cast<FacturaDetalleResponse>().ToList();
                    factura.Pagos = results[2].Cast<FacturaPagoResponse>().ToList();
                }

                return factura;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(IEnumerable<FacturaResponse> facturas, int total)> ListarFacturas(FacturaFiltroRequest filtros)
        {
            try
            {
                var parametros = new
                {
                    filtros.NumeroFactura,
                    filtros.ClienteID,
                    filtros.VendedorID,
                    filtros.FechaDesde,
                    filtros.FechaHasta,
                    filtros.MontoDesde,
                    filtros.MontoHasta,
                    NumeroPagina = filtros.Pagina,
                    TamanoPagina = filtros.RegistrosPorPagina
                };

                var resultTypes = new[] { typeof(FacturaResponse), typeof(TotalRegistros) };
                var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync("SP_Factura_Listar", resultTypes, parametros);

                var facturas = results[0].Cast<FacturaResponse>();
                var total = results[1].Cast<TotalRegistros>().FirstOrDefault()?.Total ?? 0;

                return (facturas, total);
            }
            catch (Exception)
            {
                return (Enumerable.Empty<FacturaResponse>(), 0);
            }
        }

        private static string ValidarFacturaRequest(FacturaRequest factura)
        {
            var errores = new List<string>();

            if (factura == null)
            {
                return "El objeto FacturaRequest no puede ser nulo";
            }

            if (factura.ClienteID <= 0)
            {
                errores.Add("El campo ClienteID es requerido");
            }

            if (factura.VendedorID <= 0)
            {
                errores.Add("El campo VendedorID es requerido");
            }

            if (factura.FechaFactura == default)
            {
                errores.Add("El campo FechaFactura es requerido");
            }

            if (factura.Detalles == null || factura.Detalles.Count == 0)
            {
                errores.Add("La factura debe tener al menos un detalle");
            }
            else
            {
                for (int i = 0; i < factura.Detalles.Count; i++)
                {
                    var detalle = factura.Detalles[i];
                    if (detalle.ProductoID <= 0)
                    {
                        errores.Add($"Detalle {i + 1}: ProductoID es requerido");
                    }
                    if (detalle.Cantidad <= 0)
                    {
                        errores.Add($"Detalle {i + 1}: Cantidad debe ser mayor a 0");
                    }
                    if (detalle.PrecioUnitario < 0)
                    {
                        errores.Add($"Detalle {i + 1}: PrecioUnitario no puede ser negativo");
                    }
                }
            }

            if (factura.Pagos == null || factura.Pagos.Count == 0)
            {
                errores.Add("La factura debe tener al menos una forma de pago");
            }
            else
            {
                for (int i = 0; i < factura.Pagos.Count; i++)
                {
                    var pago = factura.Pagos[i];
                    if (pago.FormaPagoID <= 0)
                    {
                        errores.Add($"Pago {i + 1}: FormaPagoID es requerido");
                    }
                    if (pago.Monto <= 0)
                    {
                        errores.Add($"Pago {i + 1}: Monto debe ser mayor a 0");
                    }
                }
            }

            return string.Join(", ", errores);
        }

        private async Task<string> ValidarClienteExistente(int clienteID)
        {
            var parameter = new { ClienteID = clienteID };
            
            var cliente = await _sqlPort.ExecuteStoredProcedureSingleAsync<ClienteResponse>("SP_Cliente_ExistePorID", parameter);

            if (cliente == null)
            {
                return $"Cliente ID {clienteID} no existe";
            }
            
            if (!cliente.Activo)
            {
                return $"Cliente '{cliente.Nombre}' (ID {clienteID}) está inactivo";
            }

            return string.Empty;
        }

        private async Task<string> ValidarVendedorExistente(int vendedorID)
        {
            var parameter = new { UsuarioID = vendedorID };
            
            var vendedor = await _sqlPort.ExecuteStoredProcedureSingleAsync<UsuarioResponse>("SP_Usuario_ExistePorID", parameter);

            if (vendedor == null)
            {
                return $"Vendedor ID {vendedorID} no existe";
            }
            
            if (!vendedor.Activo)
            {
                return $"Vendedor '{vendedor.Nombre}' (ID {vendedorID}) está inactivo";
            }

            return string.Empty;
        }

        private async Task<string> ValidarProductosExistentes(List<FacturaDetalleRequest> detalles)
        {
            var productosIDs = detalles.Select(d => d.ProductoID).Distinct().ToList();
            var productosJSON = JsonSerializer.Serialize(productosIDs);

            var parameter = new { ProductosJSON = productosJSON };
            var resultTypes = new Type[] { typeof(ProductoInvalidoResponse) };
            
            var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync("SP_Factura_ValidarProductos", resultTypes, parameter);
            var productosInvalidos = results[0].Cast<ProductoInvalidoResponse>().ToList();

            if (productosInvalidos.Any())
            {
                var mensajes = productosInvalidos.Select(p =>
                {
                    if (p.Nombre == null)
                        return $"Producto ID {p.ProductoID} no existe";
                    else if (p.Activo == false)
                        return $"Producto '{p.Nombre}' (ID {p.ProductoID}) está inactivo";
                    else
                        return $"Producto ID {p.ProductoID} es inválido";
                });
                
                return string.Join(", ", mensajes);
            }

            return string.Empty;
        }

        private async Task<string> ValidarFormasPagoExistentes(List<FacturaPagoRequest> pagos)
        {
            var formasPagoIDs = pagos.Select(p => p.FormaPagoID).Distinct().ToList();
            var formasPagoJSON = JsonSerializer.Serialize(formasPagoIDs);

            var parameter = new { FormasPagoJSON = formasPagoJSON };
            var resultTypes = new Type[] { typeof(FormaPagoInvalidaResponse) };
            
            var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync("SP_FormaPago_ValidarMultiples", resultTypes, parameter);
            var formasPagoInvalidas = results[0].Cast<FormaPagoInvalidaResponse>().ToList();

            if (formasPagoInvalidas.Any())
            {
                var mensajes = formasPagoInvalidas.Select(f =>
                {
                    if (f.Nombre == null)
                        return $"Forma de pago ID {f.FormaPagoID} no existe";
                    else if (f.Activo == false)
                        return $"Forma de pago '{f.Nombre}' (ID {f.FormaPagoID}) está inactiva";
                    else
                        return $"Forma de pago ID {f.FormaPagoID} es inválida";
                });
                
                return string.Join(", ", mensajes);
            }

            return string.Empty;
        }

        private static (decimal subtotal, decimal iva, decimal total, List<DetalleCalculado> detalles) CalcularTotalesFactura(List<FacturaDetalleRequest> detalles)
        {
            decimal subtotalTotal = 0;
            decimal ivaTotal = 0;
            decimal totalTotal = 0;
            var detallesCalculados = new List<DetalleCalculado>();

            foreach (var detalle in detalles)
            {
                decimal subtotalDetalle = detalle.Cantidad * detalle.PrecioUnitario;
                decimal ivaDetalle = subtotalDetalle * IVA_PORCENTAJE;
                decimal totalDetalle = subtotalDetalle + ivaDetalle;

                detallesCalculados.Add(new DetalleCalculado
                {
                    ProductoID = detalle.ProductoID,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Subtotal = subtotalDetalle,
                    IVA = ivaDetalle,
                    Total = totalDetalle
                });

                subtotalTotal += subtotalDetalle;
                ivaTotal += ivaDetalle;
                totalTotal += totalDetalle;
            }

            return (subtotalTotal, ivaTotal, totalTotal, detallesCalculados);
        }

        private static string ValidarSumaPagos(List<FacturaPagoRequest> pagos, decimal totalFactura)
        {
            decimal sumaPagos = pagos.Sum(p => p.Monto);

            if (sumaPagos != totalFactura)
            {
                return $"La suma de los pagos ({sumaPagos:F2}) no coincide con el total de la factura ({totalFactura:F2})";
            }

            return string.Empty;
        }

    }
}
