using System;
using System.Collections.Generic;
using System.Linq;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Domain.Address;
using DbAddress = MosaicResidentInformationApi.V1.Infrastructure.Address;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static ResidentInformation ToDomain(this Person databaseEntity)
        {
            return new ResidentInformation
            {
                MosaicId = databaseEntity.Id.ToString(),
                FirstName = databaseEntity.FirstName,
                LastName = databaseEntity.LastName,
                NhsNumber = databaseEntity.NhsNumber?.ToString(),
                DateOfBirth = databaseEntity.DateOfBirth?.ToString("O"),
            };
        }
        public static List<ResidentInformation> ToDomain(this IEnumerable<Person> people)
        {
            return people.Select(p => p.ToDomain()).ToList();
        }

        public static PhoneNumber ToDomain(this TelephoneNumber number)
        {
            var canParseType = Enum.TryParse<PhoneType>(number.Type, out var type);
            return canParseType ? new PhoneNumber { Number = number.Number, Type = type } : null;
        }

        public static Address ToDomain(this DbAddress address)
        {
            return new Address
            {
                AddressLine1 = address.AddressLines,
                PostCode = address.PostCode
            };
        }
    }
}
