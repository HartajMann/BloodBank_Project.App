using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
	public class InvalidDonorRegisterException:Exception
	{
		public InvalidDonorRegisterException() : base("") { }

		public InvalidDonorRegisterException(string message) : base(message) { }
	}
}
