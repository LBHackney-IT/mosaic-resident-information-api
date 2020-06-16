using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetEntityByIdCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetEntityByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockMosaicGateway = new Mock<IMosaicGateway>();

            _classUnderTest = new GetEntityByIdUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformation()
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

        [Test]
        public void IfGatewayReturnsNullThrowNotFoundError()
        {
            ResidentInformation nullResult = null;
            _mockMosaicGateway.Setup(x => x.GetEntityById(It.IsAny<int>()))
                .Returns(nullResult);
            Func<ResidentInformationResponse> testDelegate = () => _classUnderTest.Execute(1);

            testDelegate.Should().Throw<ResidentNotFoundException>();
        }
    }
}
