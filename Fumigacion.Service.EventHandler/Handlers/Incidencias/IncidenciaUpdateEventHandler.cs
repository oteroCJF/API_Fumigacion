using Fumigacion.Persistence.Database;
using Fumigacion.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Fumigacion.Domain.DIncidencias;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Fumigacion.Service.EventHandler.Handlers.Incidencias
{
    public class IncidenciaUpdateEventHandler : IRequestHandler<IncidenciaUpdateCommand, Incidencia>
    {
        private readonly ApplicationDbContext _context;

        public IncidenciaUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Incidencia> Handle(IncidenciaUpdateCommand request, CancellationToken cancellationToken)
        {
            var incidencia = _context.Incidencias.SingleOrDefault(i => i.Id == request.Id);
            var fechaProgramada = Convert.ToDateTime(request.FechaProgramada).ToString("dd/MM/yyyy");
            var fechaRealizada = Convert.ToDateTime(request.FechaRealizada).ToString("dd/MM/yyyy");

            incidencia.CedulaEvaluacionId = request.CedulaEvaluacionId;
            incidencia.UsuarioId = request.UsuarioId;
            incidencia.IncidenciaId = request.IncidenciaId;
            incidencia.DIncidenciaId = request.DIncidenciaId;
            incidencia.Pregunta = request.Pregunta;
            incidencia.FechaProgramada = request.FechaProgramada;
            incidencia.FechaRealizada = request.FechaRealizada;
            incidencia.FechaReaparicion = request.FechaReaparicion;
            incidencia.HoraProgramada = request.HoraProgramada;
            incidencia.HoraRealizada = request.HoraRealizada;
            incidencia.MesId = request.MesId;
            if (!fechaProgramada.Equals("01/01/1990") && !fechaRealizada.Equals("01/01/1990"))
                incidencia.DiasAtraso = GetDiasAtraso((DateTime)request.FechaProgramada, (DateTime)request.FechaRealizada);
            else
                incidencia.DiasAtraso = 0;

            incidencia.HorasAtraso = GetHorasAtraso((TimeSpan)request.HoraProgramada, (TimeSpan)request.HoraRealizada);
            incidencia.Observaciones = request.Observaciones;
            incidencia.FechaActualizacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();

                return incidencia;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return incidencia;
            }
        }

        private int GetDiasAtraso(DateTime fechaProgramada, DateTime fechaRealizada)
        {
            var diasAtraso = 0;
            diasAtraso = (fechaRealizada - fechaProgramada).Days;
            return diasAtraso;
        }

        private int GetHorasAtraso(TimeSpan horaProgramada, TimeSpan horaRealizada)
        {
            var diasAtraso = 0;
            var fp = Convert.ToDateTime(horaRealizada.ToString());
            var fr = Convert.ToDateTime(horaProgramada.ToString());
            diasAtraso = fp.Hour - fr.Hour;
            return diasAtraso;
        }
    }
}
