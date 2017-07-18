using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWebApp.Models
{
    public class Pet : Base
    {
        public string Race { get; set; }
        public int Wight { get; set; }
        public DateTime Birthdate { get; set; }
        public string ImageProfileId { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PetTypeId { get; set; }
        public PetType PetType { get; set; }
    }
    
}
