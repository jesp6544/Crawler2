using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerLibrary.Models
{
    public class Image
    {
        [Key]
        public int id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(500)]
        [Index(IsUnique = true)]
        [Required]
        public string Path { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(500)]
        [Index]
        [Required]
        public string Alt { get; set; }

        [ForeignKey("Page_id")]
        public Page Page { get; set; }

        public virtual int? Page_id { get; set; }
    }
}