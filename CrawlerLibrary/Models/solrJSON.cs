using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerLibrary.Models
{
    class solrJSON
    {

        public class Rootobject
        {
            public Responseheader responseHeader { get; set; }
            public Response response { get; set; }
            public Highlighting highlighting { get; set; }
        }

        public class Responseheader
        {
            public int status { get; set; }
            public int QTime { get; set; }
            public Params _params { get; set; }
        }

        public class Params
        {
            public string q { get; set; }
            public string hl { get; set; }
            public string fl { get; set; }
            public string hlfragsize { get; set; }
            public string hlfl { get; set; }
        }

        public class Response
        {
            public int numFound { get; set; }
            public int start { get; set; }
            public Doc[] docs { get; set; }
        }

        public class Doc
        {
            public string id { get; set; }
            public string[] resourcename { get; set; }
            public string[] title { get; set; }
        }

        public class Highlighting
        {
            public _3840410 _3840410 { get; set; }
            public _3840422 _3840422 { get; set; }
            public _3839701 _3839701 { get; set; }
            public _3839963 _3839963 { get; set; }
            public _3840393 _3840393 { get; set; }
            public _3840728 _3840728 { get; set; }
            public _3839637 _3839637 { get; set; }
            public _3839763 _3839763 { get; set; }
            public _3839930 _3839930 { get; set; }
            public _3840053 _3840053 { get; set; }
        }

        public class _3840410
        {
            public string[] p { get; set; }
        }

        public class _3840422
        {
            public string[] p { get; set; }
        }

        public class _3839701
        {
            public string[] p { get; set; }
        }

        public class _3839963
        {
            public string[] p { get; set; }
        }

        public class _3840393
        {
            public string[] p { get; set; }
        }

        public class _3840728
        {
            public string[] p { get; set; }
        }

        public class _3839637
        {
            public string[] p { get; set; }
        }

        public class _3839763
        {
            public string[] p { get; set; }
        }

        public class _3839930
        {
            public string[] p { get; set; }
        }

        public class _3840053
        {
            public string[] p { get; set; }
        }

    }
}
