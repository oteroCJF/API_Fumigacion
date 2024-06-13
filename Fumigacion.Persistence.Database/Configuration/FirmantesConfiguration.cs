using Fumigacion.Domain.DFirmantes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fumigacion.Persistence.Database.Configuration
{
    public class FirmantesConfiguration
    {
        public FirmantesConfiguration(EntityTypeBuilder<Firmante> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
