using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;
using DomainAddress = MosaicResidentInformationApi.V1.Domain.Address;
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

        public List<ResidentInformation> GetAllResidents(int cursor, int limit, long? id = null, string firstname = null,
            string lastname = null, string postcode = null, string address = null, string contextflag = null)
        {
            if (id != null && id.HasValue)
            {
                var mosaicId = id.Value;
                var resident = GetEntityById(mosaicId);
                return new List<ResidentInformation> { resident };
            }
            else
            {
                var addressSearchPattern = GetSearchPattern(address);
                var postcodeSearchPattern = GetSearchPattern(postcode);

                var queryByAddress = postcode != null || address != null;

                var peopleIds = queryByAddress
                    ? PeopleIdsForAddressQuery(cursor, limit, firstname, lastname, postcode, address, contextflag)
                    : PeopleIds(cursor, limit, firstname, lastname, contextflag);

                var dbRecords = _mosaicContext.Persons
                    .Where(p => peopleIds.Contains(p.Id))
                    .Select(p => new
                    {
                        Person = p,
                        Addresses = _mosaicContext
                            .Addresses
                            .Where(add => add.PersonId == p.Id)
                            .Where(add =>
                                string.IsNullOrEmpty(address) || EF.Functions.ILike(add.AddressLines.Replace(" ", ""), addressSearchPattern))
                            .Where(add =>
                                string.IsNullOrEmpty(postcode) || EF.Functions.ILike(add.PostCode.Replace(" ", ""), postcodeSearchPattern))
                            .Distinct()
                            .ToList(),
                        TelephoneNumbers = _mosaicContext.TelephoneNumbers.Where(n => n.PersonId == p.Id).Distinct().ToList()
                    }).ToList();

                return dbRecords.Select(x => MapPersonAndAddressesToResidentInformation(x.Person, x.Addresses, x.TelephoneNumbers)
                ).ToList();
            }
        }

        public ResidentInformation GetEntityById(long id)
        {
            var databaseRecord = _mosaicContext.Persons.Find(id);
            if (databaseRecord == null) return null;

            var addressesForPerson = _mosaicContext.Addresses.Where(a => a.PersonId == databaseRecord.Id);
            var person = MapPersonAndAddressesToResidentInformation(databaseRecord, addressesForPerson);
            AttachPhoneNumberToPerson(person);

            return person;
        }

        private List<long> PeopleIds(int cursor, int limit, string firstname, string lastname, string contextflag)
        {
            var firstNameSearchPattern = GetSearchPattern(firstname);
            var lastNameSearchPattern = GetSearchPattern(lastname);
            var contextFlagSearchPattern = GetSearchPattern(contextflag);

            return _mosaicContext.Persons
                .Where(person => person.Id > cursor)
                .Where(person =>
                    string.IsNullOrEmpty(firstname) || EF.Functions.ILike(person.FirstName, firstNameSearchPattern))
                .Where(person =>
                    string.IsNullOrEmpty(lastname) || EF.Functions.ILike(person.LastName, lastNameSearchPattern))
                .Where(person =>
                    string.IsNullOrEmpty(contextflag) || EF.Functions.ILike(person.AgeContext, contextFlagSearchPattern))
                .Take(limit)
                .Select(p => p.Id)
                .ToList();
        }

        private List<long> PeopleIdsForAddressQuery(int cursor, int limit, string firstname, string lastname, string postcode, string address, string contextflag)
        {
            var firstNameSearchPattern = GetSearchPattern(firstname);
            var lastNameSearchPattern = GetSearchPattern(lastname);
            var addressSearchPattern = GetSearchPattern(address);
            var postcodeSearchPattern = GetSearchPattern(postcode);
            var contextFlagSearchPattern = GetSearchPattern(contextflag);

            return _mosaicContext.Addresses
                .Where(add =>
                    string.IsNullOrEmpty(address) || EF.Functions.ILike(add.AddressLines.Replace(" ", ""), addressSearchPattern))
                .Where(add =>
                    string.IsNullOrEmpty(postcode) || EF.Functions.ILike(add.PostCode.Replace(" ", ""), postcodeSearchPattern))
                .Where(add =>
                    string.IsNullOrEmpty(firstname) || EF.Functions.ILike(add.Person.FirstName, firstNameSearchPattern))
                .Where(add =>
                    string.IsNullOrEmpty(lastname) || EF.Functions.ILike(add.Person.LastName, lastNameSearchPattern))
                .Where(add =>
                    string.IsNullOrEmpty(contextflag) || EF.Functions.ILike(add.Person.AgeContext, contextFlagSearchPattern))
                .Include(add => add.Person)
                .Where(add => add.PersonId > cursor)
                .GroupBy(add => add.PersonId)
                .Where(p => p.Key != null)
                .Take(limit)
                .Select(p => (long) p.Key)
                .ToList();
        }

        private ResidentInformation AttachPhoneNumberToPerson(ResidentInformation person)
        {
            var phoneNumbersForPerson = _mosaicContext.TelephoneNumbers
                .Where(n => n.PersonId == int.Parse(person.MosaicId));
            person.PhoneNumberList = phoneNumbersForPerson.Any() ? phoneNumbersForPerson.Select(n => n.ToDomain()).ToList() : null;
            return person;
        }

        private static ResidentInformation MapPersonAndAddressesToResidentInformation(Person person,
            IEnumerable<Address> addresses, IEnumerable<TelephoneNumber> numbers = null)
        {
            var resident = person.ToDomain();
            var addressesDomain = addresses.Select(address => address.ToDomain()).ToList();
            resident.Uprn = GetMostRecentUprn(addresses);
            resident.AddressList = addressesDomain;
            resident.AddressList = addressesDomain.Any()
                ? addressesDomain
                : null;
            resident.PhoneNumberList = numbers == null || !numbers.Any()
                ? null
                : numbers.Select(n => n.ToDomain()).ToList();
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

        private static string GetSearchPattern(string str)
        {
            return $"%{str?.Replace(" ", "")}%";
        }
    }

}
