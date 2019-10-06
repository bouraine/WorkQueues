using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Elasticsearch;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWorker.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly PeopleReader _peopleReader;

        public ValuesController(PeopleReader peopleReader)
        {
            _peopleReader = peopleReader;
        }

        [HttpGet]
        public async Task<ActionResult<List<People>>> Get() => (await _peopleReader.GetAllPeope()).Take(10).ToList();
    }
}