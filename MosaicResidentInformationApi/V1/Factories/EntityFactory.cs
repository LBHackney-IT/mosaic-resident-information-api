using System;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Factories
{
    public class EntityFactory : AbstractEntityFactory
    {
        public override ResidentInformation ToDomain(Person databaseEntity)
        {
            return new ResidentInformation
            {
                FirstName = databaseEntity.FirstName,
                LastName = databaseEntity.LastName,
                NhsNumber = databaseEntity.NhsNumber.ToString(),
                DateOfBirth = databaseEntity.DateOfBirth.ToString("O"),
            };
        }

        public override PhoneNumber ToDomain(TelephoneNumber number)
        {
            var canParseType = Enum.TryParse<PhoneType>(number.Type, out var type);
            return canParseType ? new PhoneNumber { Number = number.Number, Type = type } : null;
        }

        public override Address ToDomain(AddressSchema address)
        {
            return new Address
            {
                AddressLine1 = address.AddressLines,
                PostCode = address.PostCode
            };
        }
    }
}
