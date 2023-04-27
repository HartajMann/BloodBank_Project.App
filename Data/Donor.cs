using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
    public class Donor : Idonor
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BloodGroup { get; set; }
        public int Age { get; set; }
    }
}
