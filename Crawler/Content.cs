using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Content {

    public Content() {
    }

    [Key]
    public int id { get; set; }

    [Required]
    public string tag { get; set; }

    [Required]
    public string text { get; set; }

    public int page_id { get; set; }

    [ForeignKey("page_id")]
    public Page page { get; set; }
}