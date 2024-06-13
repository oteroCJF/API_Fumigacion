using Fumigacion.Service.EventHandler.Commands.Contratos;
using Fumigacion.Service.Queries.DTOs.Contratos;
using Fumigacion.Service.Queries.Queries.Contratos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fumigacion.Api.Controllers.Contratos
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/contratos")]
    public class ContratoController : ControllerBase
    {
        private readonly IContratosQueryService _contratos;
        private readonly IMediator _mediator;

        public ContratoController(IContratosQueryService contratos, IMediator mediator)
        {
            _contratos = contratos;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<ContratoDto>> GetAllContratos()
        {
            return await _contratos.GetAllContratosAsync();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ContratoDto> GetContratoById(int id)
        {
            return await _contratos.GetContratoByIdAsync(id);
        }

        [Route("createContrato")]
        [HttpPost]
        public async Task<IActionResult> CreateContrato([FromBody] ContratoCreateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("updateContrato")]
        [HttpPut]
        public async Task<IActionResult> UpdateContrato([FromBody] ContratoUpdateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("deleteContrato")]
        [HttpPut]
        public async Task<IActionResult> DeleteContrato([FromBody] ContratoDeleteCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }
    }
}
