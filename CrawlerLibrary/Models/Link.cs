using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerLibrary.Models
{
    public class Link
    {
        public Link()
        {
        }

        [Key]
        public int id { get; set; }

        [Required]
        public string text { get; set; }

        [Required]
        public bool local { get; set; } = false;

        //[Required]
        public virtual int? from_id { get; set; }

        [Index("IX_from_id", 1)]
        [ForeignKey("from_id")]
        public Page from { get; set; }

        public virtual int? to_id { get; set; }

        [Index("IX_from_id", 2)]
        [ForeignKey("to_id")]
        public Page to { get; set; }

        /*[Required]
    public int page_id { get; set; }

    [ForeignKey("page_id")]
    public Page page { get; set; }*/
    }
}