using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
        [Route("api/Search/{query}/{page?}")]
        public IHttpActionResult Get(string query, int page = 1)
        {
            using (var client = new WebClient())
            {
                int offset = (page - 1)*10;
                //http://176.23.159.28:8983/solr/testcore/query?q=p:{0}&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename&start={1}
                //http://176.23.159.28:8983/solr/testcore/query?q={0}&qf=resourcename^2+title^3&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename&start={1}
                Uri uri =
                    new Uri(
                        string.Format(
                            "http://176.23.159.28:8983/solr/testcore/query?q={0}&qf=resourcename^2+title^3&hl=true&hl.fl=p&hl.fragsize=500&fl=id+title+resourcename&start={1}",
                            query, offset));
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
                        try
                        {
                            temp.Text = dyn.highlighting[i.id.ToString()].p[0];
                        }
                        catch (Exception)
                        {
                        }
                        linkList.Add(temp);
                    }
                    return Ok(linkList);
                }
                catch (WebException e)
                {
                    return BadRequest();
                }
            }
        }
    }
}