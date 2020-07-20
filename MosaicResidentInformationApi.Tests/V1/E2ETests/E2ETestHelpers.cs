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
        public static ResidentInformation AddPersonWithRelatedEntitiesToDb(MosaicContext context, int? id = null, string firstname = null, string lastname = null, string postcode = null, string addressLines = null)
        {
            var person = TestHelper.CreateDatabasePersonEntity(firstname, lastname, id);
            var addedPerson = context.Persons.Add(person);
            context.SaveChanges();

            var address = TestHelper.CreateDatabaseAddressForPersonId(addedPerson.Entity.Id, address: addressLines, postcode: postcode);
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(addedPerson.Entity.Id);

            context.Addresses.Add(address);
            context.TelephoneNumbers.Add(phone);
            context.SaveChanges();

            return new ResidentInformation
            {
                MosaicId = person.Id.ToString(),
                FirstName = person.FirstName,
                LastName = person.LastName,
                Uprn = address.Uprn.ToString(),
                NhsNumber = person.NhsNumber.ToString(),
                PhoneNumber =
                    new List<Phone>
                    {
                        new Phone {PhoneNumber = phone.Number, PhoneType = phone.Type}
                    },
                DateOfBirth = person.DateOfBirth?.ToString("O"),
                AddressList = new List<Address>
                {
                    new Address {AddressLine1 = address.AddressLines, PostCode = address.PostCode}
                }
            };
        }
    }
}
