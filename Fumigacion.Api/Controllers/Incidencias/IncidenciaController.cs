using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Fumigacion.Service.Queries.DTOs.Incidencias;
using Fumigacion.Service.EventHandler.Commands.Incidencias;
using MediatR;
using Fumigacion.Service.Queries.Queries.Incidencias;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Fumigacion.Api.Controllers.Incidencias
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/incidenciasCedula")]
    public class IncidenciaController : ControllerBase
    {
        private readonly IIncidenciasQueryService _incidencias;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public IncidenciaController(IIncidenciasQueryService incidencias, IMediator mediator, IHostingEnvironment environment)
        {
            _incidencias = incidencias;
            _mediator = mediator;
            _environment = environment;
        }

        [Route("getConfiguracionIncidencias")]
        [HttpGet]
        public async Task<List<ConfiguracionIncidenciaDto>> GetConfiguracionIncidencias()
        {
            var configuracion = await _incidencias.GetConfiguracionIncidencias();

            return configuracion;
        }

        [Route("getConfiguracionIncidenciasByPregunta/{pregunta}/{respuesta}")]
        [HttpGet]
        public async Task<ConfiguracionIncidenciaDto> GetConfiguracionIncidenciasByPregunta(int pregunta, bool respuesta)
        {
            var configuracion = await _incidencias.GetConfiguracionIncidenciasByPregunta(pregunta, respuesta);

            return configuracion;
        }

        [Route("getIncidenciasByCedulaAndPregunta/{cedula}/{pregunta}")]
        [HttpGet]
        public async Task<List<IncidenciaDto>> GetIncidenciasByPreguntaAndCedula(int cedula, int pregunta)
        {
            var incidencias = await _incidencias.GetIncidenciasByPreguntaAndCedula(cedula, pregunta);

            return incidencias;
        }

        [Route("getIncidenciaByCedulaAndPregunta/{cedula}/{pregunta}")]
        [HttpGet]
        public async Task<IncidenciaDto> GetIncidenciaByPreguntaAndCedula(int cedula, int pregunta)
        {
            var incidencias = await _incidencias.GetIncidenciaByPreguntaAndCedula(cedula, pregunta);

            return incidencias != null ? incidencias : new IncidenciaDto();
        }

        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [Route("insertaIncidencia")]
        [HttpPost]
        public async Task<IActionResult> InsertaIncidencia([FromForm] IncidenciaCreateCommand incidencia)
        {
            await _mediator.Send(incidencia);
            return Ok();
        }

        [Route("actualizarIncidencia")]
        [HttpPut]
        public async Task<IActionResult> ActualizarIncidencia([FromForm] IncidenciaUpdateCommand incidencia)
        {
            await _mediator.Send(incidencia);
            return Ok();
        }

        [Route("eliminarIncidencias")]
        [HttpPost]
        public async Task<IActionResult> EliminarIncidencias([FromBody] IncidenciaDeleteCommand incidencia)
        {
            var lIncidencias = await _incidencias.GetIncidenciasByPreguntaAndCedula(incidencia.CedulaEvaluacionId, incidencia.Pregunta);
            
            foreach (var inc in lIncidencias) 
            {
                incidencia.Id = inc.Id;
                await _mediator.Send(incidencia);
            }

            return Ok(lIncidencias.Count);
        }

        [Route("eliminarIncidencia")]
        [HttpPost]
        public async Task<IActionResult> EliminarIncidencia([FromBody] IncidenciaDeleteCommand request)
        {
            var incidencia = await _mediator.Send(request);

            return Ok(incidencia);
        }

        [Route("visualizarActas/{anio}/{mes}/{folio}/{tipo}/{tipoArchivo}/{archivo}")]
        [HttpGet]
        public string VisualizarActas(int anio, string mes, string folio, string tipo, string tipoArchivo, string archivo)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio + "\\Actas " + tipo.Replace("Guías ","") + "\\"+tipoArchivo;
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
