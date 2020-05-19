using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    [Table("dm_addresses")]
    public class Address
    {
        [Column("ref_addresses_people_id")]
        [MaxLength(9)]
        [Key]
        public int PersonAddressId { get; set; }

        [Column("ref_address_id")]
        [MaxLength(9)]
        public int AddressId { get; set; }

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        [Column("person_id")]
        [MaxLength(16)]
        public int PersonId { get; set; }

        // If this is populated it means the address is historical
        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("address")]
        [MaxLength(464)]
        public string AddressLines { get; set; }

        [Column("post_code")]
        [MaxLength(16)]
        public string PostCode { get; set; }

        [Column("unique_id")]
        [MaxLength(15)]
        public int Uprn { get; set; }
    }
}