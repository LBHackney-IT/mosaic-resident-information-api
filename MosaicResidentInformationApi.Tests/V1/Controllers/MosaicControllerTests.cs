using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Controllers;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using ResidentCannotBeAddedException = MosaicResidentInformationApi.V1.Domain.ResidentCannotBeAddedException;

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<IGetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IGetEntityByIdUseCase> _mockGetEntityByIdUseCase;
        private Mock<IAddResidentUseCase> _mockAddResidentUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllResidentsUseCase = new Mock<IGetAllResidentsUseCase>();
            _mockGetEntityByIdUseCase = new Mock<IGetEntityByIdUseCase>();
            _mockAddResidentUseCase = new Mock<IAddResidentUseCase>();
            _classUnderTest = new MosaicController(_mockGetAllResidentsUseCase.Object, _mockGetEntityByIdUseCase.Object, _mockAddResidentUseCase.Object);
        }

        [Test]
        public void ViewRecordTests()
        {
            var residentInfo = new ResidentInformation()
            {
                MosaicId = "abc123",
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
                    MosaicId = "abc123",
                    FirstName = "test",
                    LastName = "test",
                    DateOfBirth = "01/01/2020"
                }
            };

            var residentInformationList = new ResidentInformationList()
            {
                Residents = residentInfo
            };

            var rqp = new ResidentQueryParam
            {
                FirstName = "Ciasom",
                LastName = "Tessellate",
            };

            _mockGetAllResidentsUseCase.Setup(x => x.Execute(rqp, 3, 2)).Returns(residentInformationList);
            var response = _classUnderTest.ListContacts(rqp, 3, 2) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(residentInformationList);
        }

        [Test]
        public void AddResidentReturns201IfSuccessful()
        {
            var addResidentRequest = new AddResidentRequest();
            var residentInformation = new ResidentInformation()
            {
                FirstName = "Adora",
                LastName = "Grayskull",
            };
            _mockAddResidentUseCase.Setup(x => x.Execute(addResidentRequest)).Returns(residentInformation);

            var response = _classUnderTest.AddResident(addResidentRequest) as CreatedAtActionResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(201);
            response.Value.Should().BeEquivalentTo(residentInformation);
        }

        [Test]
        public void AddResidentReturns500IfResidentCannotBeAddedExceptionIsCaught()
        {
            var addResidentRequest = new AddResidentRequest();
            _mockAddResidentUseCase.Setup(x => x.Execute(addResidentRequest)).Throws(new ResidentCannotBeAddedException());

            var response = _classUnderTest.AddResident(addResidentRequest) as ObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Error: Unable to create resident.");
        }
    }
}
