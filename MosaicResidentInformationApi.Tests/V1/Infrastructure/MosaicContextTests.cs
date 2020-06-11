using System;
using System.Linq;
using FluentAssertions;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class MosaicContextTests : DatabaseTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();

            // databaseEntity.Id = null;
            Console.WriteLine(JsonConvert.SerializeObject(databaseEntity));

            MosaicContext.Add(databaseEntity);
            MosaicContext.SaveChanges();

            var result = MosaicContext.Persons.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
            true.Should().BeFalse();
        }
    }
}
