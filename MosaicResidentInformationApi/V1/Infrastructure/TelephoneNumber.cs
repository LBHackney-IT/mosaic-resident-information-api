using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    [Table("dm_telephone_numbers")]
    public class TelephoneNumber
    {
        [Column("telephone_number_id")]
        [MaxLength(9)]
        [Key]
        public int Id { get; set; }

        [Column("telephone_number")]
        [MaxLength(32)]
        public string Number { get; set; }

        //In the database the types are currently:
        //Fax, Work , Pager, Ex-directory (do not disclose number), Mobile - Secondary, Primary, Home, Mobile
        [Column("telephone_number_type")]
        [MaxLength(80)]
        public string Type { get; set; }

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        [Column("person_id")]
        [MaxLength(16)]
        public int PersonId { get; set; }

    }
}
