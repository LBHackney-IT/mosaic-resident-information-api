using AutoFixture;
using FluentAssertions;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Infrastructure;
using NUnit.Framework;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;
using DomainAddress = MosaicResidentInformationApi.V1.Domain.Address;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.Factories
{
    public class EntityFactoryTests
    {
        private Fixture _fixture;
        private EntityFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _factory = new EntityFactory();
        }

        [Test]
        public void ItMapsAPersonDatabaseRecordIntoResidentInformationDomainObject()
        {
            var personRecord = TestHelper.CreateDatabasePersonEntity();
            var domain = _factory.ToDomain(personRecord);
            domain.Should().BeEquivalentTo(new ResidentInformation
            {
                FirstName = personRecord.FirstName,
                LastName = personRecord.LastName,
                NhsNumber = personRecord.NhsNumber.ToString(),
                DateOfBirth = personRecord.DateOfBirth.ToString("O"),
            });
        }

        [Test]
        public void IfPhoneTypeCanNotBeParsedReturnsNull()
        {
            var dbPhone = new TelephoneNumber { Number = "number", Type = "not a real type" };
            _factory.ToDomain(dbPhone).Should().BeNull();
        }

        [TestCase(PhoneType.Fax, "Fax")]
        [TestCase(PhoneType.Home, "Home")]
        public void IfPhoneTypeCanBeParsedReturnsPhoneNumber(PhoneType type, string typeString)
        {
            var number = _fixture.Create<string>();
            var dbPhone = new TelephoneNumber { Number = number, Type = typeString };
            _factory.ToDomain(dbPhone).Should().BeEquivalentTo(new PhoneNumber
            {
                Number = number,
                Type = type
            });
        }

        [Test]
        public void ItMapsADatabaseAddressToADomainObject()
        {
            var dbAddress = _fixture.Create<Address>();

            _factory.ToDomain(dbAddress).Should().BeEquivalentTo(new DomainAddress
            {
                AddressLine1 = dbAddress.AddressLines,
                PostCode = dbAddress.PostCode
            });
        }
    }
}
