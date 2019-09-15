using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public ValuesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<dynamic> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "pokemon/ditto/");
            var client = _clientFactory.CreateClient("pokemon");
            var response = await client.SendAsync(request);
            return Ok(await response.Content.ReadAsAsync<dynamic>());
        }
    }
}