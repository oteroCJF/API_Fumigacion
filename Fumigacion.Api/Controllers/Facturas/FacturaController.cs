using Fumigacion.Service.EventHandler.Commands.Facturas;
using Fumigacion.Service.Queries.DTOs.Facturas;
using Fumigacion.Service.Queries.Queries.Facturas;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fumigacion.Service.EventHandler.Commands.CFDIs;
using System.IO;

namespace Fumigacion.Api.Controllers.Facturas
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/facturas")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturasQueryService _facturas;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public FacturaController(IFacturasQueryService facturas, IMediator mediator, IHostingEnvironment environment)
        {
            _facturas = facturas;
            _mediator = mediator;
            _environment = environment;
        }

        [Route("getAllFacturas")]
        [HttpGet]
        public async Task<List<FacturaDto>> GetAllFacturas()
        {
            var facturas = await _facturas.GetAllFacturas();
            
            return facturas;
        }
        
        [Route("getFacturasByFacturacion/{facturacion}")]
        [HttpGet]
        public async Task<List<FacturaDto>> GetAllFacturas(int facturacion)
        {
            var facturas = await _facturas.GetAllFacturasAsync(facturacion);
            
            return facturas;
        }

        [Route("getFacturasByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]
        public async Task<List<FacturaDto>> GetFacturasByInmueble(int inmueble, int facturacion)
        {
            var facturas = await _facturas.GetFacturasByInmuebleAsync(inmueble, facturacion);

            return facturas;
        }

        [Route("getConceptosFacturaByIdAsync/{factura}")]
        [HttpGet]
        public async Task<List<ConceptoFacturaDto>> GetConceptosFacturaByIdAsync(int factura)
        {
            return await _facturas.GetConceptosFacturaByIdAsync(factura);
        }

        [Route("getFacturasNCPendientes/{estatus}")]
        [HttpGet]
        public async Task<List<FacturaDto>> getFacturasNCPendientes(int estatus)
        {
            return await _facturas.GetFacturasNCPendientes(estatus);
        }

        [Route("getFacturasCargadas/{facturacion}")]
        [HttpGet]
        public async Task<int> GetFacturasCargadas(int facturacion)
        {
            return await _facturas.GetFacturasCargadasAsync(facturacion);
        }
        
        [Route("getNotasCreditoCargadas/{facturacion}")]
        [HttpGet]
        public async Task<int> GetNotasCreditoCargadas(int facturacion)
        {
            return await _facturas.GetNotasCreditoCargadasAsync(facturacion);
        }
        
        [Route("getTotalFacturasByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]
        public async Task<int> GetTotalFacturasByInmueble(int inmueble, int facturacion)
        {
            return await _facturas.GetTotalFacturasByInmuebleAsync(facturacion, inmueble);
        }

        [Route("getNCByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]
        public async Task<int> GetNCByInmueble(int inmueble, int facturacion)
        {
            return await _facturas.GetNCByInmuebleAsync(facturacion, inmueble);
        }

        [Consumes("multipart/form-data")]
        [Route("createFactura")]
        [HttpPost]
        public async Task<IActionResult> CreateFactura([FromForm] FacturaCreateCommand request)
        {
            var factura = await _mediator.Send(request);
            return Ok(factura);
        }
        
        [Consumes("multipart/form-data")]
        [Route("updateFactura")]
        [HttpPut]
        public async Task<IActionResult> UpdateFactura([FromForm] FacturaUpdateCommand request)
        {
            var factura = await _mediator.Send(request);
            return Ok(factura);
        }

        [Route("createHistorialMF")]
        [HttpPost]
        public async Task<IActionResult> CreateHistorialMF([FromBody] HistorialMFCreateCommand request)
        {
            var historial = await _mediator.Send(request);
            if(historial != null)
            {
                return Ok(historial);
            }
            return BadRequest();
        }

        [Route("getHistorialMFByFacturacion/{facturacion}")]
        [HttpGet]
        public async Task<List<HistorialMFDto>> GetHistorialMFByFacturacion(int repositorio)
        {
            var historial = await _facturas.GetHistorialMFByFacturacion(repositorio);

            HistorialMFDeleteCommand delete = new HistorialMFDeleteCommand();

            delete.RepositorioId = repositorio;

            await _mediator.Send(delete);

            return historial;
        }

        [Route("visualizarFactura/{anio}/{mes}/{folio}/{tipo}/{inmueble}/{archivo}")]
        [HttpGet]
        public string VisualizarFactura(int anio, string mes, string folio, string tipo, string inmueble, string archivo)
        {
            string folderName = "";
            if (tipo.Equals("NC"))
            {
                folderName = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Notas de Crédito\\" + folio;
            }
            else
            {
                folderName = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Facturas\\" + folio;
            }
            string webRootPath = _environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                return pathArchivo;
            }

            return "";
        }
    }
}
