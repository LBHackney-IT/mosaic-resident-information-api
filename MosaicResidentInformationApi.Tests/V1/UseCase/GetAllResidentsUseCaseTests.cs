using System.Collections.Generic;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;
using AutoFixture;
using MosaicResidentInformationApi.V1.Factories;
using System.Linq;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockMosaicGateway = new Mock<IMosaicGateway>();
            _classUnderTest = new GetAllResidentsUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>();
            var expectedResponse = new ResidentInformationList()
            {
                Residents = stubbedResidents.ToResponse()
            };

            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidents())
                .Returns(stubbedResidents.ToList());

            var response = _classUnderTest.Execute();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
