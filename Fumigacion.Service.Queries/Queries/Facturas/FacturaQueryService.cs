using Fumigacion.Domain.DFacturas;
using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.Facturas;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.Facturas
{
    public interface IFacturasQueryService
    {
        Task<List<FacturaDto>> GetAllFacturas();
        Task<List<FacturaDto>> GetAllFacturasAsync(int repositorio);
        Task<List<FacturaDto>> GetFacturasByInmuebleAsync(int inmueble, int facturacion);
        Task<List<ConceptoFacturaDto>> GetConceptosFacturaByIdAsync(int factura);
        Task<List<HistorialMFDto>> GetHistorialMFByFacturacion(int facturacion);
        Task<List<FacturaDto>> GetFacturasNCPendientes(int estatus);
        Task<int> GetFacturasCargadasAsync(int facturacion);
        Task<int> GetNotasCreditoCargadasAsync(int facturacion);
        Task<int> GetTotalFacturasByInmuebleAsync(int facturacion, int inmueble);
        Task<int> GetNCByInmuebleAsync(int facturacion, int inmueble);
    }

    public class FacturaQueryService : IFacturasQueryService
    {
        private readonly ApplicationDbContext _context;

        public FacturaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FacturaDto>> GetAllFacturas()
        {
            try
            {
                var facturas = await _context.Facturas.Where(f => !f.FechaEliminacion.HasValue).ToListAsync();

                return facturas.MapTo<List<FacturaDto>>();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<FacturaDto>> GetAllFacturasAsync(int repositorio)
        {
            try
            {
                var facturas = await _context.Facturas.Where(f => f.RepositorioId == repositorio && !f.FechaEliminacion.HasValue).ToListAsync();

                return facturas.MapTo<List<FacturaDto>>();
            } 
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<FacturaDto>> GetFacturasByInmuebleAsync(int inmueble, int facturacion)
        {
            var collection = await _context.Facturas.Where(x => x.InmuebleId == inmueble && x.RepositorioId == facturacion 
                                                            && !x.FechaEliminacion.HasValue).OrderBy(x => x.InmuebleId).ToListAsync();

            return collection.MapTo<List<FacturaDto>>();
        }

        public async Task<List<ConceptoFacturaDto>> GetConceptosFacturaByIdAsync(int factura)
        {
            var conceptos = await _context.ConceptosFactura.Where(x => x.FacturaId == factura)
                                            .GroupBy(c => new {
                                                c.FacturaId,
                                                c.ClaveProducto,
                                                c.ClaveUnidad,
                                                c.Unidad,
                                                c.Descripcion,
                                                c.PrecioUnitario
                                            })
                                            .Select(cf => new ConceptoFacturaDto {
                                                FacturaId = cf.Key.FacturaId,
                                                Cantidad = cf.Sum(c => c.Cantidad),
                                                ClaveProducto = cf.Key.ClaveProducto,
                                                ClaveUnidad = cf.Key.ClaveUnidad,
                                                Unidad = cf.Key.Unidad,
                                                Descripcion = cf.Key.Descripcion,
                                                PrecioUnitario = cf.Key.PrecioUnitario,
                                                Subtotal = cf.Sum(sb => sb.Subtotal),
                                                Descuento = cf.Sum(sb => sb.Descuento),
                                                IVA= (cf.Sum(sb => sb.Subtotal)*Convert.ToDecimal(0.16))
                                            })
                                            .ToListAsync();

            return conceptos.MapTo<List<ConceptoFacturaDto>>();
        }

        public async Task<List<HistorialMFDto>> GetHistorialMFByFacturacion(int facturacion)
        {
            var historial = await _context.HistorialMF.Where(hf => hf.RepositorioId == facturacion).OrderBy(i => i.InmuebleId).ToListAsync();
            return historial.MapTo<List<HistorialMFDto>>();
        }

        public async Task<List<FacturaDto>> GetFacturasNCPendientes(int estatus)
        {
            var collection = await _context.Facturas.Where(f => f.EstatusId == estatus && !f.FechaEliminacion.HasValue)
                             .OrderBy(x => x.InmuebleId).ToListAsync();

            return collection.MapTo<List<FacturaDto>>();
        }

        public async Task<int> GetFacturasCargadasAsync(int facturacion)
        {
            var collection = await _context.Facturas.Where(x => x.RepositorioId == facturacion && x.Tipo.Equals("Factura")
                                                    && !x.FechaEliminacion.HasValue).OrderBy(x => x.InmuebleId).ToListAsync();

            int facturas = collection.Count(x => x.Total > 0);

            return facturas;
        }

        public async Task<int> GetNotasCreditoCargadasAsync(int facturacion)
        {
            var collection = await _context.Facturas.Where(x => x.RepositorioId == facturacion && x.Tipo.Equals("NC") && !x.FechaEliminacion.HasValue)
                                .OrderBy(x => x.InmuebleId).ToListAsync();

            int facturas = collection.Count(x => x.Total > 0);

            return facturas;
        }

        public async Task<int> GetTotalFacturasByInmuebleAsync(int facturacion, int inmueble)
        {
            var collection = await _context.Facturas
                .Where(x => x.RepositorioId == facturacion && x.InmuebleId == inmueble && x.Tipo.Equals("Factura")
                        && !x.FechaEliminacion.HasValue).OrderBy(x => x.InmuebleId).ToListAsync();

            int facturas = collection.Count(x => x.Total > 0);

            return facturas;
        }

        public async Task<int> GetNCByInmuebleAsync(int facturacion, int inmueble)
        {
            var collection = await _context.Facturas.Where(x => x.RepositorioId == facturacion && x.InmuebleId == inmueble && x.Tipo.Equals("NC")
                && !x.FechaEliminacion.HasValue).OrderBy(x => x.InmuebleId).ToListAsync();

            int facturas = collection.Count(x => x.Total > 0);

            return facturas;
        }

    }
}
