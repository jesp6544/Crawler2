using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Net.Http;
using System.Web.Http;

namespace CrawlerMVC.Controllers.api
{
    public class SearchController : ApiController
    {
        // slaves calling home: GET api/Slave
        public IHttpActionResult Get()
        {
            SolrNet.Startup.InitContainer();
            SolrNet.Startup.Init<HTMLContent>("http://176.23.159.28:8983/solr/testcore");

            ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();

            var httpClient = new System.Net.Http.HttpClient(new HttpClientHandler());
            //httpClient.

            //var results =
            return Ok(results);
        }
    }
}