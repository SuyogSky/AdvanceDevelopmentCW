﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class OtpDetailsModel
    {
        public string Email { get; set; }
        public string Otp { get; set; } 
        public string NewPassword {  get; set; }    
    }
}
