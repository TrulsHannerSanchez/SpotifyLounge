using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoungeAPI.Models
{
    public class SpotifySearchResponse
    {

        public int limit { get; set; }
        public int offset { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
        public string href { get; set; }
        public List<SpotifyItem> items { get; set; }

    }

    public class SpotifyTrackResponse {

        public SpotifySearchResponse tracks { get; set; }

    }

    public class SpotifyItem {

        public string id { get; set; }
        public string name { get; set; }
        public List<SpotifyArtist> artists { get; set; }
    }

    public class SpotifyArtist {

        public string name { get; set; }


    }

    public class Track {

        public string id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }

    }
}
