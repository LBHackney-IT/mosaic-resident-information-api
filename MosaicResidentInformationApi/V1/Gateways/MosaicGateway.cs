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
        private readonly EntityFactory _entityFactory;

        public MosaicGateway(MosaicContext mosaicContext)
        {
            _mosaicContext = mosaicContext;
            _entityFactory = new EntityFactory();
        }

        public ResidentInformationList GetAllResidentsSelect(ResidentQueryParam rqp)
        {
            var results = _mosaicContext.ResidentDatabaseEntities
                            .Where(res => res.FirstName.Equals(rqp.FirstName) || res.LastName.Equals(rqp.LastName))
                            .ToList();

            return new ResidentInformationList() { Residents = results };
        }

        public ResidentInformation GetEntityById(int id)
        {
            var databaseRecord = _mosaicContext.Persons.Find(id);
            if (databaseRecord == null) return null;

            var person = _entityFactory.ToDomain(databaseRecord);

            person.PhoneNumberList = GetPhoneNumbersByPersonId(databaseRecord);

            var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId == databaseRecord.Id);
            person.AddressList = addressesForPerson.Select(s => _entityFactory.ToDomain(s)).ToList();
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
            return addressesForPerson.Select(s => _entityFactory.ToDomain(s)).ToList();
        }

        private List<PhoneNumber> GetPhoneNumbersByPersonId(Person person)
        {
            var phoneNumbersForPerson = _mosaicContext.TelephoneNumbers.Where(n => n.PersonId == person.Id);
            return phoneNumbersForPerson.Select(n => _entityFactory.ToDomain(n)).ToList();
        }


    }
}
