using Fumigacion.Service.EventHandler.Commands.Entregables;
using MediatR;
using Fumigacion.Domain.DEntregables;
using Fumigacion.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fumigacion.Service.EventHandler.Handlers.Entregables
{
    public class ESREntregableUpdateEventHandler : IRequestHandler<ESREntregableUpdateCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public ESREntregableUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(ESREntregableUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Entregable entregable = _context.Entregables.Where(e => e.Id == request.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();

                entregable.UsuarioId = request.UsuarioId;
                entregable.EstatusId = request.EstatusId;
                if (request.Aprobada)
                {
                    entregable.FechaEliminacion = DateTime.Now;
                }
                else
                {
                    entregable.FechaEliminacion = null;
                }

                await _context.SaveChangesAsync();

                return entregable;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
