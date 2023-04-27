using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
	public class InvalidIssueBloodException:Exception
	{
		public InvalidIssueBloodException() :base(""){ }

		public InvalidIssueBloodException(string message) : base(message) { }
	}
}
