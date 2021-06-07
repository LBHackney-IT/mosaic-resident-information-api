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

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ItMapsAPersonDatabaseRecordIntoResidentInformationDomainObject()
        {
            var personRecord = TestHelper.CreateDatabasePersonEntity();
            var domain = personRecord.ToDomain();
            domain.Should().BeEquivalentTo(new ResidentInformation
            {
                MosaicId = personRecord.Id.ToString(),
                FirstName = personRecord.FirstName,
                LastName = personRecord.LastName,
                NhsNumber = personRecord.NhsNumber.ToString(),
                DateOfBirth = personRecord.DateOfBirth?.ToString("O"),
                AgeContext = personRecord.AgeContext,
                Nationality = personRecord.Nationality,
                Gender = personRecord.Gender,
                Restricted = personRecord.Restricted
            });
        }

        [TestCase("Fax")]
        [TestCase("Home")]
        public void ItMapsTelephoneDetailsToDomainObject(string phoneType)
        {
            var number = _fixture.Create<string>();
            var dbPhone = new TelephoneNumber { Number = number, Type = phoneType };
            dbPhone.ToDomain().Should().BeEquivalentTo(new PhoneNumber
            {
                Number = number,
                Type = phoneType
            });
        }

        [Test]
        public void ItMapsADatabaseAddressToADomainObject()
        {
            var dbAddress = _fixture.Create<Address>();

            dbAddress.ToDomain().Should().BeEquivalentTo(new DomainAddress
            {
                AddressLine1 = dbAddress.AddressLines,
                PostCode = dbAddress.PostCode,
                EndDate = dbAddress.EndDate,
                ContactAddressFlag = dbAddress.ContactAddressFlag,
                DisplayAddressFlag = dbAddress.DisplayAddressFlag
            });
        }
    }
}
