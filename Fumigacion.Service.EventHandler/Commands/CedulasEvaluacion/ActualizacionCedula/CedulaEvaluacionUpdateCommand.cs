using MediatR;
using Fumigacion.Domain.DCedulaEvaluacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula
{
    public class CedulaEvaluacionUpdateCommand : IRequest<CedulaEvaluacion>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstatusId { get; set; }
        public int RepositorioId { get; set; }
        public int EFacturaId { get; set; }
        public string Estatus { get; set; }
        public bool Bloqueada { get; set; }
        public bool Aprobada { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Observaciones { get; set; }
    }
}
