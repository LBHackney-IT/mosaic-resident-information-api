using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetResidentInformationByPersonIdUseCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetResidentInformationByPersonIdUseCase _classUnderTest;
        private ResidentInformation _residentInfo;

        [SetUp]
        public void SetUp()
        {
            _residentInfo = new ResidentInformation()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020"
            };

            _mockMosaicGateway = new Mock<IMosaicGateway>();
            _mockMosaicGateway.Setup(x =>
                    x.GetResidentInformationByPersonId(12345))
                .Returns(_residentInfo);

            _classUnderTest = new GetResidentInformationByPersonIdUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var response = _classUnderTest.Execute(12345);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(_residentInfo);
        }
    }
}
