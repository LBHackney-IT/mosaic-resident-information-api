using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using MosaicResidentInformationApi.Tests.V1.Helper;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Gateways;
using NUnit.Framework;
using System.Linq;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;
using DomainAddress = MosaicResidentInformationApi.V1.Domain.Address;
using Person = MosaicResidentInformationApi.V1.Infrastructure.Person;

namespace MosaicResidentInformationApi.Tests.V1.Gateways
{
    [NonParallelizable]
    [TestFixture]
    public class MosaicGatewayTests : DatabaseTests
    {
        private readonly Fixture _faker = new Fixture();
        private MosaicGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new MosaicGateway(MosaicContext);
        }

        [Test]
        public void GetResidentInformationByPersonId_WhenThereAreNoMatchingRecords_ReturnsNull()
        {
            var response = _classUnderTest.GetEntityById(123);

            response.Should().BeNull();
        }

        [Test]
        public void GetResidentInformationByPersonId_ReturnsPersonalDetails()
        {
            var databaseEntity = AddPersonRecordToDatabase();
            var response = _classUnderTest.GetEntityById(databaseEntity.Id);

            response.FirstName.Should().Be(databaseEntity.FirstName);
            response.LastName.Should().Be(databaseEntity.LastName);
            response.NhsNumber.Should().Be(databaseEntity.NhsNumber.ToString());
            response.DateOfBirth.Should().Be(databaseEntity.DateOfBirth.ToString("O"));
            response.Should().NotBe(null);
        }

        [Test]
        public void GetResidentInformationByPersonId_ReturnsAddressDetails()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var response = _classUnderTest.GetEntityById(databaseEntity.Id);

            var expectedDomainAddress = new DomainAddress
            {
                AddressLine1 = address.AddressLines,
                PostCode = address.PostCode,
            };
            response.AddressList.Should().BeEquivalentTo(new List<DomainAddress> { expectedDomainAddress });
            response.Uprn.Should().Be(address.Uprn.ToString());
        }

        [Test]
        public void GetResidentInformationByPersonId_IfThereAreMultipleAddresses_ReturnsTheUprnForMostCurrentAddress()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var addressOld = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            addressOld.EndDate = DateTime.Now.AddDays(-67);
            MosaicContext.Addresses.Add(addressOld);

            var addressCurrent = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            addressCurrent.EndDate = null;
            MosaicContext.Addresses.Add(addressCurrent);
            MosaicContext.SaveChanges();

            var response = _classUnderTest.GetEntityById(databaseEntity.Id);
            response.Uprn.Should().Be(addressCurrent.Uprn.ToString());
        }

        [Test]
        public void GetResidentInformationByPersonId_ReturnsContactDetails()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var phoneNumber = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseEntity.Id);
            var type = PhoneType.Primary.ToString();
            phoneNumber.Type = type;
            MosaicContext.TelephoneNumbers.Add(phoneNumber);
            MosaicContext.SaveChanges();

            var response = _classUnderTest.GetEntityById(databaseEntity.Id);
            response.PhoneNumberList.Should().BeEquivalentTo(new List<PhoneNumber>
            {
                new PhoneNumber {Number = phoneNumber.Number, Type = PhoneType.Primary}
            });
        }

        [Test]
        public void GetAllResidents_IfThereAreNoResidents_ReturnsAnEmptyList()
        {
            _classUnderTest.GetAllResidents().Should().BeEmpty();
        }

        [Test]
        public void GetAllResident_IfThereAreResidentsWillReturnThem()
        {
            var databaseEntity = AddPersonRecordToDatabase();
            var databaseEntity1 = AddPersonRecordToDatabase();
            var databaseEntity2 = AddPersonRecordToDatabase();

            var listOfPersons = _classUnderTest.GetAllResidents();

            listOfPersons.Should().ContainEquivalentOf(databaseEntity.ToDomain());
            listOfPersons.Should().ContainEquivalentOf(databaseEntity1.ToDomain());
            listOfPersons.Should().ContainEquivalentOf(databaseEntity2.ToDomain());
        }

        [Test]
        public void GetAllResidents_IfThereAreResidentAddressesWillReturnThem()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents();

            listOfPersons
                .Where(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .First()
                .AddressList
                .Should().ContainEquivalentOf(address.ToDomain());
        }

        [Test]
        public void GetAllResidents_IfThereAreResidentPhoneNumbersWillReturnThem()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var phoneNumber = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseEntity.Id);
            var type = PhoneType.Primary.ToString();
            phoneNumber.Type = type;
            MosaicContext.TelephoneNumbers.Add(phoneNumber);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents();

            listOfPersons
                .Where(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .First()
                .PhoneNumberList
                .Should().ContainEquivalentOf(phoneNumber.ToDomain());
        }

        [Test]
        public void GetAllResidents_ReturnsResidentsUPRN()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents();

            listOfPersons
                .Where(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .First()
                .Uprn
                .Should().Be(address.Uprn.ToString());
        }

        [Test]
        public void GetAllResidentsWithFirstNameQueryParameter_ReturnsMatchingResident()
        {
            var databaseEntity = AddPersonRecordToDatabase(firstname: "ciasom");
            var databaseEntity1 = AddPersonRecordToDatabase(firstname: "shape");
            var databaseEntity2 = AddPersonRecordToDatabase(firstname: "Ciasom");

            var listOfPersons = _classUnderTest.GetAllResidents(firstname: "ciasom");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(databaseEntity.ToDomain());
            listOfPersons.Should().ContainEquivalentOf(databaseEntity2.ToDomain());
        }

        [Test]
        public void GetAllResidentsWithLastNameQueryParameter_ReturnsMatchingResident()
        {
            var databaseEntity = AddPersonRecordToDatabase(lastname: "tessellate");
            var databaseEntity1 = AddPersonRecordToDatabase(lastname: "square");
            var databaseEntity2 = AddPersonRecordToDatabase(lastname: "Tessellate");

            var listOfPersons = _classUnderTest.GetAllResidents(lastname: "tessellate");
            listOfPersons.Count.Should().Be(2);
            listOfPersons.Should().ContainEquivalentOf(databaseEntity.ToDomain());
            listOfPersons.Should().ContainEquivalentOf(databaseEntity2.ToDomain());
        }

        [Test]
        public void GetAllResidentsWithNameQueryParameters_ReturnsMatchingResidentOnlyOnce()
        {
            var databaseEntity = AddPersonRecordToDatabase(firstname: "ciasom", lastname: "Tessellate");

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id);
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(firstname: "ciasom", lastname: "Tessellate");
            listOfPersons.Count.Should().Be(1);
            listOfPersons.First().MosaicId.Should().Be(databaseEntity.Id.ToString());
        }

        [Test]
        public void GetAllResidentsWithPostCodeQueryParameter_ReturnsMatchingResident()
        {
            var databaseEntity = AddPersonRecordToDatabase();
            var databaseEntity1 = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, "E8 1DY");
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.Id, "E8 5TG");
            MosaicContext.Addresses.Add(address1);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(postcode: "E8 1DY");
            listOfPersons.Count.Should().Be(1);
            listOfPersons
                .First(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .AddressList
                .Should().ContainEquivalentOf(address.ToDomain());
        }

        [Test]
        public void GetAllResidentsWithPostCodeQueryParameter_WhenAPersonHasTwoAddress_ReturnsOneRecordWithAListOfAddresses()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, "E8 1DY");
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, "E8 1DY");
            MosaicContext.Addresses.Add(address1);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(postcode: "E8 1DY").ToList();
            listOfPersons.Count.Should().Be(1);
            listOfPersons
                .First(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .AddressList.Count
                .Should().Be(2);
        }

        [Test]
        public void GetAllResidentsWithPostCodeQueryParameter_ReturnsPhoneNumberAndUprnWithResidentInformation()
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, "E8 1DY");
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var phoneNumber = TestHelper.CreateDatabaseTelephoneNumberForPersonId(databaseEntity.Id);
            MosaicContext.TelephoneNumbers.Add(phoneNumber);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(postcode: "E8 1DY").ToList();

            var personUnderTest = listOfPersons
                .First(p => p.MosaicId.Equals(databaseEntity.Id.ToString()));

            personUnderTest.PhoneNumberList.Should().ContainEquivalentOf(phoneNumber.ToDomain());
            personUnderTest.Uprn.Should().Be(address.Uprn.ToString());
        }

        [Test]
        public void GetAllResidentsWithNameAndPostCodeQueryParameter_ReturnsMatchingResident()
        {
            var databaseEntity = AddPersonRecordToDatabase(firstname: "ciasom");
            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, "E8 1DY");

            var databaseEntity1 = AddPersonRecordToDatabase(firstname: "wrong name");
            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.Id, "E8 1DY");

            var databaseEntity2 = AddPersonRecordToDatabase(firstname: "ciasom");
            var address2 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity2.Id, "E8 5RT");

            MosaicContext.Addresses.AddRange(new List<Address> { address, address1, address2 });
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(firstname: "ciasom", postcode: "E8 1DY").ToList();

            listOfPersons.Count.Should().Be(1);
            listOfPersons.First().MosaicId.Should().Be(databaseEntity.Id.ToString());
            listOfPersons.First()
                .AddressList
                .Should().ContainEquivalentOf(address.ToDomain());
        }

        [TestCase("E81DY")]
        [TestCase("e8 1DY")]
        public void GetAllResidentsWithPostCodeQueryParameter_IgnoresFormatting(string postcode)
        {
            var databaseEntity = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, postcode);
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(postcode: "E8 1DY");

            listOfPersons.Count.Should().Be(1);

            listOfPersons.First().MosaicId.Should().Be(databaseEntity.Id.ToString());
            listOfPersons.First().AddressList
                .Should().ContainEquivalentOf(address.ToDomain());
        }

        [TestCase("1 My Street")]
        [TestCase("My Street")]
        [TestCase("1 My Street, Hackney, London")]
        [TestCase("Hackney")]
        public void GetAllResidentsWithAddressQueryParameter_ReturnsMatchingResident(string addressQuery)
        {
            var databaseEntity = AddPersonRecordToDatabase();
            var databaseEntity1 = AddPersonRecordToDatabase();

            var address = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity.Id, address: "1 My Street, Hackney, London");
            MosaicContext.Addresses.Add(address);
            MosaicContext.SaveChanges();

            var address1 = TestHelper.CreateDatabaseAddressForPersonId(databaseEntity1.Id, address: "5 Another Street, Lambeth, London");
            MosaicContext.Addresses.Add(address1);
            MosaicContext.SaveChanges();

            var listOfPersons = _classUnderTest.GetAllResidents(address: addressQuery).ToList();
             listOfPersons.Count.Should().Be(1);
            listOfPersons
                .First(p => p.MosaicId.Equals(databaseEntity.Id.ToString()))
                .AddressList
                .Should().ContainEquivalentOf(address.ToDomain());
        }

        private Person AddPersonRecordToDatabase(string firstname = null, string lastname = null)
        {
            var databaseEntity = TestHelper.CreateDatabasePersonEntity();
            databaseEntity.FirstName = firstname ?? databaseEntity.FirstName;
            databaseEntity.LastName = lastname ?? databaseEntity.LastName;
            MosaicContext.Persons.Add(databaseEntity);
            MosaicContext.SaveChanges();
            return databaseEntity;
        }
    }
}
