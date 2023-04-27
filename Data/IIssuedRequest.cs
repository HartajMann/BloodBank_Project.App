using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
    public interface IIssuedRequest
    {
        string Name { get; set; }
        string Mobile { get; set; }
        string Hospital { get; set; }
        string Purpose { get; set; }
        string BloodGroup { get; set; }
        int Units { get; set; }
        DateTime DateOfIssue { get; set; }
    }

}
