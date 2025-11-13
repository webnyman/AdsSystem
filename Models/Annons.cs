using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ads.Web.Models
{
    [Table("tbl_ads")]
    public class Annons
    {
        [Key]
        [Column("ad_id")]
        public int Id { get; set; }

        [Required]
        [Column("ad_annonsor_id")]
        public int AnnonsorId { get; set; }

        [Required]
        [Column("ad_varans_pris")]
        public decimal VaransPris { get; set; }

        [Required]
        [Column("ad_innehall")]
        public string Innehall { get; set; } = string.Empty;

        [Required]
        [Column("ad_rubrik")]
        [MaxLength(200)]
        public string Rubrik { get; set; } = string.Empty;

        [Required]
        [Column("ad_annonspris")]
        public decimal Annonspris { get; set; }

        [Column("ad_skapad")]
        public DateTime Skapad { get; set; } = DateTime.UtcNow;

        public Annonsor? Annonsor { get; set; }
    }
}
