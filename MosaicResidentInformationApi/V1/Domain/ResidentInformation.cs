using System;
using System.Collections.Generic;
using MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.V1.Domain
{
    public class ResidentInformation
    {
        public string MosaicId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uprn { get; set; }
        public string DateOfBirth { get; set; }
        public string AgeContext { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }

        public List<PhoneNumber> PhoneNumberList { get; set; }
        public List<Address> AddressList { get; set; }
        public string NhsNumber { get; set; }
    }

    public class PhoneNumber
    {
        public string Number { get; set; }
        public string Type { get; set; }
    }

    public class Address
    {
        public DateTime? EndDate { get; set; }
        public string ContactAddressFlag { get; set; }
        public string DisplayAddressFlag { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostCode { get; set; }
    }

    public class CaseNoteList
    {
        public List<CaseNote> PersonCaseNoteList { get; set; }
    }

    public class CaseNote
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int EpisodeId { get; set; }
        public int PersonVisitId { get; set; }
        public string NoteType { get; set; }
        public string Title { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string Note { get; set; }
        public int CopyOfCaseNoteId { get; set; }
    }
}
