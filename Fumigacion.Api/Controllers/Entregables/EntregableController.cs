using Fumigacion.Service.EventHandler.Commands.Entregables;
using Fumigacion.Service.Queries.DTOs.Entregables;
using Fumigacion.Service.Queries.Queries.Entregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fumigacion.Service.EventHandler.Commands.LogEntregables;
using System;
using Fumigacion.Service.EventHandler.Commands.Entregables;

namespace Fumigacion.Api.Controllers.Entregables
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/fumigacion/entregables")]
    public class EntregableController : ControllerBase
    {
        private readonly IEntregableQueryService _entregables;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public EntregableController(IEntregableQueryService entregables, IMediator mediator, IHostingEnvironment environment)
        {
            _entregables = entregables;
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet]
        public async Task<List<EntregableDto>> GetAllEntregables()
        {
            var configuracion = await _entregables.GetAllEntregablesAsync();

            return configuracion;
        }

        [Route("getEntregablesByCedula/{cedula}")]
        [HttpGet]
        public async Task<List<EntregableDto>> GetEntregablesByCedula(int cedula)
        {
            var entregables = await _entregables.GetEntregablesByCedula(cedula);

            return entregables;
        }

        [Route("getEntregableById/{entregable}")]
        [HttpGet]
        public async Task<EntregableDto> GetEntregableById(int entregable)
        {
            var entregables = await _entregables.GetEntregableById(entregable);

            return entregables;
        }

        [Route("getEntregablesByEstatus/{estatus}")]
        [HttpGet]
        public async Task<List<EntregableEstatusDto>> GetEntregablesByEstatus(int estatus)
        {
            var entregables = await _entregables.GetEntregablesByEstatus(estatus);

            return entregables;
        }

        [Consumes("multipart/form-data")]
        [Route("actualizarEntregable")]
        [HttpPut]
        public async Task<IActionResult> UpdateEntregable([FromForm] EntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if(entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = entregable.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = "Se actualizó el entregable.",
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }

        [Route("updateEntregableSR")]
        [HttpPut]
        public async Task<IActionResult> UpdateEntregableSR([FromBody] ESREntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if (entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = entregable.UsuarioId,
                    EstatusId = !request.Aprobada ? entregable.EstatusId : request.EstatusId,
                    Observaciones = request.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }

        [Route("autorizarEntregable")]
        [HttpPut]
        public async Task<IActionResult> AUpdateEntregable([FromBody] EEntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if(entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = entregable.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = request.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }


        [Route("visualizarEntregable/{anio}/{mes}/{folio}/{archivo}/{tipo}")]
        [HttpGet]
        public async Task<string> VisualizarEntregable(int anio, string mes, string folio, string archivo, string tipo)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio+"\\"+tipo;
            string webRootPath = _environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                return pathArchivo;
            }

            return "";
        }
        
        [Route("getPathEntregables")]
        [HttpGet]
        public async Task<string> GetPathEntregables()
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables";

            return folderName;
        }
    }
}
