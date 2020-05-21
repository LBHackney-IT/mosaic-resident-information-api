using Microsoft.EntityFrameworkCore;
using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    public class MosaicContext : DbContext
    {
        public MosaicContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation> ResidentDatabaseEntities { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<TelephoneNumber> TelephoneNumbers { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
