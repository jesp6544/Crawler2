using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerLibrary.Models
{
    public class Error
    {
        public Error()
        {
        }

        [Key]
        public int id { get; set; }

        public string error { get; set; }

        public DateTime time { get; set; } = DateTime.Now;

        [ForeignKey("Page_id")]
        public Page Page { get; set; }

        [Index]
        public virtual int? Page_id { get; set; }
    }
}