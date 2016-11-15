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

        public SlaveController()
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.SlaveControls.Any() == false) // Create initial dummy SlaveControl item for a fresh db
                {
                    _slaveControl = new SlaveControl { TimeStamp = DateTime.Now };
                    ctx.SlaveControls.Add(_slaveControl);
                    ctx.SaveChanges();
                }
            }
        }

        // slaves calling home: GET api/Slave
        public string GetAllSlaves()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return JsonConvert.SerializeObject(ctx.SlaveControls.First());
            }
        }

        // appveyor success notification: GET api/Slave/{buildNumber}
        public IHttpActionResult Get(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    _slaveControl = ctx.SlaveControls.First();
                    _slaveControl.BuildNumber = id;
                    _slaveControl.TimeStamp = DateTime.Now;
                    ctx.SlaveControls.Add(_slaveControl);
                    ctx.SaveChanges();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
        }
    }
}