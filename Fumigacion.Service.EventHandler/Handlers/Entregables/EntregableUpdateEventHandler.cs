using Fumigacion.Persistence.Database;
using Fumigacion.Service.EventHandler.Commands.Entregables;
using Fumigacion.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.IO;
using Fumigacion.Domain.DEntregables;
using System.Text.RegularExpressions;
using Fumigacion.Service.EventHandler.Commands.LogEntregables;

namespace Fumigacion.Service.EventHandler.Handlers.Entregables
{
    public class EntregableUpdateEventHandler : IRequestHandler<EntregableUpdateCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public EntregableUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(EntregableUpdateCommand request, CancellationToken cancellationToken)
        {
            Entregable entregable = _context.Entregables.Where(e => e.Id == request.Id).FirstOrDefault();
            string newDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (request.Archivo != null)
            {
                if (entregable.Archivo != null && !entregable.Archivo.Equals(""))
                {
                    eliminaArchivoActual(request.Anio, request.Mes, request.Folio, entregable.Archivo, request.TipoEntregable);
                }

                if (await guardaArchivo(request.Archivo, request.Anio, request.Mes, request.Folio, newDate, request.TipoEntregable))
                {
                    entregable.UsuarioId = request.UsuarioId;
                    entregable.EstatusId = request.EstatusId;
                    entregable.Archivo = newDate + "_" + request.Archivo.FileName;
                    entregable.Observaciones = request.Observaciones;
                    entregable.FechaActualizacion = DateTime.Now;
                }
            }

            if (request.Validar)
            {
                entregable.Validado = request.Validado;
            }

            await _context.SaveChangesAsync();

            return entregable;
        }

        public async Task<bool> guardaArchivo(IFormFile archivo, int anio, string mes, string folio, string fecha, string tipoEntregable)
        {
            long size = archivo.Length;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio+"\\"+tipoEntregable;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            using (var stream = new FileStream(newPath + "\\" + fecha + "_" + archivo.FileName, FileMode.Create))
            {
                try
                {
                    await archivo.CopyToAsync(stream);
                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return false;
                }
            }
        }

        public bool eliminaArchivoActual(int anio, string mes, string folio, string archivo, string tipoEntregable)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio+"\\"+tipoEntregable;

            newPath += "\\" + archivo;

            try
            {
                File.Delete(newPath);
                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return false;
            }
        }
    }
}
