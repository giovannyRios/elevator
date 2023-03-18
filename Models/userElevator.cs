using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Elevator.Models
{
    public class userElevator
    {
        [Required]
        public string userName { get; set; }

        [Range(1, 15, ErrorMessage = "Piso destino no valido")]
        public int piso { get; set; }
    }
}
