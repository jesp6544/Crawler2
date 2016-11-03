using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerLibrary.Models
{
    public class Page
    {
        public Page()
        {
        }

        [Key]
        public int id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(500)]
        [Index(IsUnique = true)]
        [Required]
        public string url { get; set; }

        public string title { get; set; }

        [Index("IX_AttemptAndScanned", 2)]
        public DateTime? LastAttempt { get; set; }

        [Index("IX_AttemptAndScanned", 1)]
        public bool? scanned { get; set; } = false;

        public ICollection<Content> content { get; set; }

        public ICollection<Link> fromLinks { get; set; }

        public ICollection<Link> toLinks { get; set; }
    }
}