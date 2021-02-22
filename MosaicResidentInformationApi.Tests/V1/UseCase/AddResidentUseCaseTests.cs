using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;
using ResidentInformationDomain = MosaicResidentInformationApi.V1.Domain.ResidentInformation;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using ResidentCannotBeAddedException = MosaicResidentInformationApi.V1.Domain.ResidentCannotBeAddedException;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

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
            var residentInformation = _fixture.Build<ResidentInformationDomain>()
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

        [Test]
        public void ThrowsResidentCannotBeAddedExceptionIfGatewayThrowsDbUpdateException()
        {
            var addResidentRequest = new AddResidentRequest()
            {
                FirstName = "Adora",
                LastName = "Grayskull",
            };
            _mockMosaicGateway.Setup(x =>
                    x.InsertResident("Adora", "Grayskull"))
                .Throws(new DbUpdateException());

            Func<ResidentInformationResponse> testDelegate = () => _classUnderTest.Execute(addResidentRequest);

            testDelegate.Should().Throw<ResidentCannotBeAddedException>();
        }
    }
}
