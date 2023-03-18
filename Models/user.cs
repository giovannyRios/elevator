using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Elevator.Models
{
    public class user
    {

        [Required]
        public string userName { get; set; }
    }
}
