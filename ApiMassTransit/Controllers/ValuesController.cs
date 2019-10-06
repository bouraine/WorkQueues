using System.Net.Http;
using System.Threading.Tasks;
using MassTransit;
using Messages;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBus _bus;

        public ValuesController(IHttpClientFactory clientFactory, IBus bus)
        {
            _clientFactory = clientFactory;
            _bus = bus;
        }

        [HttpGet]
        public async Task<dynamic> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "pokemon/ditto/");
            var client = _clientFactory.CreateClient("pokemon");
            var response = await client.SendAsync(request);
            return Ok(await response.Content.ReadAsAsync<dynamic>());
        }

        public async void SendMessage()
        {
            await _bus.Publish(new TextMessage {Text = "hello world"});
        }
    }
}