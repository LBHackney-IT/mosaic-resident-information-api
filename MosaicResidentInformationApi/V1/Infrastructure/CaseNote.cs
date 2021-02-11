using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    [Table("CASE_NOTES", Schema = "dbo")]
    public class CaseNote
    {
        [Column("NOTE_ID")]
        [MaxLength(9)]
        public int Id { get; set; }

        [Column("person_id")]
        [MaxLength(9)]
        public int PersonId { get; set; }

        [Column("TITLE")]
        [MaxLength(32)]
        public string Title { get; set; }

        [Column("NOTE")]
        public string Note { get; set; }

    }
}
