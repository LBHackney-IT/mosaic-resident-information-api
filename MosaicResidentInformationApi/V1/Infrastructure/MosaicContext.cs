using Microsoft.EntityFrameworkCore;
using MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    public class MosaicContext : DbContext
    {
        public MosaicContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<TelephoneNumber> TelephoneNumbers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DatabaseEntity> DatabaseEntities { get; set; }

        public DbSet<ResidentInformation> ResidentDatabaseEntities { get; set; }
    }
}
