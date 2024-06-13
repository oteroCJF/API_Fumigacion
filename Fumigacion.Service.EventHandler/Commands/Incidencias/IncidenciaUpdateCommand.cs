using MediatR;
using Fumigacion.Domain.DIncidencias;
using Microsoft.AspNetCore.Http;
using System;

namespace Fumigacion.Service.EventHandler.Commands.Incidencias
{
    public class IncidenciaUpdateCommand : IRequest<Incidencia>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int IncidenciaId { get; set; }
        public int DIncidenciaId { get; set; }
        public int Pregunta { get; set; }
        public int MesId { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaRealizada { get; set; }
        public DateTime? FechaReaparicion { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public TimeSpan HoraRealizada { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
