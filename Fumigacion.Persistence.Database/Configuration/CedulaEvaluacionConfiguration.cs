using Fumigacion.Domain.DCedulaEvaluacion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fumigacion.Persistence.Database.Configuration
{
    public class CedulaEvaluacionConfiguration
    {
        public CedulaEvaluacionConfiguration(EntityTypeBuilder<CedulaEvaluacion> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
