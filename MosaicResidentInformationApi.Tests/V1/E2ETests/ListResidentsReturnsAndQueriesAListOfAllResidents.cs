using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class ListResidentsReturnsAndQueriesAListOfAllResidents : E2ETests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task IfNoQueryParametersReturnsAllResidentRecordsFromMosaic()
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
        public async Task FirstNameLastNameQueryParametersReturnsMatchingResidentRecordsFromMosaic()
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

        [Test]
        public async Task PostcodeAndAddressQueryParametersReturnsMatchingResidentsRecordsFromMosaic()
        {
            var matchingResidentOne = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, postcode: "ER 1RR", addressLines: "1 Seasame street, Hackney, LDN");
            var matchingResidentTwo = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, postcode: "ER 1RR", addressLines: "1 Seasame street");
            var nonMatchingResident1 = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, postcode: "E4 1RR");
            var nonMatchingResident2 = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, addressLines: "1 Seasame street, Hackney, LDN");
            var nonMatchingResident3 = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext);

            var response = Client.GetAsync("api/v1/residents?post_code=er1rr&address=1 Seasame street");

            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync();
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformationList>(stringContent);

            convertedResponse.Residents.Count.Should().Be(2);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResidentOne);
            convertedResponse.Residents.Should().ContainEquivalentOf(matchingResidentTwo);
        }

        [Test]
        public async Task UsingAllQueryParametersReturnsMatchingResidentsRecordsFromMosaic()
        {

        }
    }
}
