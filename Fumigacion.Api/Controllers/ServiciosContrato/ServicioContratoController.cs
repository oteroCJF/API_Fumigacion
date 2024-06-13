using Fumigacion.Service.EventHandler.Commands.ServiciosContrato;
using Fumigacion.Service.Queries.DTOs.Contratos;
using Fumigacion.Service.Queries.Queries.ServiciosContrato;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fumigacion.Api.Controllers.ServicioContrato
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/servicioContrato")]
    public class ServicioContratoController : ControllerBase
    {
        private readonly IServicioContratoQueryService _scontrato;
        private readonly IMediator _mediator;

        public ServicioContratoController(IServicioContratoQueryService scontrato, IMediator mediator)
        {
            _scontrato = scontrato;
            _mediator = mediator;
        }

        [Route("getServiciosContrato/{contrato}")]
        [HttpGet]
        public async Task<List<ServicioContratoDto>> GetServiciosByContrato(int contrato)
        {
            return await _scontrato.GetServicioContratoListAsync(contrato);
        }

        [Route("createSContrato")]
        [HttpPost]
        public async Task<IActionResult> CreateContrato([FromBody] ServicioContratoCreateCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }

        [Route("updateSContrato")]
        [HttpPut]
        public async Task<IActionResult> UpdateContrato([FromBody] ServicioContratoUpdateCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }

        [Route("deleteSContrato")]
        [HttpPut]
        public async Task<IActionResult> DeleteContrato([FromBody] ServicioContratoDeleteCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }
    }
}
