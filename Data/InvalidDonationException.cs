using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
    public class InvalidDonationException:Exception
    {
        public InvalidDonationException() : base("") { }

        public InvalidDonationException(string message) : base(message) { }
    }
}
