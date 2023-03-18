using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Elevator.Models
{
    public class floor
    {
        [Range(1,15, ErrorMessage = "Piso actual no valido")]
        public int currentFloor { get; set; }

        [Range(1, 15, ErrorMessage = "Piso destino no valido")]
        public int destinationFloor { get; set; }
    }
}
