using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.LogCedulas;
using Fumigacion.Service.Queries.DTOs.LogEntregables;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.LogEntregables
{
    public interface ILogEntregablesQueryService
    {
        Task<List<LogEntregableDto>> GetHistorialEntregablesByCedula(int cedula);
    }

    public class LogEntregableQueryService : ILogEntregablesQueryService
    {
        private readonly ApplicationDbContext _context;

        public LogEntregableQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LogEntregableDto>> GetHistorialEntregablesByCedula(int cedula)
        {
            var historial = await _context.LogEntregables.Where(h => h.CedulaEvaluacionId == cedula).ToListAsync();

            return historial.MapTo<List<LogEntregableDto>>();
        }
    }
}
