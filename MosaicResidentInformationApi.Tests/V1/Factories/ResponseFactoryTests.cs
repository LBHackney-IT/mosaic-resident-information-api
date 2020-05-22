using System.Collections.Generic;
using FluentAssertions;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using NUnit.Framework;
using Address = MosaicResidentInformationApi.V1.Domain.Address;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

namespace MosaicResidentInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTests
    {
        [Test]
        public void CanMapResidentInformationFromDomainToResponse()
        {
            var domain = new ResidentInformation
            {
                Uprn = "uprn",
                AddressList = new List<Address>
                {
                    new Address
                    {
                        AddressLine1 = "addess11",
                        AddressLine2 = "address22",
                        AddressLine3 = "address33",
                        PostCode = "Postcode"
                    }
                },
                FirstName = "Name",
                LastName = "Last",
                NhsNumber = "nhs",
                DateOfBirth = "DOB",
                PhoneNumberList = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = "number",
                        Type = PhoneType.Fax
                    }
                },
            };

            var expectedResponse = new ResidentInformationResponse
            {
                Uprn = "uprn",
                AddressList = new List<MosaicResidentInformationApi.V1.Boundary.Responses.Address>
                {
                    new MosaicResidentInformationApi.V1.Boundary.Responses.Address()
                    {
                        AddressLine1 = "addess11",
                        AddressLine2 = "address22",
                        AddressLine3 = "address33",
                        PostCode = "Postcode"
                    }
                },
                FirstName = "Name",
                LastName = "Last",
                NhsNumber = "nhs",
                DateOfBirth = "DOB",
                PhoneNumber = new List<Phone>
                {
                    new Phone
                    {
                        PhoneNumber = "number",
                        PhoneType = PhoneType.Fax
                    }
                },
            };
            domain.ToResponse().Should().BeEquivalentTo(expectedResponse);
        }
    }
}
