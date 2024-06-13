using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.Repositorios;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.Facturacion
{
    public interface IRepositorioQueryService
    {
        Task<List<RepositorioDto>> GetAllRepositoriosAsync(int anio);
        Task<RepositorioDto> GetRepositorioByIdAsync(int facturacion);
    }

    public class RepositorioQueryService : IRepositorioQueryService
    {
        private readonly ApplicationDbContext _context;

        public RepositorioQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RepositorioDto>> GetAllRepositoriosAsync(int anio)
        {
            try
            {
                var collection = await _context.Repositorios.Where(x => x.Anio == anio).OrderBy(x => x.MesId).ToListAsync();

                return collection.MapTo<List<RepositorioDto>>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public async Task<RepositorioDto> GetRepositorioByIdAsync(int facturacion)
        {
            try
            {
                var collection = await _context.Repositorios.SingleOrDefaultAsync(x => x.Id == facturacion);

                return collection.MapTo<RepositorioDto>();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

    }
}
