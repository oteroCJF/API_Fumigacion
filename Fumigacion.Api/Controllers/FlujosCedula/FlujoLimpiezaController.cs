using Fumigacion.Service.Queries.DTOs.FlujoFumigacion;
using Fumigacion.Service.Queries.Queries.Flujo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fumigacion.Api.Controllers.Flujo
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/limpieza/flujo")]
    public class FlujoFumigacionController : ControllerBase
    {
        private readonly IFlujoCedulaQueryService _flujo;

        public FlujoFumigacionController(IFlujoCedulaQueryService flujo)
        {
            _flujo = flujo;
        }

        [Route("getFlujoByCedulaEstatus/{estatus}")]
        [HttpGet]
        public async Task<List<FlujoCedulaDto>> getFlujoByCedulaEstatus(int estatus)
        {
            var flujo = await _flujo.GetFlujoByEstatus(estatus);

            return flujo;
        }
    }
}
