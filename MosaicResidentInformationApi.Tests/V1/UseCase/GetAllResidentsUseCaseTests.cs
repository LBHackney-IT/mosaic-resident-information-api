using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetAllResidentsUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new GetAllResidentsUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var expectedResponseResidents = new ResidentInformationResponse()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020"
            };

            var expectedResponse = new ResidentInformationList()
            {
                Residents = new List<ResidentInformationResponse>{ expectedResponseResidents }
            };

            var stubbedResident = new ResidentInformation
            {
                FirstName = "test", LastName = "test", DateOfBirth = "01/01/2020"
            };

            _mockMosaicGateway = new Mock<IMosaicGateway>();
            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidentsSelect())
                .Returns(new List<ResidentInformation>{ stubbedResident });

            var response = _classUnderTest.Execute();

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
