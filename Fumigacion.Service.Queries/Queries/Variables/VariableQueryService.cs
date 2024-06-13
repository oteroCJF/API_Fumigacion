using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.Incidencias;
using Fumigacion.Service.Queries.DTOs.Variables;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.Variables
{
    public interface IVariablesQueryService
    {
        Task<List<VariableDto>> GetAllVariablesAsync();
        Task<List<VariableDto>> GetVariablesTipoIncidencia();
        Task<int> GetVariableIdByTipoIncidencia(string abreviacion, string valor);
        Task<int> GetIdByIncidencia(string abreviacion);
        Task<VariableDto> GetVariableById(int variable);
    }

    public class VariableQueryService : IVariablesQueryService
    {
        private readonly ApplicationDbContext _context;

        public VariableQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VariableDto>> GetAllVariablesAsync()
        {
            var collection = await _context.Variables.OrderByDescending(x => x.Id).ToListAsync();

            return collection.MapTo<List<VariableDto>>();
        }

        public async Task<List<VariableDto>> GetVariablesTipoIncidencia()
        {
            var collection = await _context.Variables.Where(v => v.Tipo.Equals("TipoIncidencia")).OrderByDescending(x => x.Id).ToListAsync();

            return collection.MapTo<List<VariableDto>>();
        }

        public async Task<int> GetVariableIdByTipoIncidencia(string abreviacion, string valor)
        {
            var variable = await _context.Variables.SingleOrDefaultAsync(v => v.Abreviacion.Equals(abreviacion) && v.Valor.Equals(valor));

            return variable.Id;
        }

        public async Task<VariableDto> GetVariableById(int variable)
        {
            return (await _context.Variables.SingleOrDefaultAsync(v => v.Id == variable)).MapTo<VariableDto>();
        }

        public async Task<int> GetIdByIncidencia(string abreviacion)
        {
            var variable = await _context.Variables.SingleOrDefaultAsync(v => v.Abreviacion.Equals(abreviacion));

            return variable.Id;
        }
    }
}
