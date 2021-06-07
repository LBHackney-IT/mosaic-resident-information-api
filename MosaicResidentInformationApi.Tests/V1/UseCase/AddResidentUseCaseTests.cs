using AutoFixture;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class AddResidentUseCaseTests
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private AddResidentUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockMosaicGateway = new Mock<IMosaicGateway>();

            _classUnderTest = new AddResidentUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformation()
        {
            var residentInformation = _fixture.Build<ResidentInformation>()
                .With(r => r.FirstName, "Adora")
                .With(r => r.LastName, "Grayskull")
                .Create();
            var addResidentRequest = new AddResidentRequest()
            {
                FirstName = "Adora",
                LastName = "Grayskull",
            };
            _mockMosaicGateway.Setup(x =>
                    x.InsertResident("Adora", "Grayskull"))
                .Returns(residentInformation);

            var response = _classUnderTest.Execute(addResidentRequest);

            response.FirstName.Should().Be("Adora");
            response.LastName.Should().Be("Grayskull");
        }
    }
}
