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

            var newQueryResponse = new QueryResponse();

            Console.WriteLine("Querying for addresses");

            var queryResponse = (from person in _mosaicContext.Persons
                where string.IsNullOrEmpty(firstname) || EF.Functions.ILike(person.FirstName, firstNameSearchPattern)
                where string.IsNullOrEmpty(lastname) || EF.Functions.ILike(person.LastName, lastNameSearchPattern)
                where person.Id > cursor
                join a in _mosaicContext.Addresses on person.Id equals a.PersonId into joinTable
                from addressInfo in joinTable.DefaultIfEmpty()
                join telephone in _mosaicContext.TelephoneNumbers on person.Id equals telephone.PersonId into joinTable2
                from tn in joinTable2.DefaultIfEmpty()
                where string.IsNullOrEmpty(address) ||
                      EF.Functions.ILike(addressInfo.AddressLines.Replace(" ", ""), addressSearchPattern)
                where string.IsNullOrEmpty(postcode) ||
                      EF.Functions.ILike(addressInfo.PostCode.Replace(" ", ""), postcodeSearchPattern)
                orderby person.Id
                group new {p = person, a = addressInfo, t = tn} by person.Id into groupedPeople
                select new
                {
                    person = groupedPeople.Key,
                    grouping = groupedPeople
                }).ToList();

                // select new QueryResponse
                //                  {
                //                      PersonId = person.Id,
                //                      PersonFirstName = person.FirstName,
                //                      PersonLastName = person.LastName,
                //                      PersonNhsNumber = person.NhsNumber,
                //                      PersonDateOfBirth = person.DateOfBirth,
                //                      AddressPerson = person,
                //                      AddressEndDate = addressInfo != null ? addressInfo.EndDate: null,
                //                      AddressPersonId = addressInfo != null ? addressInfo.PersonId : null,
                //                      AddressLines = addressInfo != null ? addressInfo.AddressLines : null,
                //                      PostCode = addressInfo != null ? addressInfo.PostCode : null,
                //                      Uprn = addressInfo != null ? addressInfo.Uprn : null,
                //                      TelNumber = tn != null ? tn.Number : null,
                //                      TelType = tn != null ? tn.Type : null
                //                  }).ToList();

            // var grouptest = queryResponse.GroupBy(x => x.PersonId, (y, z) => new ResidentInformation
            // {
            //     MosaicId = y.ToString(),
            //     DateOfBirth = z.FirstOrDefault().PersonDateOfBirth?.ToString("O"),
            //     FirstName = z.FirstOrDefault().PersonFirstName,
            //     LastName = z.FirstOrDefault().PersonLastName,
            //     NhsNumber = z.FirstOrDefault().PersonNhsNumber.ToString(),
            //     AddressList = MapAddress(z),
            //     Uprn = GetMostRecentUprn(z),
            //     PhoneNumberList = MapPhoneNumbers(z)
            // }).Take(limit).ToList();


            return new List<ResidentInformation>();
        }

        private static List<PhoneNumber> MapPhoneNumbers(IEnumerable<QueryResponse> z)
        {
            var phoneNumberList = z.Where(tn => tn.TelNumber != null && tn.TelType != null).Select(tn => new PhoneNumber
            {
                Number = tn.TelNumber,
                Type = Enum.Parse<PhoneType>(tn.TelType)
            }).Distinct().ToList();

            return !phoneNumberList.Any() ? null : phoneNumberList;
        }

        private static List<DomainAddress> MapAddress(IEnumerable<QueryResponse> q)
        {
            var addressList = q.Where(a => a.AddressLines != null && a.PostCode != null).Select(a => new DomainAddress
            {
                AddressLine1 = a.AddressLines, PostCode = a.PostCode
            }).Distinct().ToList();

            return !addressList.Any() ? null : addressList;
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
        private static string GetMostRecentUprn(IEnumerable<QueryResponse> q)
        {
            var addressesForPerson = q.Where(u => u.Uprn != null)
                .Select(a => new Address {EndDate = a.AddressEndDate, Uprn = a.Uprn});

            return GetMostRecentUprn(addressesForPerson);
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

        private class QueryResponse
        {
            public long PersonId { get; set; }
            public string PersonFirstName { get; set; }
            public string PersonLastName { get; set; }
            public long? PersonNhsNumber { get; set; }
            public DateTime? PersonDateOfBirth { get; set; }
            public Person AddressPerson { get; set; }
            public DateTime? AddressEndDate { get; set; }
            public long? AddressPersonId { get; set; }
            public string AddressLines { get; set; }
            public string PostCode { get; set; }
            public long? Uprn { get; set; }
            public string TelNumber { get; set; }
            public string TelType { get; set; }
        }
    }

}
