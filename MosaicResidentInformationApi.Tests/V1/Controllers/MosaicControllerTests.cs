using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Controllers;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<GetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<GetEntityByIdUseCase> _mockGetEntityByIdUseCase;

        private Mock<IMosaicGateway> _mockIMosaicGateway;

        [SetUp]
        public void SetUp()
        {
            _mockIMosaicGateway = new Mock<IMosaicGateway>();
            _mockGetAllResidentsUseCase = new Mock<GetAllResidentsUseCase>(_mockIMosaicGateway.Object);
            _mockGetEntityByIdUseCase = new Mock<GetEntityByIdUseCase>(_mockGetEntityByIdUseCase);
            _classUnderTest = new MosaicController(_mockGetAllResidentsUseCase.Object, _mockGetEntityByIdUseCase.Object);
        }

        [Test]
        public void ViewRecordTests()
        {
            var response = _classUnderTest.ViewRecord("12345") as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(new ResidentInformationList());

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
            var response = _classUnderTest.ListContacts(residentQueryParam) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(new Entity());

        }

    }
}
