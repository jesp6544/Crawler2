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
        private ApplicationDbContext _ctxMvc;

        public SlaveController()
        {
            _ctxMvc = new ApplicationDbContext();
            if (_ctxMvc.SlaveControls == null)
            {
                _ctxMvc.SlaveControls.Add(new SlaveControl());
                _ctxMvc.SaveChanges();
            }
            else
            {
                _slaveControl = _ctxMvc.SlaveControls.FirstOrDefault();
            }
        }

        // slaves calling home: GET api/Slave
        public string Get()
        {
            return JsonConvert.SerializeObject(_slaveControl);
        }

        // appveyor success notification: GET api/Slave/{buildNumber}
        public IHttpActionResult Get(Version buildNumber)
        {
            try
            {
                _slaveControl.BuildNumber = buildNumber;
                _ctxMvc.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}