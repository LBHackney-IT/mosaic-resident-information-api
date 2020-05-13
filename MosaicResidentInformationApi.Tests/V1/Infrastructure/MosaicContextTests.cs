using System.Linq;
using FluentAssertions;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Infrastructure;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class MosaicContextTests : DatabaseTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = DatabaseEntityHelper.CreateDatabaseEntity();

            _mosaicContext.Add(databaseEntity);
            _mosaicContext.SaveChanges();

            var result = _mosaicContext.DatabaseEntities.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
