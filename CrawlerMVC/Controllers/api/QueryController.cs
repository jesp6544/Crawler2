using CrawlerLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrawlerMVC.Controllers.api {

    private class


    public class QueryController : ApiController {

        // slaves calling home: GET api/Slave
        public IHttpActionResult Get(string query, int page = 1) {
            using(var ctx = new CrawlerContext()) {
                int limit = 10;
                int offset = page - 1 * limit;

                string sql = @"
                    SELECT TOP 10 *
                    FROM Contents

                    SELECT TOP 10 *
                    FROM [dbo].Contents AS FT_TBL

                    INNER JOIN
                    FREETEXTTABLE(
                        Contents,
                        [text],
                        '{0}') AS KEY_TBL
                    ON FT_TBL.id = KEY_TBL.[KEY]
                    ORDER BY KEY_TBL.RANK DESC

                    OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY
                ";

                ctx.Database.ExecuteSqlCommand(sql, query, offset, limit);

                List<Content> content = ctx.Content.SqlQuery(sql).ToList();

                return Ok(
                    ctx.Content.SqlQuery(sql).ToList()
                );
            }
        }
    }
}