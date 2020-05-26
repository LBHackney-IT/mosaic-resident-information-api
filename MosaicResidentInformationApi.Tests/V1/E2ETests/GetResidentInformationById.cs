using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class GetResidentInformationById : E2ETests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetResidentInformationByIdReturnsTheCorrectInformation()
        {
            var personId = _fixture.Create<int>();
            var expectedResponse = E2ETestHelpers.AddPersonWithRelatesEntitiesToDb(MosaicContext, personId);

            var response = Client.GetAsync($"api/v1/residents/{personId}");
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync();
            var convertedResponse = JsonConvert.DeserializeObject<ResidentInformation>(stringContent);

            convertedResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
