using System;
using System.Collections.Generic;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using Domain = MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;
using ResponseAddress = MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.Tests.V1.E2ETests
{
    public static class E2ETestHelpers
    {
        public static ResidentInformation AddPersonWithRelatesEntitiesToDb(MosaicContext context, int? id = null)
        {
            var person = TestHelper.CreateDatabasePersonEntity();
            if (id != null) person.Id = (int) id;
            var address = TestHelper.CreateDatabaseAddressForPersonId(person.Id);
            var phone = TestHelper.CreateDatabaseTelephoneNumberForPersonId(person.Id);

            context.Persons.Add(person);
            context.SaveChanges();

            context.Addresses.Add(address);
            context.TelephoneNumbers.Add(phone);
            context.SaveChanges();

            return new ResidentInformation()
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Uprn = address.Uprn.ToString(),
                NhsNumber = person.NhsNumber.ToString(),
                PhoneNumberList = new List<PhoneNumber>()
                {
                    new PhoneNumber {Number = phone.Number, Type = Enum.Parse<PhoneType>(phone.Type)},
                },

                DateOfBirth = person.DateOfBirth.ToString("O"),
                AddressList = new List<Domain.Address>()
                {
                    new Domain.Address{AddressLine1 = address.AddressLines, PostCode = address.PostCode},
                }
            };
        }
    }
}
