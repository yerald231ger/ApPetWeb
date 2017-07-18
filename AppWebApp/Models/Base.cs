using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWebApp.Models
{
    public class Base
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime UpDate { get; set; }
        public DateTime ModDate { get; set; }
        public bool IsActive { get; set; }
    }
}
