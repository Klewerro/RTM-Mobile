using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Models
{
    public class Departure
    {
        public string Number { get; set; }

        public string Direction { get; set; }

        public string Time { get; set; }

        public bool HaveTicketMachine { get; set; }

        public int? RouteId { get; set; }
    }
}
