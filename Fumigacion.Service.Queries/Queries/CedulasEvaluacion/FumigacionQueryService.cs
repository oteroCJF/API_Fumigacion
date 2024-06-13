using Fumigacion.Domain.DCuestionario;
using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.DTOs.CedulaEvaluacion;
using Fumigacion.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fumigacion.Service.Queries.Queries.CedulasEvaluacion
{
    public interface IFumigacionQueryService
    {
        Task<List<CedulaEvaluacionDto>> GetAllCedulasAsync();
        Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnio(int anio);
        Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnioMes(int anio, int mes);
        Task<CedulaEvaluacionDto> GetCedulaEvaluacionByInmuebleAnioMesAsync(int inmueble, int anio, int mes);
        Task<CedulaEvaluacionDto> GetCedulaById(int cedula);
    }

    public class FumigacionQueryService : IFumigacionQueryService
    {
        private readonly ApplicationDbContext _context;

        public FumigacionQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CedulaEvaluacionDto>> GetAllCedulasAsync()
        {
            var collection = await _context.CedulaEvaluacion.OrderByDescending(x => x.Id)
                                                            .OrderBy(c => c.MesId)
                                                            .ToListAsync();

            return collection.MapTo<List<CedulaEvaluacionDto>>();
        }

        public async Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnio(int anio)
        {
            try
            {
                var cedulas = await _context.CedulaEvaluacion.Where(x => x.Anio == anio && !x.FechaEliminacion.HasValue)
                                                             .OrderBy(c => c.MesId)
                                                             .ToListAsync();
                return cedulas.MapTo<List<CedulaEvaluacionDto>>();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnioMes(int anio, int mes)
        {
            try
            {
                var cedulas = await _context.CedulaEvaluacion
                          .Where(x => x.Anio == anio && x.MesId == mes && !x.FechaEliminacion.HasValue)
                          .ToListAsync();
                return cedulas.MapTo<List<CedulaEvaluacionDto>>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<CedulaEvaluacionDto> GetCedulaById(int cedula)
        {
            return (await _context.CedulaEvaluacion.SingleOrDefaultAsync(x => x.Id == cedula)).MapTo<CedulaEvaluacionDto>();
        }
        
        public async Task<CedulaEvaluacionDto> GetCedulaEvaluacionByInmuebleAnioMesAsync(int inmueble, int anio, int mes)
        {
            try
            {
                var cedula = await _context.CedulaEvaluacion.SingleOrDefaultAsync(x => x.InmuebleId == inmueble && x.Anio == anio && x.MesId == mes);
                return cedula.MapTo<CedulaEvaluacionDto>();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
