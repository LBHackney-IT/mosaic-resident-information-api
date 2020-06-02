using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

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

            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidents(3, 15, "ciasom", "tessellate", "E8 1DY", "1 Montage street"))
                .Returns(stubbedResidents.ToList());
            var rqp = new ResidentQueryParam
            {
                FirstName = "ciasom",
                LastName = "tessellate",
                Postcode = "E8 1DY",
                Address = "1 Montage street"
            };

            var response = _classUnderTest.Execute(rqp, 3, 15);

            response.Should().NotBeNull();
            response.Residents.Should().BeEquivalentTo(stubbedResidents.ToResponse());
        }

        [Test]
        public void IfLimitLessThanTheMinimum_WillUseTheMinimumLimit()
        {
            _mockMosaicGateway.Setup(x => x.GetAllResidents(0, 10, null, null, null, null))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam(), 0, 4);

            _mockMosaicGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximum_WillUseTheMaximumLimit()
        {
            _mockMosaicGateway.Setup(x => x.GetAllResidents(0, 100, null, null, null, null))
                .Returns(new List<ResidentInformation>()).Verifiable();

            _classUnderTest.Execute(new ResidentQueryParam(), 0, 400);

            _mockMosaicGateway.Verify();
        }

        [Test]
        public void ReturnsTheNextCursor()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(10);

            var expectedNextCursor = stubbedResidents.Max(r => r.MosaicId);

            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidents(0, 10, null, null, null, null))
                .Returns(stubbedResidents.ToList());

            _classUnderTest.Execute(new ResidentQueryParam(), 0, 10).NextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        public void WhenAtTheEndOfTheResidentList_ReturnsTheNextCursorAsEmptyString()
        {
            var stubbedResidents = _fixture.CreateMany<ResidentInformation>(7);

            _mockMosaicGateway.Setup(x =>
                    x.GetAllResidents(0, 10, null, null, null, null))
                .Returns(stubbedResidents.ToList());

            _classUnderTest.Execute(new ResidentQueryParam(), 0, 10).NextCursor.Should().Be("");
        }
    }
}
