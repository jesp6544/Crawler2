using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Page {

    public Page() {
    }

    [Key]
    public int id { get; set; }

    [Index(IsUnique = true)]
    [Required]
    public string url { get; set; }

    public string title { get; set; }

    public bool? scanned { get; set; } = false;

    public ICollection<Content> content { get; set; }

    public ICollection<Link> fromLinks { get; set; }

    public ICollection<Link> toLinks { get; set; }
}