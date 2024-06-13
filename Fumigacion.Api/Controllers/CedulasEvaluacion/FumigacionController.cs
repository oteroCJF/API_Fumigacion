using Fumigacion.Service.Queries.DTOs.CedulaEvaluacion;
using Fumigacion.Service.Queries.Queries.Incidencias;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fumigacion.Service.Queries.Queries.CedulasEvaluacion;
using Fumigacion.Service.EventHandler.Commands.LogCedulas;
using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;
using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;

namespace Fumigacion.Api.Controllers.CedulaEvaluacion
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/cedulaEvaluacion")]
    public class FumigacionController : ControllerBase
    {
        private readonly IFumigacionQueryService _cedula;
        private readonly IIncidenciasQueryService _incidencias;
        private readonly IRespuestasQueryService _respuestas;
        private readonly IMediator _mediator;

        public FumigacionController(IFumigacionQueryService cedula, IRespuestasQueryService respuestas, 
                                    IIncidenciasQueryService incidencias, IMediator mediator)
        {
            _cedula = cedula;
            _incidencias = incidencias;
            _respuestas = respuestas;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<CedulaEvaluacionDto>> GetAllCedulasEvaluacion()
        {
            return await _cedula.GetAllCedulasAsync();
        }

        [Route("getCedulasByAnio/{anio}")]
        [HttpGet]
        public async Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnio(int anio)
        {
            var cedulaEvaluacion = await _cedula.GetCedulaEvaluacionByAnio(anio);

            return cedulaEvaluacion;
        }

        [Route("getCedulasByAnioMes/{anio}/{mes}")]
        [HttpGet]
        public async Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnioMes(int anio, int mes)
        {
            var cedulaEvaluacion = await _cedula.GetCedulaEvaluacionByAnioMes(anio, mes);

            return cedulaEvaluacion;
        }

        [Route("getCedulasByInmuebleAM/{inmueble}/{anio}/{mes}")]
        [HttpGet]
        public async Task<CedulaEvaluacionDto> GetCedulaEvaluacionByInmuebleAnioMes(int inmueble, int anio, int mes)
        {
            var cedulaEvaluacion = await _cedula.GetCedulaEvaluacionByInmuebleAnioMesAsync(inmueble, anio, mes);

            return cedulaEvaluacion != null ? cedulaEvaluacion : new CedulaEvaluacionDto();
        }

        [Route("getCedulaById/{cedula}")]
        [HttpGet]
        public async Task<CedulaEvaluacionDto> GetCedulaEvaluacionById(int cedula)
        {
            var cedulaEvaluacion = await _cedula.GetCedulaById(cedula);

            return cedulaEvaluacion != null ? cedulaEvaluacion : new CedulaEvaluacionDto();
        }

        [Route("enviarCedula")]
        [HttpPut]
        public async Task<IActionResult> UpdateCedula([FromBody] EnviarCedulaEvaluacionUpdateCommand request)
        {
            var cedula = await _mediator.Send(request);

            if (cedula != null)
            {
                var log = new LogCedulasCreateCommand
                {
                    UsuarioId = request.UsuarioId,
                    CedulaEvaluacionId = cedula.Id,
                    EstatusId = request.EstatusId,
                    Observaciones = request.Observaciones
                };

                var logs = await _mediator.Send(log);

                if (logs != null)
                {
                    return Ok(cedula);
                }
            }
            return Ok(cedula);
        }

        [Route("updateCedula")]
        [HttpPut]
        public async Task<IActionResult> CedulaSolicitudRechazo([FromBody] CedulaEvaluacionUpdateCommand request)
        {
            var cedula = await _mediator.Send(request);

            if (cedula != null)
            {
                var log = new LogCedulasCreateCommand
                {
                    UsuarioId = request.UsuarioId,
                    CedulaEvaluacionId = cedula.Id,
                    EstatusId = cedula.EstatusId,
                    Observaciones = request.Observaciones
                };

                var logs = await _mediator.Send(log);

                if (logs != null)
                {
                    return Ok(cedula);
                }
            }
            return Ok(cedula);
        }

        [Route("getTotalPD/{cedula}")]
        [HttpGet]
        public async Task<decimal> GetTotalPDAsync(int cedula)
        {
            return await _respuestas.GetTotalPenasDeductivas(cedula);
        }
    }
}
