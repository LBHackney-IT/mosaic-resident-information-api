using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Controllers;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Domain;
using System.Collections.Generic;
using MosaicResidentInformationApi.V1.Boundary.Responses;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetEntityByIdUseCase> _mockGetEntityByIdUseCase;

        private Mock<IMosaicGateway> _mockIMosaicGateway;
        private ResidentInformation _residentInfo;

        [SetUp]
        public void SetUp()
        {
            _mockIMosaicGateway = new Mock<IMosaicGateway>();
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetEntityByIdUseCase = new Mock<IGetEntityByIdUseCase>();
            _classUnderTest = new MosaicController(_mockGetAllResidentsUseCase.Object, _mockGetEntityByIdUseCase.Object);
            _residentInfo = new ResidentInformation()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020"
            };
        }

        [Test]
        public void ViewRecordTests()
        {
            _mockGetEntityByIdUseCase.Setup(x => x.Execute(12345)).Returns(_residentInfo);
            var response = _classUnderTest.ViewRecord(12345) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().Equals(_residentInfo);
        }

        [Test]
        public void ListContacts()
        {
            ResidentQueryParam residentQueryParam = new ResidentQueryParam()
            {
                FirstName = "test",
                LastName = "test1",
                Address = "1 Hillman Street",
                PostCode = "E8 1DY"
            };
     
            List<ResidentInformation> residentInfo = new List<ResidentInformation>()
            {
                _residentInfo
            };
            
            ResidentInformationList residentInformationList = new ResidentInformationList()
            {
                Residents = residentInfo
            };

            _mockGetAllResidentsUseCase.Setup(x => x.Execute(residentQueryParam)).Returns(residentInformationList);
            var response = _classUnderTest.ListContacts(residentQueryParam) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().Equals(residentInformationList);
        }

    }
}
