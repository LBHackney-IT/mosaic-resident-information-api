using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;
using ResidentInformation = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetEntityByIdUseCase> _mockGetEntityByIdUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetEntityByIdUseCase = new Mock<IGetEntityByIdUseCase>();
            _classUnderTest = new MosaicController(_mockGetAllResidentsUseCase.Object, _mockGetEntityByIdUseCase.Object);
        }

        [Test]
        public void ViewRecordTests()
        {
            var residentInfo = new ResidentInformation()
            {
                FirstName = "test",
                LastName = "test",
                DateOfBirth = "01/01/2020"
            };

            _mockGetEntityByIdUseCase.Setup(x => x.Execute(12345)).Returns(residentInfo);
            var response = _classUnderTest.ViewRecord(12345) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(residentInfo);
        }

        [Test]
        public void ListContacts()
        {
            var residentInfo = new List<ResidentInformation>()
            {
                new ResidentInformation()
                {
                    FirstName = "test",
                    LastName = "test",
                    DateOfBirth = "01/01/2020"
                }
            };

            var residentInformationList = new ResidentInformationList()
            {
                Residents = residentInfo
            };

            _mockGetAllResidentsUseCase.Setup(x => x.Execute()).Returns(residentInformationList);
            var response = _classUnderTest.ListContacts() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(residentInformationList);
        }

    }
}
