using Fumigacion.Domain.DOficios;
using Fumigacion.Persistence.Database;
using Fumigacion.Service.EventHandler.Commands.Oficios;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fumigacion.Service.EventHandler.Handlers.Oficios
{
    public class OficioCreateEventHandler : IRequestHandler<OficioCreateCommand, Oficio>
    {
        private readonly ApplicationDbContext _context;

        public OficioCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Oficio> Handle(OficioCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var oficio = new Oficio
                {
                    ContratoId = request.ContratoId,
                    Anio = request.Anio,
                    NumeroOficio = request.NumeroOficio.Replace("/", "_"),
                    FechaTramitado = request.FechaTramitado,
                    FechaCreacion = DateTime.Now
                };

                await _context.AddAsync(oficio);
                await _context.SaveChangesAsync();

                request.Id = oficio.Id;

                var path = CargaArchivoExcel(request.Oficio, oficio.NumeroOficio.Replace("\"", "_").ToString());

                var detalles = await GetDatosExcel(path, request);

                return detalles.Count() != 0 ? oficio : null;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<DetalleOficio>> GetDatosExcel(string path, OficioCreateCommand request)
        {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=NO'";
            DataTable datatable = new DataTable();

            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {

                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
                        excelconn.Close();

                        excelconn.Open();
                        cmd.CommandText = "SELECT * From [PAGO$A10:P1000]";
                        adapterexcel.SelectCommand = cmd;
                        adapterexcel.Fill(datatable);
                        excelconn.Close();
                    }
                }
            }

            return await GetModelIncidencias(datatable, request);
        }

        public async Task<List<DetalleOficio>> GetModelIncidencias(DataTable excel, OficioCreateCommand oficio)
        {
            try
            {
                DetalleOficio detalle = null;

                foreach (DataRow row in excel.Rows)
                {
                    if (row[3] != DBNull.Value)
                    {
                        if (GetFacturaId(Convert.ToInt64(row[3])) != 0 && GetCedula(Convert.ToInt64(row[3])) != 0)
                        {
                            detalle = new DetalleOficio
                            {
                                ServicioId = oficio.ServicioId,
                                OficioId = oficio.Id,
                                FacturaId = GetFacturaId(Convert.ToInt64(row[3])),
                                CedulaId = GetCedula(Convert.ToInt64(row[3])),
                            };
                            await _context.AddAsync(detalle);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                var detalles = _context.DetalleOficios.Where(dt => dt.OficioId == oficio.Id).ToList();

                return detalles;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public int GetFacturaId(long folio)
        {
            try
            {
                var factura = _context.Facturas.SingleOrDefault(f => f.Folio == folio);

                return factura != null ? factura.Id : 0;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public int GetCedula(long folio)
        {
            try
            {
                var factura = _context.Facturas.Single(f => f.Folio == folio);
                var repositorio = _context.Repositorios.Single(f => f.Id == factura.RepositorioId);

                var cedula = _context.CedulaEvaluacion.Single(c => repositorio.MesId == c.MesId && repositorio.Anio == c.Anio
                                                                && factura.InmuebleId == c.InmuebleId && repositorio.ContratoId == c.ContratoId);

                return cedula != null ? cedula.Id : 0;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        private string CargaArchivoExcel(IFormFile fromFiles, string folio)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Oficios\\" + folio;
            string dest_path = Path.Combine(newPath, "Oficios");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return path;
        }
    }

}
