using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CityGuide.Api.Models
{
    public class City
    {
        public City()
        {
            Photos = new List<Photo>();
        }

        public int Id { get; set; }
        public int  UserId { get; set; }
        public string  Name { get; set; }
        public string  Description { get; set; }
        [JsonIgnore]
        public List<Photo> Photos { get; set; }
        public User User { get; set; }
    }
}
