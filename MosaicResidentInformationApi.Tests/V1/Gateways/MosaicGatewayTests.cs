using Bogus;
using FluentAssertions;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Gateways;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class MosaicGatewayTests : DatabaseTests
    {
        private readonly Faker _faker = new Faker();
        private MosaicGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new MosaicGateway(MosaicContext);
        }

        [Test]
        public void GetEntityById_ReturnsEmptyArray()
        {
            var response = _classUnderTest.GetEntityById(123);

            response.Should().BeNull();
        }

        [Test]
        public void GetEntityById_ReturnsCorrectResponse()
        {
            var databaseEntity = TestHelper.CreateDatabaseEntity();

            MosaicContext.DatabaseEntities.Add(databaseEntity);
            MosaicContext.SaveChanges();

            var response = _classUnderTest.GetEntityById(databaseEntity.Id);

            response.Should().NotBe(null);
        }
    }
}
