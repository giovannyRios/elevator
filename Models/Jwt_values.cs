using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elevator.Models
{
    public class Jwt_values
    {
        public string JWT_SET_KEY { get; set;}
        public string JWT_AUDIENCE_TOKEN { get; set;}
        public string JWT_ISSUER_TOKEN { get; set;}
        public string JWT_EXPIRE_MINUTES { get; set;}
    }
}
