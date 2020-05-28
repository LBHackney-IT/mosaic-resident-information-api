using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public List<ResidentInformation> GetAllResidents(string firstname = null, string lastname = null, string postcode = null, string address = null)
        {
            var addressesFilteredByPostcode = _mosaicContext.Addresses
                .Include(p => p.Person)
                .Where(a => string.IsNullOrEmpty(address) || a.AddressLines.ToLower().Replace(" ", "").Contains(StripString(address)))
                .Where(a => string.IsNullOrEmpty(postcode) || a.PostCode.ToLower().Replace(" ", "").Equals(StripString(postcode)))
                .Where(a => string.IsNullOrEmpty(firstname) || a.Person.FirstName.ToLower().Replace(" ", "").Equals(StripString(firstname)))
                .Where(a => string.IsNullOrEmpty(lastname) || a.Person.LastName.ToLower().Replace(" ", "").Equals(StripString(lastname)))
                .ToList();

            var peopleWithAddresses = addressesFilteredByPostcode
                .GroupBy(address => address.Person, MapPersonAndAddressesToResidentInformation)
                .ToList();

            var peopleWithNoAddress = string.IsNullOrEmpty(postcode) && string.IsNullOrEmpty(address)
                ? QueryPeopleWithNoAddressByName(firstname, lastname, addressesFilteredByPostcode)
                : new List<ResidentInformation>();

            var allPeople = peopleWithAddresses.Concat(peopleWithNoAddress);

            return allPeople.Select(AttachPhoneNumberToPerson).ToList();
        }

        public ResidentInformation GetEntityById(int id)
        {
            var databaseRecord = _mosaicContext.Persons.Find(id);
            if (databaseRecord == null) return null;

            var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId == databaseRecord.Id);
            var person = MapPersonAndAddressesToResidentInformation(databaseRecord, addressesForPerson);
            AttachPhoneNumberToPerson(person);

            return person;
        }
        private List<ResidentInformation> QueryPeopleWithNoAddressByName(string firstname, string lastname, List<Address> addressesFilteredByPostcode)
        {
            return _mosaicContext.Persons
                .Where(p => string.IsNullOrEmpty(firstname) || p.FirstName.ToLower().Equals(firstname.ToLower()))
                .Where(p => string.IsNullOrEmpty(lastname) || p.LastName.ToLower().Equals(lastname.ToLower()))
                .ToList()
                .Where(p => addressesFilteredByPostcode.All(add => add.PersonId != p.Id))
                .Select(person =>
                {
                    var domainPerson = person.ToDomain();
                    domainPerson.AddressList = null;
                    return domainPerson;
                }).ToList();
        }

        private ResidentInformation AttachPhoneNumberToPerson(ResidentInformation person)
        {
            var phoneNumbersForPerson = _mosaicContext.TelephoneNumbers
                .Where(n => n.PersonId == int.Parse(person.MosaicId));
            person.PhoneNumberList = phoneNumbersForPerson.Any() ? phoneNumbersForPerson.Select(n => n.ToDomain()).ToList() : null;
            return person;
        }

        private static ResidentInformation MapPersonAndAddressesToResidentInformation(Person person,
            IEnumerable<Address> addresses)
        {
            var resident = person.ToDomain();
            var addressesDomain = addresses.Select(address => address.ToDomain()).ToList();
            resident.Uprn = GetMostRecentUprn(addresses);
            resident.AddressList = addressesDomain;
            resident.AddressList = addressesDomain.Any()
                ? addressesDomain
                : null;
            return resident;
        }
        private static string GetMostRecentUprn(IEnumerable<Address> addressesForPerson)
        {
            if (!addressesForPerson.Any()) return null;
            var currentAddress = addressesForPerson.FirstOrDefault(a => a.EndDate == null);
            if (currentAddress != null)
            {
                return currentAddress.Uprn.ToString();
            }

            return addressesForPerson.OrderByDescending(a => a.EndDate).First().Uprn.ToString();
        }

        private static string StripString(string str)
        {
            return str?.ToLower().Replace(" ", "");
        }
    }
}
