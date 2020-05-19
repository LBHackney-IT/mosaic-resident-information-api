using Microsoft.EntityFrameworkCore;
using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    public class MosaicContext : DbContext, IMosaicContext
    {
        public MosaicContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DatabaseEntity> DatabaseEntities { get; set; }

        public DbSet<ResidentInformation> ResidentDatabaseEntities { get; set; }
    }
}
