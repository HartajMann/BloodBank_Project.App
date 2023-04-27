using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
    public interface Idonor
    {
        string PhoneNumber { get; set; }
        string Name { get; set; }
        string Gender { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string BloodGroup { get; set; }
        int Age { get; set; }
    }
}
