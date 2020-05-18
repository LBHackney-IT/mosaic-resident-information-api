using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MosaicResidentInformationApi.V1.Controllers;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MosaicResidentInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class MosaicControllerTests
    {
        private MosaicController _classUnderTest;
        private Mock<GetAllResidentsUseCase> _mock;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<GetAllResidentsUseCase>();
            _classUnderTest = new MosaicController(_mock.Object);
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
