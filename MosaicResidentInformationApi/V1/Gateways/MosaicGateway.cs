using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Infrastructure;
using Newtonsoft.Json;
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
            Console.WriteLine("In gateway constructor");
            _mosaicContext = mosaicContext;
        }

        public List<ResidentInformation> GetAllResidents(int cursor, int limit, string firstname = null,
            string lastname = null, string postcode = null, string address = null)
        {
            var firstNameSearchPattern = GetSearchPattern(firstname);
            var lastNameSearchPattern = GetSearchPattern(lastname);
            var addressSearchPattern = GetSearchPattern(address);
            var postcodeSearchPattern = GetSearchPattern(postcode);

            Console.WriteLine("Querying for addresses");

            var queryResponse = (from person in _mosaicContext.Persons
                                 where string.IsNullOrEmpty(firstname) || EF.Functions.ILike(person.FirstName, firstNameSearchPattern)
                                 where string.IsNullOrEmpty(lastname) || EF.Functions.ILike(person.LastName, lastNameSearchPattern)
                                 where person.Id > cursor
                                 join addressInfo in _mosaicContext.Addresses.DefaultIfEmpty() on person.Id equals addressInfo.PersonId
                                 join tn in _mosaicContext.TelephoneNumbers.DefaultIfEmpty() on person.Id equals tn.PersonId
                                 where string.IsNullOrEmpty(address) ||
                                       EF.Functions.ILike(addressInfo.AddressLines.Replace(" ", ""), addressSearchPattern)
                                 where string.IsNullOrEmpty(postcode) ||
                                       EF.Functions.ILike(addressInfo.PostCode.Replace(" ", ""), postcodeSearchPattern)
                                 orderby person.Id
                                 select new
                                 {
                                     PersonId = person.Id,
                                     PersonFirstName = person.FirstName,
                                     PersonLastName = person.LastName,
                                     PersonNhsNumber = person.NhsNumber,
                                     PersonDateOfBirth = person.DateOfBirth,
                                     AddressPerson = person,
                                     AddressEndDate = addressInfo != null ? addressInfo.EndDate: null,
                                     AddressPersonId = addressInfo != null ? addressInfo.PersonId : null,
                                     AddressLines = addressInfo != null ? addressInfo.AddressLines : null,
                                     PostCode = addressInfo != null ? addressInfo.PostCode : null,
                                     Uprn = addressInfo != null ? addressInfo.Uprn : null,
                                     TelNumber = tn != null ? tn.Number : null,
                                     TelType = tn != null ? tn.Type : null
                                 }).Take(limit).ToList();

            var grouptest = queryResponse.GroupBy(x => x.PersonId, (y, z) => new ResidentInformation
            {
                MosaicId = y.ToString(),
                DateOfBirth = z.FirstOrDefault().PersonDateOfBirth.ToString(),
                FirstName = z.FirstOrDefault().PersonFirstName,
                LastName = z.FirstOrDefault().PersonLastName,
                NhsNumber = z.FirstOrDefault().PersonNhsNumber.ToString(),
                AddressList = z.Select(a => new DomainAddress
                {
                    AddressLine1 = a.AddressLines,
                    PostCode = a.PostCode
                }).Distinct().ToList(),
                Uprn = GetMostRecentUprn(z.Select(a => new Address
                {
                    EndDate = a.AddressEndDate,
                    Uprn = a.Uprn
                })),
                PhoneNumberList = z.Select(tn => new PhoneNumber
                {
                    Number = tn.TelNumber,
                    Type = Enum.Parse<PhoneType>(tn.TelType)
                }).Distinct().ToList()
            });

            Console.WriteLine(JsonConvert.SerializeObject(grouptest));


            var addressesFilteredByPostcode = _mosaicContext.Addresses
                .Include(a => a.Person)
                .Where(a => string.IsNullOrEmpty(address) || EF.Functions.ILike(a.AddressLines.Replace(" ", ""), addressSearchPattern))
                .Where(a => string.IsNullOrEmpty(postcode) || EF.Functions.ILike(a.PostCode.Replace(" ", ""), postcodeSearchPattern))
                .Where(a => string.IsNullOrEmpty(firstname) || EF.Functions.ILike(a.Person.FirstName, firstNameSearchPattern))
                .Where(a => string.IsNullOrEmpty(lastname) || EF.Functions.ILike(a.Person.LastName, lastNameSearchPattern))
                .Where(a => a.Person.Id > cursor)
                .ToList();

            Console.WriteLine("Got addresses from database");
            Console.WriteLine($"{addressesFilteredByPostcode.FirstOrDefault()?.AddressId}");

            Console.WriteLine("Querying for people without addresses");

            var peopleWithNoAddress = string.IsNullOrEmpty(postcode) && string.IsNullOrEmpty(address)
                ? QueryPeopleWithNoAddressByName(firstname, lastname, addressesFilteredByPostcode, cursor)
                : new List<ResidentInformation>();

            Console.WriteLine("Got people without an address");
            Console.WriteLine($"{peopleWithNoAddress.FirstOrDefault()?.FirstName}");

            Console.WriteLine("Map people with addresses to Domain Object");
            var peopleWithAddresses = addressesFilteredByPostcode
                .GroupBy(address => address.Person, MapPersonAndAddressesToResidentInformation)
                .ToList();

            Console.WriteLine("Add people without addresses to Domain Object");
            var allPeople = peopleWithAddresses.Concat(peopleWithNoAddress);

            Console.WriteLine("Leaving gateway method, about to return domain object");

            return allPeople.Select(AttachPhoneNumberToPerson).OrderBy(a => a.MosaicId).Take(limit).ToList();
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
        private List<ResidentInformation> QueryPeopleWithNoAddressByName(string firstname, string lastname, List<Address> addressesFilteredByPostcode, int cursor)
        {
            var firstNameSearchPattern = GetSearchPattern(firstname);
            var lastNameSearchPattern = GetSearchPattern(lastname);

            return _mosaicContext.Persons
                .Where(p => string.IsNullOrEmpty(firstname) || EF.Functions.ILike(p.FirstName, firstNameSearchPattern))
                .Where(p => string.IsNullOrEmpty(lastname) || EF.Functions.ILike(p.LastName, lastNameSearchPattern))
                .Where(p => p.Id > cursor)
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

        private static string GetSearchPattern(string str)
        {
            return $"%{str?.Replace(" ", "")}%";
        }
    }
}
