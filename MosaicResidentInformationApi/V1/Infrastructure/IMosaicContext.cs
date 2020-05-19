using Microsoft.EntityFrameworkCore;
using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    public interface IMosaicContext
    {
        DbSet<DatabaseEntity> DatabaseEntities { get; set; }
        DbSet<ResidentInformation> ResidentDatabaseEntities { get; set; }

    }

}
