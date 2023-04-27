﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank_Project.Data
{
	public interface Idonate
	{
        string PhoneNumber { get; set; }
        string BloodGroup { get; set; }
        DateTime DateOfDonation { get; set; }
        int NoOfUnits { get; set; }
    }
}
