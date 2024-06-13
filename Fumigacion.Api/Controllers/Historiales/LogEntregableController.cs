﻿using Fumigacion.Service.EventHandler.Commands.LogEntregables;
using Fumigacion.Service.Queries.DTOs.LogEntregables;
using Fumigacion.Service.Queries.Queries.LogEntregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fumigacion.Api.Controllers.Historiales
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/logEntregables")]
    public class LogEntregableController : ControllerBase
    {
        private readonly ILogEntregablesQueryService _logs;
        private readonly IMediator _mediator;

        public LogEntregableController(ILogEntregablesQueryService logs, IMediator mediator)
        {
            _logs = logs;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("getHistorialEntregablesByCedula/{cedula}")]
        public async Task<List<LogEntregableDto>> GetHistorialEntregablesByCedula(int cedula)
        {
            return await _logs.GetHistorialEntregablesByCedula(cedula);
        }

        [HttpPost]
        [Route("createHistorial")]
        public async Task<IActionResult> CreateHistorial([FromBody] LogEntregablesCreateCommand historial)
        {
            await _mediator.Send(historial);
            return Ok();
        }
    }
}
