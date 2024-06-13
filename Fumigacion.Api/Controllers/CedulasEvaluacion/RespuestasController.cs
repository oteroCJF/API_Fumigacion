using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Fumigacion.Service.Queries.DTOs.CedulaEvaluacion;
using Fumigacion.Service.EventHandler.Commands.Respuestas;
using MediatR;
using System;
using Fumigacion.Service.Queries.Queries.CedulasEvaluacion;

namespace Fumigacion.Api.Controllers.CedulasEvaluacion
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/respuestasEvaluacion")]
    public class RespuestasController : ControllerBase
    {
        private readonly IRespuestasQueryService _respuestas;
        private readonly IMediator _mediator;

        public RespuestasController(IRespuestasQueryService respuestas, IMediator mediator)
        {
            _respuestas = respuestas;
            _mediator = mediator;
        }

        [Route("{cedula}")]
        [HttpGet]
        public async Task<List<RespuestaDto>> GetCedulaEvaluacionByCedulaAnioMes(int cedula)
        {
            var respuestas = await _respuestas.GetRespuestasByCedulaAsync(cedula);

            return respuestas;
        }
        
        [Route("getRespuestasByAnio/{anio}")]
        [HttpGet]
        public async Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio)
        {
            var respuestas = await _respuestas.GetAllRespuestasByAnioAsync(anio);

            return respuestas;
        }

        [Route("updateRespuestasByCedula")]
        [HttpPut]
        public async Task<IActionResult> UpdateRespuestasByCedula([FromBody] List<RespuestasUpdateCommand> respuestas)
        {
            try
            {
                foreach (var rs in respuestas)
                {
                    await _mediator.Send(rs);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }

        }
    }
}
