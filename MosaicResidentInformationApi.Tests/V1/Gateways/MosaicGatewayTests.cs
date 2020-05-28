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
using DomainAddress = MosaicResidentInformationApi.V1.Domain.Address;
using Person = MosaicResidentInformationApi.V1.Infrastructure.Person;

namespace MosaicResidentInformationApi.Tests.V1.Gateways
{
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
