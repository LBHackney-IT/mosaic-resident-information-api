using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using AutoFixture;
using MosaicResidentInformationApi.V1.Factories;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetEntityByIdCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetEntityByIdUseCase _classUnderTest;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockMosaicGateway = new Mock<IMosaicGateway>();

            _classUnderTest = new GetEntityByIdUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var stubbedResidentInfo = _fixture.Create<ResidentInformation>();

            _mockMosaicGateway.Setup(x =>
                    x.GetEntityById(12345))
                .Returns(stubbedResidentInfo);

            var response = _classUnderTest.Execute(12345);
            var expectedResponse = stubbedResidentInfo.ToResponse();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
