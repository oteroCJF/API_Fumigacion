using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.LogCedulas;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.LogCedulas
{
    public interface ILogCedulasQueryService
    {
        Task<List<LogCedulaDto>> GetHistorialCedula(int cedula);
    }

    public class LogCedulaQueryService : ILogCedulasQueryService
    {
        private readonly ApplicationDbContext _context;

        public LogCedulaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LogCedulaDto>> GetHistorialCedula(int cedula)
        {
            try
            {
                var historial = await _context.LogCedulas.Where(h => h.CedulaEvaluacionId == cedula).ToListAsync();

                return historial.MapTo<List<LogCedulaDto>>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
