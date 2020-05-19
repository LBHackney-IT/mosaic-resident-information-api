using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class ListResidentsReturnsAListOfAllResidents : E2ETests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ListResidentsReturnsAllResidentRecordInMosaic()
        {
            //Add some records to the database
            // var residents = _fixture.CreateMany<>()

            //Call endpoint
            var response = Client.GetAsync("api/v1/residents");
            var statusCode = response.Result.StatusCode;
            var context = response.Result.Content;

            statusCode.Should().Be(200);

            //Check records have been returned
        }
    }
}
