﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWebApp.Models
{
    public class Veterinary : Base
    {
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public string ImageProfileId { get; set; }

        public virtual ICollection<VeterinaryVetService> VeterinaryVetServices { get; set; }
    }
}
