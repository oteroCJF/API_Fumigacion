using Fumigacion.Domain.DHistorial;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fumigacion.Persistence.Database.Configuration
{
    public class LogCedulasConfiguration
    {
        public LogCedulasConfiguration(EntityTypeBuilder<LogCedula> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
