using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Error
    {
        public Error(){
        }

        [Key]
        public int id { get; set; }

        public string error { get; set; }

        public DateTime time { get; set; } = DateTime.Now;

        public Page Page { get; set; }
    }
}
