using System;
using System.Collections.Generic;
using System.Net;
using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace CrawlerMVC.Controllers.api
{
    public class Link
    {
        public string URL;
        public string Text;
        public string Title;
    }
    public class SearchController : ApiController
    {
        //http://176.23.159.28:8983/solr/testcore/query?q=p:post&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename
        // slaves calling home: GET api/Slave
        public IHttpActionResult Get()
        {

            using (var client = new WebClient())
            {
                var query = "test";
                
                Uri uri = new Uri(string.Format("http://176.23.159.28:8983/solr/testcore/query?q=p:{0}&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename",query));
                try
                {
                    var jsonReturn = client.DownloadString(uri);
                    var dyn = JsonConvert.DeserializeObject<dynamic>(jsonReturn);
                    List<Link> linkList = new List<Link>();
                    foreach (dynamic i in dyn.response.docs)
                    {
                        Link temp = new Link();
                        temp.URL = i.resourcename[0];
                        temp.Title = i.title[0];
                        temp.Text = dyn.highlighting[i.id.ToString()].p[0];
                        linkList.Add(temp);
                    }
                    return Ok(linkList);
                }
                catch (WebException e)
                {
                    return BadRequest();
                }
            }
            var httpClient = new System.Net.Http.HttpClient(new HttpClientHandler());
            //httpClient.

            //var results =
            return Ok();
        }
       
          
    }
}