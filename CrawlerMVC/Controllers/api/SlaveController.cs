using CrawlerLibrary.Models;
using CrawlerMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrawlerMVC.Controllers.api
{
    public class SlaveController : ApiController
    {
        private SlaveControl _slaveControl;
        private ApplicationDbContext ctx_mvc = new ApplicationDbContext();

        public SlaveController()
        {
        }

        // slaves calling home: GET api/Slave
        public IHttpActionResult Get()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return Ok(ctx.SlaveControls.First());
            }
        }

        // appveyor success notification: PUT api/Slave/{buildNumber}
        public IHttpActionResult Put(int id)
        {
            try
            {
                _slaveControl = ctx_mvc.SlaveControls.First();
                _slaveControl.BuildNumber = id;
                _slaveControl.TimeStamp = DateTime.Now;
                ctx_mvc.SaveChanges();
                return Ok(_slaveControl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}