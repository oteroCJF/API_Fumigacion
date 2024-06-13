﻿using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.Contratos;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.ServiciosContrato
{
    public interface IServicioContratoQueryService
    {
        Task<List<ServicioContratoDto>> GetServicioContratoListAsync(int contrato);
    }

    public class ServicioContratoQueryService : IServicioContratoQueryService
    {
        private readonly ApplicationDbContext _context;

        public ServicioContratoQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServicioContratoDto>> GetServicioContratoListAsync(int contrato)
        {
            var servicios = await _context.ServicioContrato.Where(sc => sc.ContratoId == contrato).ToListAsync();

            return servicios.MapTo<List<ServicioContratoDto>>();
        }
    }
}
