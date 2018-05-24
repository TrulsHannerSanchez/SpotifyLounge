using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//System.Net.Http.Headers saknades men är osäker om den passar projektet.
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LoungeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LoungeAPI.Controllers
{
    
    [Route("api/[controller]")]
    public class TracksController : Controller
    {

        private readonly IConfiguration _configuration;

        public TracksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private HttpClient httpClient = new HttpClient();
        
        private async Task<SpotifySearchResponse> getTracks(string q)
        {
            string spotifyAccessToken = await getBearerToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", spotifyAccessToken);
            
            var response = await httpClient.GetAsync("https://api.spotify.com/v1/search?type=track&q="+q);
            
            var responseString = await response.Content.ReadAsStringAsync();
            var responseContent = JsonConvert.DeserializeObject<SpotifyTrackResponse>(responseString);
            return responseContent.tracks;
            
        }

        private async Task<List<Track>> formatTracks(List<SpotifyItem> tracks) {

            List<Track> formattedTracks = new List<Track>();

            foreach (SpotifyItem item in tracks) {

                Track t = new Track();
                t.id = item.id;
                t.name = item.name;
                t.artist = String.Join(", ", item.artists.Select(artist => artist.name));
                formattedTracks.Add(t);
            }

            return formattedTracks;
        }
        
        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async Task<string> getBearerToken() {
            var clientID = _configuration.GetSection("Secrets").GetValue<string>("ClientId");
            var clientSecret = _configuration.GetSection("Secrets").GetValue<string>("ClientSecret");


            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Encode(clientID + ":" + clientSecret));
            
            var values = new Dictionary<string, string>
                {
                    //{ "Key", "Value" }
                    { "grant_type", "client_credentials" },

                };

            var content = new FormUrlEncodedContent(values);


            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responsePost = await httpClient.PostAsync("https://accounts.spotify.com/api/token", content);
            
            if (responsePost.IsSuccessStatusCode) {
                string contentAsString = await responsePost.Content.ReadAsStringAsync();
                Dictionary<string, dynamic> responseContent = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(contentAsString);
                return responseContent["access_token"];
            }

            return string.Empty;
        }

        // GET api/values
        //[HttpGet]
        //public async Task<List<Track>> Get()
        //{
        //    var searchResponse = await getTracks();
        //    var formattedTracks = await formatTracks(searchResponse.items);


        //    return formattedTracks;
        //}

        //Nästa steg!!
        [HttpGet]
        public async Task<List<Track>> Get([FromQuery] string q) {

            var searchResponse = await getTracks(q);
            var formattedTracks = await formatTracks(searchResponse.items);
            
            return formattedTracks;
            

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public JsonResult Post([FromBody]string value)
        {

            return Json(new
            {
                message = "The value was not saved",
                value,
            });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
