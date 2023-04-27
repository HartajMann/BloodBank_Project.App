using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
    public abstract class BaseRequest : IIssuedRequest
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Hospital { get; set; }
        public string Purpose { get; set; }
        public string BloodGroup { get; set; }
        public int Units { get; set; }
        public DateTime DateOfIssue { get; set; }
    }
}
