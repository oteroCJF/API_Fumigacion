﻿using Fumigacion.Persistence.Database;
using Fumigacion.Domain.DContratos;
using Fumigacion.Service.EventHandler.Commands.Contratos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fumigacion.Service.EventHandler.Handlers.Contratos
{
    public class ContratoUpdateEventHandler : IRequestHandler<ContratoUpdateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ContratoUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ContratoUpdateCommand request, CancellationToken cancellationToken)
        {
            Contrato contrato = await _context.Contratos.SingleOrDefaultAsync(c => c.Id == request.Id);

            contrato.UsuarioId = request.UsuarioId;
            contrato.NoContrato = request.NoContrato;
            contrato.Empresa = request.Empresa;
            contrato.RFC = request.RFC;
            contrato.Direccion = request.Direccion;
            contrato.MontoMin = request.MontoMin;
            contrato.MontoMax = request.MontoMax;
            contrato.VolumetriaMin = request.VolumetriaMin;
            contrato.VolumetriaMax = request.VolumetriaMax;
            contrato.Representante = request.Representante;
            contrato.InicioVigencia = request.InicioVigencia;
            contrato.FinVigencia = request.FinVigencia;
            contrato.FechaFirmaContrato = request.FechaFirmaContrato;
            contrato.FechaRecepcion = request.FechaRecepcion;
            contrato.Activo = request.Activo;
            contrato.FechaActualizacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return 201;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 500;
            }
        }
    }
}
