using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Link {

    public Link() {
    }


    [Key]
    public int id { get; set; }

    [Required]
    public string text { get; set; }

    [Required]
    public bool local { get; set; } = false;

    //[Required]
    public virtual int? from_id { get; set; }

    [ForeignKey("from_id")]
    public Page from { get; set; }

    public virtual int? to_id { get; set; }

    [ForeignKey("to_id")]
    public Page to { get; set; }

    /*[Required]
    public int page_id { get; set; }

    [ForeignKey("page_id")]
    public Page page { get; set; }*/
}