using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using Fumigacion.Persistence.Database;
using Fumigacion.Domain.DCedulaEvaluacion;
using Fumigacion.Domain.DFacturas;
using Fumigacion.Domain.DIncidencias;
using Fumigacion.Domain.DCuestionario;
using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion;
using Fumigacion.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;

namespace Fumigacion.Service.EventHandler.Handlers.CedulasEvaluacion
{
    public class EnviarCedulaEvaluacionUpdateEventHandler : IRequestHandler<EnviarCedulaEvaluacionUpdateCommand, CedulaEvaluacion>
    {
        private readonly ApplicationDbContext _context;

        public EnviarCedulaEvaluacionUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CedulaEvaluacion> Handle(EnviarCedulaEvaluacionUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                CedulaEvaluacion cedula = _context.CedulaEvaluacion.FirstOrDefault(c => c.Id == request.Id);

                List<Factura> facturas = _context.Facturas
                                                               .Where(f => f.RepositorioId == request.RepositorioId &&
                                                                           f.InmuebleId == cedula.InmuebleId && f.Tipo.Equals("Factura")
                                                                           && f.Facturacion.Equals("Mensual"))
                                                               .ToList();

                if (request.Calcula)
                {
                    List<CuestionarioMensual> cuestionarioMensual = _context.CuestionarioMensual
                                                                .Where(cm => cm.Anio == cedula.Anio && cm.MesId == cedula.MesId && cm.ContratoId == cedula.ContratoId)
                                                                .ToList();

                    var incidencias = await CalculaPDIncidencias(request.Id, cuestionarioMensual, request.Penalizacion, facturas);
                    var respuestas = await Obtienetotales(request.Id, cuestionarioMensual);
                    var calificacion = await GetCalificacionCedula(request.Id, cuestionarioMensual);

                    cedula.EstatusId = request.EstatusId;
                    if (calificacion < 10)
                    {
                        string calif = calificacion.ToString().Substring(0, 3);
                        cedula.Calificacion = Convert.ToDouble(calif);
                    }
                    else
                    {
                        cedula.Calificacion = (double)calificacion;
                    }
                    if (calificacion < Convert.ToDecimal(8))
                    {
                        cedula.Penalizacion = (Convert.ToDecimal(facturas.Sum(f => f.Subtotal)) * Convert.ToDecimal(0.01)) / calificacion;
                    }
                    else
                    {
                        cedula.Penalizacion = 0;
                    }
                    cedula.FechaActualizacion = DateTime.Now;
                }

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

        private async Task<List<Incidencia>> CalculaPDIncidencias(int cedula, List<CuestionarioMensual> cuestionario,
                                                                   List<ServicioContratoDto> servicio, List<Factura> facturas)
        {
            var incidencias = new List<Incidencia>();

            decimal montoP = 0;

            foreach (var cm in cuestionario)
            {
                incidencias = _context.Incidencias.Where(i => i.Pregunta == cm.Consecutivo && i.CedulaEvaluacionId == cedula
                                                                && !i.FechaEliminacion.HasValue).ToList();
                foreach (var inc in incidencias)
                {
                    montoP = 0;
                    if (cm.Formula.Contains("CFMI"))
                    {
                        montoP = Convert.ToDecimal(facturas.Sum(f => f.Subtotal)) * cm.Porcentaje;
                        if (cm.Formula.Contains("NDA"))
                        {
                            montoP = montoP * inc.DiasAtraso;
                        }
                        else if (cm.Formula.Contains("NHA"))
                        {
                            montoP = montoP * inc.HorasAtraso;
                        }
                    }

                    inc.Penalizable = montoP != 0 ? true : false;
                    inc.MontoPenalizacion = montoP;

                    await _context.SaveChangesAsync();
                }
            }

            return incidencias;
        }

        private async Task<List<RespuestaEvaluacion>> Obtienetotales(int cedula, List<CuestionarioMensual> cuestionario)
        {
            var incidencias = new List<Incidencia>();

            foreach (var cm in cuestionario)
            {
                incidencias = _context.Incidencias.Where(i => i.Pregunta == cm.Consecutivo && i.CedulaEvaluacionId == cedula
                                                            && !i.FechaEliminacion.HasValue).ToList();
                var respuesta = _context.Respuestas.SingleOrDefault(r => r.CedulaEvaluacionId == cedula && r.Pregunta == cm.Consecutivo);

                if (!respuesta.Detalles.Equals("N/A"))
                {
                    respuesta.Detalles = incidencias.Count() + "";
                }
                respuesta.Penalizable = (incidencias.Sum(i => i.MontoPenalizacion) != 0 ? true : false);
                respuesta.MontoPenalizacion = (incidencias.Sum(i => i.MontoPenalizacion) != 0 ? incidencias.Sum(i => i.MontoPenalizacion) : 0);

                await _context.SaveChangesAsync();
            }
            var respuestas = _context.Respuestas.ToList();

            return respuestas;
        }

        private async Task<decimal> GetCalificacionCedula(int cedula, List<CuestionarioMensual> cuestionario)
        {
            decimal calificacion = 0;
            decimal ponderacion = 0;
            bool calidad = true;
            var incidencias = 0;

            List<decimal> ponder = new List<decimal>();

            var respuestas = _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).ToList();

            foreach (var rs in respuestas)
            {
                var cm = cuestionario.Single(c => c.Consecutivo == rs.Pregunta);
                var dtPregunta = _context.Cuestionarios.Single(c => c.Id == cm.CuestionarioId);
                if (cm.ACLRS == rs.Respuesta)
                {
                    calidad = !calidad;
                    incidencias = _context.Incidencias.Where(i => i.CedulaEvaluacionId == cedula && i.Pregunta == cm.Consecutivo
                                                            && !i.FechaEliminacion.HasValue).Count();

                    if (incidencias != 0)
                    {
                        ponderacion = Convert.ToDecimal(cm.Ponderacion) / Convert.ToDecimal(2);
                    }

                    calificacion += ponderacion;

                    rs.Detalles = incidencias + "";
                    rs.Penalizable = false;
                    rs.MontoPenalizacion = _context.Incidencias.Where(i => i.CedulaEvaluacionId == cedula &&
                                                                      i.Pregunta == cm.Consecutivo &&
                                                                      !i.FechaEliminacion.HasValue).Sum(i => i.MontoPenalizacion);
                }
                else
                {
                    calificacion += Convert.ToDecimal(cm.Ponderacion);
                    rs.Penalizable = false;
                    rs.Detalles = incidencias + "";
                    rs.MontoPenalizacion = _context.Incidencias.Where(i => i.CedulaEvaluacionId == cedula &&
                                                                      i.Pregunta == cm.Consecutivo &&
                                                                      !i.FechaEliminacion.HasValue).Sum(i => i.MontoPenalizacion);
                }

                await _context.SaveChangesAsync();
                ponder.Add(ponderacion);
            }

            calificacion = Convert.ToDecimal(calificacion / respuestas.Count());

            if (calificacion >= Convert.ToDecimal(8))
            {
                calificacion += 1;
            }

            return calificacion;
        }
    }
}
