using MediatR;
using Fumigacion.Domain.DCedulaEvaluacion;
using System.Collections.Generic;

namespace Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula
{
    public class EnviarCedulaEvaluacionUpdateCommand : IRequest<CedulaEvaluacion>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstatusId { get; set; }
        public int RepositorioId { get; set; }
        public int EFacturaId { get; set; }
        public bool Calcula { get; set; }
        public string Estatus { get; set; }
        public string Observaciones { get; set; }

        public virtual List<ServicioContratoDto> Penalizacion { get; set; } 
    }
}
