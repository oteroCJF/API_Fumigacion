using Fumigacion.Persistence.Database;
using Fumigacion.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Fumigacion.Service.EventHandler.Commands.Firmantes;
using System.Linq;
using Fumigacion.Domain.DIncidencias;
using Fumigacion.Domain.DFirmantes;

namespace Fumigacion.Service.EventHandler.Handlers.Firmantes
{
    public class FirmanteUpdateEventHandler : IRequestHandler<FirmantesUpdateCommand, Firmante>
    {
        private readonly ApplicationDbContext _context;

        public FirmanteUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Firmante> Handle(FirmantesUpdateCommand firmantes, CancellationToken cancellationToken)
        {
            try
            {
                var firmante = _context.Firmantes.SingleOrDefault(f => f.Id == firmantes.Id);

                firmante.UsuarioId = firmantes.UsuarioId;
                firmante.Escolaridad = firmantes.Escolaridad;
                firmante.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                return firmante;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
