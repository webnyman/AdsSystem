using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ads.Web.Models
{
    [Table("tbl_annonsorer")]
    public class Annonsor
    {
        [Key]
        [Column("ann_id")]
        public int Id { get; set; }

        [Required]
        [Column("ann_typ")] // 'P' = prenumerant, 'F' = foretag
        public char Typ { get; set; }

        [Column("ann_prenr")]
        [MaxLength(20)]
        public string? Prenumerationsnummer { get; set; }

        [Required]
        [Column("ann_namn")]
        [MaxLength(100)]
        public string Namn { get; set; } = string.Empty;

        [Column("ann_orgnr")]
        [MaxLength(20)]
        public string? Organisationsnummer { get; set; }

        [Required]
        [Column("ann_telefon")]
        [MaxLength(20)]
        public string Telefon { get; set; } = string.Empty;

        [Required]
        [Column("ann_adr_utm")]
        [MaxLength(100)]
        public string Utdelningsadress { get; set; } = string.Empty;

        [Required]
        [Column("ann_postnr")]
        [MaxLength(10)]
        public string Postnummer { get; set; } = string.Empty;

        [Required]
        [Column("ann_ort")]
        [MaxLength(50)]
        public string Ort { get; set; } = string.Empty;

        [Column("ann_fakt_adr_utm")]
        [MaxLength(100)]
        public string? FakturaAdress { get; set; }

        [Column("ann_fakt_postnr")]
        [MaxLength(10)]
        public string? FakturaPostnummer { get; set; }

        [Column("ann_fakt_ort")]
        [MaxLength(50)]
        public string? FakturaOrt { get; set; }

        [Column("ann_skapad")]
        public DateTime Skapad { get; set; } = DateTime.UtcNow;

        public ICollection<Annons> Annonser { get; set; } = new List<Annons>();
    }
}
