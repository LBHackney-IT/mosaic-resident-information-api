using System;
using System.Collections.Generic;
using System.Linq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public class MosaicGateway : IMosaicGateway
    {
        private readonly MosaicContext _mosaicContext;

        public MosaicGateway(MosaicContext mosaicContext)
        {
            _mosaicContext = mosaicContext;
        }

        public List<ResidentInformation> GetAllResidents()
        {
            var persons = _mosaicContext.Persons.ToList();

            var personDomain = persons.ToDomain();

            foreach (var person in personDomain)
            {
                var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId.ToString() == person.MosaicId);
                person.AddressList = addressesForPerson.Any() ? addressesForPerson.Select(s => s.ToDomain()).ToList() : null;
                var phoneNumbersForPerson = GetPhoneNumbersByPersonId(Int32.Parse(person.MosaicId));
                person.PhoneNumberList = phoneNumbersForPerson.Any() ? phoneNumbersForPerson : null;
                person.Uprn = GetMostRecentUprn(addressesForPerson);
            }

            return personDomain;
        }

        public ResidentInformation GetEntityById(int id)
        {
            var databaseRecord = _mosaicContext.Persons.Find(id);
            if (databaseRecord == null) return null;
            var person = databaseRecord.ToDomain();

            person.PhoneNumberList = GetPhoneNumbersByPersonId(databaseRecord.Id);

            var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId == databaseRecord.Id);
            person.AddressList = addressesForPerson.Select(s => s.ToDomain()).ToList();
            person.Uprn = GetMostRecentUprn(addressesForPerson);

            return person;
        }

        private static string GetMostRecentUprn(IQueryable<Address> addressesForPerson)
        {
            if (!addressesForPerson.Any()) return null;
            var currentAddress = addressesForPerson.FirstOrDefault(a => a.EndDate == null);
            if (currentAddress != null)
            {
                return currentAddress.Uprn.ToString();
            }

            return addressesForPerson.OrderByDescending(a => a.EndDate).First().Uprn.ToString();
        }

        private List<PhoneNumber> GetPhoneNumbersByPersonId(int id)
        {
            var phoneNumbersForPerson = _mosaicContext.TelephoneNumbers.Where(n => n.PersonId == id);
            return phoneNumbersForPerson.Select(n => n.ToDomain()).ToList();
        }
    }
}
