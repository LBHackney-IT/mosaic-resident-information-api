using System;
using AutoFixture;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;
using Person = MosaicResidentInformationApi.V1.Infrastructure.Person;

namespace MosaicResidentInformationApi.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static Person CreateDatabasePersonEntity(string firstname = null, string lastname = null, long? id = null)
        {
            var faker = new Fixture();
            var fp = faker.Build<Person>()
                .Without(p => p.Id)
                .Create();
            fp.DateOfBirth = new DateTime
                (fp.DateOfBirth.Value.Year, fp.DateOfBirth.Value.Month, fp.DateOfBirth.Value.Day);
            fp.FirstName = firstname ?? fp.FirstName;
            fp.LastName = lastname ?? fp.LastName;
            if (id != null) fp.Id = (int) id;

            return fp;
        }

        public static Address CreateDatabaseAddressForPersonId(long personId, string postcode = null, string address = null)
        {
            var faker = new Fixture();

            var fa = faker.Build<Address>()
                .With(add => add.PersonId, personId)
                .Without(add => add.PersonAddressId)
                .Without(add => add.Person)
                .Create();

            fa.PostCode = postcode ?? fa.PostCode;
            fa.AddressLines = address ?? fa.AddressLines;
            return fa;
        }

        public static TelephoneNumber CreateDatabaseTelephoneNumberForPersonId(long personId)
        {
            var faker = new Fixture();

            return faker.Build<TelephoneNumber>()
                .With(tel => tel.PersonId, personId)
                .With(tel => tel.Type, PhoneType.Mobile.ToString)
                .Without(tel => tel.Id)
                .Without(tel => tel.Person)
                .Create();
        }
    }
}
