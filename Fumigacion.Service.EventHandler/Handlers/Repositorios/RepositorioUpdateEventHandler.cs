using Fumigacion.Persistence.Database;
using Fumigacion.Domain.DRepositorios;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fumigacion.Service.EventHandler.Commands.Facturaciones;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Fumigacion.Service.EventHandler.Commands.Repositorios;

namespace Fumigacion.Service.EventHandler.Handlers.HFacturacion
{
    public class RepositorioUpdateEventHandler : IRequestHandler<RepositorioUpdateCommand, Repositorio>
    {
        private readonly ApplicationDbContext _context;

        public RepositorioUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Repositorio> Handle(RepositorioUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var repositorio = await _context.Repositorios.SingleOrDefaultAsync(f => f.Id == request.Id);

                repositorio.ContratoId = request.ContratoId;
                repositorio.Anio = request.Anio;
                repositorio.MesId = request.MesId;
                repositorio.UsuarioId = request.UsuarioId;
                repositorio.EstatusId = request.EstatusId;

                await _context.SaveChangesAsync();

                return repositorio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
