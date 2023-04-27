using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
	public class Donate:Idonate
	{
        public string PhoneNumber { get; set; }
        public string BloodGroup { get; set; }
        public DateTime DateOfDonation { get; set; }
        public int NoOfUnits { get; set; }
    }
}
