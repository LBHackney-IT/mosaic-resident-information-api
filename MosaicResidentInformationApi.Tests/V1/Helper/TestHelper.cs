using AutoFixture;
using Bogus;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;

namespace MosaicResidentInformationApi.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static ResidentInformation CreateDomainResident()
        {
            var faker = new Fixture();
            return faker.Create<ResidentInformation>();
        }

        public static DatabaseEntity CreateDatabaseEntity()
        {
            var faker = new Fixture();
            return faker.Create<DatabaseEntity>();
        }
    }
}
