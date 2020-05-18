using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Controllers;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<GetAllResidentsUseCase> _mockGetAllResidentsUseCase;
        private Mock<IMosaicGateway> _mockIMosaicGateway;

        [SetUp]
        public void SetUp()
        {
            _mockIMosaicGateway = new Mock<IMosaicGateway>();
            _mockGetAllResidentsUseCase = new Mock<GetAllResidentsUseCase>(_mockIMosaicGateway.Object);
            _classUnderTest = new MosaicController(_mockGetAllResidentsUseCase.Object);
        }

        [Test]
        public void ViewRecordTests()
        {
            //TODO
        }

        [Test]
        public void ListContacts()
        {
            //TODO
        }

    }
}
