using System;
using System.Collections.Generic;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Boundary.Responses.Address;

namespace MosaicResidentInformationApi.Tests.V1.E2ETests
{
    public static class E2ETestHelpers
    {
        public static ResidentInformation AddPersonWithRelatesEntitiesToDb(MosaicContext context, int? id = null, string firstname = null, string lastname = null)
        {
            var person = TestHelper.CreateDatabasePersonEntity();
            if (id != null) person.Id = (int) id;
            person.FirstName = firstname ?? person.FirstName;
            person.LastName = lastname ?? person.LastName;

            var address = TestHelper.CreateDatabaseAddressForPersonId(person.Id);
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(person.Id);

            context.Persons.Add(person);
            context.SaveChanges();

            context.Addresses.Add(address);
            context.TelephoneNumbers.Add(phone);
            context.SaveChanges();
            return new ResidentInformation
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Uprn = address.Uprn.ToString(),
                NhsNumber = person.NhsNumber.ToString(),
                PhoneNumber =
                    new List<Phone>
                    {
                        new Phone {PhoneNumber = phone.Number, PhoneType = Enum.Parse<PhoneType>(phone.Type)}
                    },
                DateOfBirth = person.DateOfBirth.ToString("O"),
                AddressList = new List<Address>
                {
                    new Address {AddressLine1 = address.AddressLines, PostCode = address.PostCode}
                }
            };
        }
    }
}
