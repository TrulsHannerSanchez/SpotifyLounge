using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LoungeAngular.Controllers
{
    public class AuthorizeController : Controller
    {

        private readonly IConfiguration _configuration;

        public AuthorizeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string clientId = _configuration.GetValue<string>("Secrets:ClientId");
            IConfigurationSection connections = _configuration.GetSection("Connections");
            string angularUrl = connections.GetValue<string>("AngularUrl");
            string spotifyAccountsUrl = connections.GetValue<string>("SpotifyAccounts");
            string responseType = "code";
            string[] scope = new string[] { "user-read-playback-state", "user-modify-playback-state" };

            string redirect = $"{spotifyAccountsUrl}/authorize" +
                                    $"?client_id={clientId}" +
                                    $"&redirect_uri={angularUrl}/authorize/callback" +
                                    $"&response_type={responseType}" +
                                    $"&scope={string.Join(',', scope)}";

            return Redirect(redirect);
        }

        public IActionResult Callback([FromQuery]string code, [FromQuery]string state, [FromQuery] string error)
        {
            return View();
        }
    }
}