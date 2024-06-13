using Fumigacion.Persistence.Database;
using Fumigacion.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fumigacion.Domain.DIncidencias;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Fumigacion.Service.EventHandler.Handlers.Incidencias
{
    public class IncidenciaCreateEventHandler : IRequestHandler<IncidenciaCreateCommand, Incidencia>
    {
        private readonly ApplicationDbContext _context;

        public IncidenciaCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Incidencia> Handle(IncidenciaCreateCommand request, CancellationToken cancellationToken)
        {
            var fechaProgramada = Convert.ToDateTime(request.FechaProgramada).ToString("dd/MM/yyyy");
            var fechaRealizada = Convert.ToDateTime(request.FechaRealizada).ToString("dd/MM/yyyy");

            var incidencia = new Incidencia 
            {
                CedulaEvaluacionId = request.CedulaEvaluacionId,
                UsuarioId = request.UsuarioId,
                IncidenciaId = request.IncidenciaId,
                DIncidenciaId = request.DIncidenciaId,
                Pregunta = request.Pregunta,
                FechaProgramada = request.FechaProgramada,
                FechaRealizada = request.FechaRealizada,
                FechaReaparicion = request.FechaReaparicion,
                HoraProgramada = request.HoraProgramada,
                HoraRealizada = request.HoraRealizada,
                MesId = request.MesId,
                DiasAtraso = !fechaProgramada.Equals("01/01/1990") && !fechaRealizada.Equals("01/01/1990") ? GetDiasAtraso((DateTime)request.FechaProgramada, (DateTime)request.FechaRealizada): 0,
                HorasAtraso = GetHorasAtraso((TimeSpan)request.HoraProgramada, (TimeSpan)request.HoraRealizada),
                Observaciones = request.Observaciones,
                FechaCreacion = DateTime.Now
            };

            try
            {
                await _context.AddAsync(incidencia);
                await _context.SaveChangesAsync();

                return incidencia;
            }
            catch(Exception ex)
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
