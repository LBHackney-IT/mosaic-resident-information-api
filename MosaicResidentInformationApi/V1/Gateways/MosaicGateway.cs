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

        //Not tested
        public List<ResidentInformation> GetAllResidentsSelect()
        {
            var results = _mosaicContext.Persons;

            return results.ToDomain();
        }

        public ResidentInformation GetEntityById(int id)
        {
            var databaseRecord = _mosaicContext.Persons.Find(id);
            if (databaseRecord == null) return null;

            var person = databaseRecord.ToDomain();

            person.PhoneNumberList = GetPhoneNumbersByPersonId(databaseRecord);

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

        private List<MosaicResidentInformationApi.V1.Domain.Address> GetAddressesByPersonId(Person person)
        {
            var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId == person.Id);
            return addressesForPerson.Select(s => s.ToDomain()).ToList();
        }

        private List<PhoneNumber> GetPhoneNumbersByPersonId(Person person)
        {
            var phoneNumbersForPerson = _mosaicContext.TelephoneNumbers.Where(n => n.PersonId == person.Id);
            return phoneNumbersForPerson.Select(n => n.ToDomain()).ToList();
        }
    }
}
