using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using Newtonsoft.Json;
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
        public async Task IfNoQueryParamaetersListResidentsReturnsAllResidentRecordInMosaic()
        {
            var expectedResidentResponseOne = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext);
            var expectedResidentResponseTwo = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext);
            var expectedResidentResponseThree = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext);

            var response = Client.GetAsync("api/v1/residents");

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync();
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseOne);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseTwo);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseThree);
        }


        [Test]
        public async Task FirstNameLastNameQueryParametersListResidentsReturnsMatchingResidentRecordInMosaic()
        {
            var expectedResidentResponseOne = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, firstname: "ciasom", lastname: "tessellate");
            var expectedResidentResponseTwo = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, firstname: "ciasom", lastname: "shape");
            var expectedResidentResponseThree = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext);

            var response = Client.GetAsync("api/v1/residents?first_name=ciasom&last_name=tessellate");

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync();
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(1);
            convertedResponse.Residents.Should().ContainEquivalentOf(expectedResidentResponseOne);
        }
    }
}
