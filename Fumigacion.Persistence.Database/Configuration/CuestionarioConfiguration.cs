using Fumigacion.Domain.DCuestionario;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fumigacion.Persistence.Database.Configuration
{
    public class CuestionarioConfiguration
    {
        public CuestionarioConfiguration(EntityTypeBuilder<Cuestionario> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
