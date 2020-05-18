using System.Collections.Generic;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using ResidentInformation = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetAllResidentsUseCaseTest
    {
        private Mock<IMosaicGateway> _mockMosaicGateway;
        private GetAllResidentsUseCase _classUnderTest;
        private ResidentInformation _residentInfo;
        private ResidentInformationList _residentInformationList;
        private ResidentQueryParam _residentQueryParam;

        [SetUp]
        public void SetUp()
        {
            _residentInfo = new ResidentInformation()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020"
            };

            List<ResidentInformation> residentInfo = new List<ResidentInformation>()
            {
                _residentInfo
            };

            _residentInformationList = new ResidentInformationList()
            {
                Residents = residentInfo
            };

            _residentQueryParam = new ResidentQueryParam()
            {
                FirstName = "test",
                LastName = "test1",
                Address = "1 Hillman Street",
                PostCode = "E8 1DY"
            };

            _mockMosaicGateway = new Mock<IMosaicGateway>();
            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidentsSelect(_residentQueryParam))
                .Returns(_residentInformationList);

            _classUnderTest = new GetAllResidentsUseCase(_mockMosaicGateway.Object);
        }

        [Test]
        public void ReturnsResidentInformationList()
        {
            var response = _classUnderTest.Execute(_residentQueryParam);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(_residentInformationList);
        }
    }
}
