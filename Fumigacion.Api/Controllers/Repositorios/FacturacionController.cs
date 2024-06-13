using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Fumigacion.Service.EventHandler.Commands.Repositorios;
using Fumigacion.Service.Queries.DTOs.Repositorios;
using Fumigacion.Service.Queries.Queries.Facturacion;
using Fumigacion.Service.EventHandler.Commands.Facturaciones;

namespace Fumigacion.Api.Controllers.Repositorios
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/repositorios")]
    public class RepositorioController : ControllerBase
    {
        private readonly IRepositorioQueryService _repositorios;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public RepositorioController(IRepositorioQueryService Repositorio, IMediator mediator, IHostingEnvironment environment)
        {
            _repositorios = Repositorio;
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet("{anio}")]
        public async Task<List<RepositorioDto>> GetAllRepositorios(int anio)
        {
            return await _repositorios.GetAllRepositoriosAsync(anio);
        }
        
        [HttpGet("getRepositorioById/{Repositorio}")]
        public async Task<RepositorioDto> GetRepositorioById(int Repositorio)
        {
            return await _repositorios.GetRepositorioByIdAsync(Repositorio);
        }

        [Route("createRepositorio")]
        [HttpPost]
        public async Task<IActionResult> CreateRepositorio([FromBody] RepositorioCreateCommand Repositorio)
        {
            int status = await _mediator.Send(Repositorio);
            return Ok(status);
        }
        
        [Route("updateRepositorio")]
        [HttpPut]
        public async Task<IActionResult> UpdateRepositorio([FromBody] RepositorioUpdateCommand Repositorio)
        {
            var status = await _mediator.Send(Repositorio);
            return Ok(status);
        }
    }
}
