using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.CedulaEvaluacion;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.CedulasEvaluacion
{
    public interface IRespuestasQueryService
    {
        Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio);
        Task<List<RespuestaDto>> GetRespuestasByCedulaAsync(int cedula);
        Task<decimal> GetTotalPenasDeductivas(int cedula);
    }

    public class RespuestaQueryService : IRespuestasQueryService
    {
        private readonly ApplicationDbContext _context;

        public RespuestaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio)
        {
            try
            {
                var cedulas = await _context.CedulaEvaluacion.Where(c => c.Anio == anio).Select(c=> c.Id).ToListAsync();
                var respuestas = await _context.Respuestas.Where(r => cedulas.Contains(r.CedulaEvaluacionId))
                                .ToListAsync();
                return respuestas.MapTo<List<RespuestaDto>>();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        
        public async Task<List<RespuestaDto>> GetRespuestasByCedulaAsync(int cedula)
        {
            try
            {
                var respuestas = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).OrderBy(r => r.Pregunta).ToListAsync();
                return respuestas.MapTo<List<RespuestaDto>>();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<decimal> GetTotalPenasDeductivas(int cedula)
        {
            try
            {
                var totalPD = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).Select(r => r.MontoPenalizacion).SumAsync();
                var tot = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).ToListAsync();

                return Convert.ToDecimal(totalPD);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

    }
}
