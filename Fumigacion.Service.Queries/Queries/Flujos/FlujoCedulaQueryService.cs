﻿using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.FlujoFumigacion;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.Flujo
{
    public interface IFlujoCedulaQueryService
    {
        Task<List<FlujoCedulaDto>> GetFlujoByEstatus(int estatus);
    }

    public class FlujoCedulaQueryService : IFlujoCedulaQueryService
    {
        private readonly ApplicationDbContext _context;

        public FlujoCedulaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlujoCedulaDto>> GetFlujoByEstatus(int estatus)
        {
            var collection = await _context.Flujo.Where(f => f.EstatusCedulaId == estatus).ToListAsync();

            return collection.MapTo<List<FlujoCedulaDto>>();
        }
    }
}
