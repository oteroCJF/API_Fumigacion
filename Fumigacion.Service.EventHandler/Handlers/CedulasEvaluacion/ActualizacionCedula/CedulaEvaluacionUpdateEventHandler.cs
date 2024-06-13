using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion;
using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;
using MediatR;
using Fumigacion.Domain.DCedulaEvaluacion;
using Fumigacion.Domain.DFacturas;
using Fumigacion.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fumigacion.Service.EventHandler.Handlers.CedulasEvaluacion
{
    public class CedulaEvaluacionUpdateEventHandler : IRequestHandler<CedulaEvaluacionUpdateCommand, CedulaEvaluacion>
    {
        private readonly ApplicationDbContext _context;

        public CedulaEvaluacionUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CedulaEvaluacion> Handle(CedulaEvaluacionUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cedula = _context.CedulaEvaluacion.Single(c => c.Id == request.Id);

                List<Factura> facturas = _context.Facturas
                                                               .Where(f => f.RepositorioId == request.RepositorioId &&
                                                                           f.InmuebleId == cedula.InmuebleId && f.Tipo.Equals("Factura")
                                                                           && f.Facturacion.Equals("Mensual"))
                                                               .ToList();

                cedula.EstatusId = request.EstatusId;
                cedula.Bloqueada = request.Bloqueada;
                cedula.FechaActualizacion = request.FechaActualizacion;

                await _context.SaveChangesAsync();

                foreach (var fac in facturas)
                {
                    fac.EstatusId = request.EFacturaId;

                    await _context.SaveChangesAsync();
                }

                return cedula;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
