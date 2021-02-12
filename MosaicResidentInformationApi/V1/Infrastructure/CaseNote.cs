using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MosaicResidentInformationApi.V1.Infrastructure
{
    [Table("case_notes", Schema = "dbo")]
    public class CaseNote
    {
        [Column("id")]
        [MaxLength(9)]
        public int Id { get; set; }

        [Column("person_id")]
        [MaxLength(9)]
        public int PersonId { get; set; }

        [Column("title")]
        [MaxLength(32)]
        public string Title { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("note_type")]
        [MaxLength(16)]
        public string NoteType { get; set; }
    }
}
