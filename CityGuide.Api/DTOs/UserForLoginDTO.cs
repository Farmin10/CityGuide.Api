using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityGuide.Api.DTOs
{
    public class UserForLoginDTO
    {
        public string  UserName { get; set; }
        public string  Password { get; set; }
    }
}
